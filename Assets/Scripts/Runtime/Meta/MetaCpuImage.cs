#if METAOPENXR_2_4_0_PRE_1_OR_NEWER
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MetaCpuImage : MonoBehaviour
    {
        [SerializeField] 
        [Tooltip("The UI RawImage used to display the camera image.")]
        RawImage m_RawImage;

        [SerializeField]
        [Tooltip("The AR Camera Manager which manages the XR Camera Subsystem.")]
        ARCameraManager m_CameraManager;

        private readonly List<Action<DataElement, XRCpuImage, XRCameraIntrinsics>> m_ValueSetFunctions = new()
        {
            // FocusLength
            (datafield, image, intrinsics) => datafield.SetFieldValue(intrinsics.focalLength),
            // PrincipalPoint 
            (datafield, image, intrinsics) => datafield.SetFieldValue(intrinsics.principalPoint),
            // CameraResolution
            (datafield, image, intrinsics) => datafield.SetFieldValue(intrinsics.resolution),
            // ImageFormat
            (datafield, image, intrinsics) => datafield.SetFieldValue(image.format),
            // ImagePlaneCount
            (datafield, image, intrinsics) => datafield.SetFieldValue(image.planeCount),
            // ImageTimestamp
            (datafield, image, intrinsics) => datafield.SetFieldValue(image.timestamp)
        };

        [SerializeField]
        [Tooltip("The data element for the various displayed properties.")]
        DataElement[] m_DataFieldElements;

        [SerializeField]
        Toggle m_MirrorXToggle;

        [SerializeField]
        Toggle m_MirrorYToggle;

        [SerializeField]
        TextMeshProUGUI m_ImageResolutionValue;

        [SerializeField]
        TextMeshProUGUI m_ErrorText;

        Texture2D m_CameraTexture;
        Vector2Int m_LastKnownImageDimensions;

        bool m_IsCameraInitialized;
        Vector2Int m_ConversionDimensions;
        XRCpuImage.Transformation m_ConversionTransform = XRCpuImage.Transformation.None;

        void Awake()
        {
            Assert.IsTrue(m_RawImage != null, "RawImage is not assigned in the Inspector.");
            Assert.IsTrue(m_CameraManager != null, "CameraImage is not assigned in the Inspector.");
            Assert.IsTrue(m_MirrorXToggle != null, "MirrorXToggle is not assigned in the Inspector.");
            Assert.IsTrue(m_MirrorYToggle != null, "MirrorYToggle is not assigned in the Inspector.");
            Assert.IsTrue(m_ImageResolutionValue != null, "ImageResolutionValue is not assigned in the Inspector.");

            foreach (var field in m_DataFieldElements)
            {
                if (field != null)
                {
                    field.gameObject.SetActive(true);
                    field.SetFieldLabel(field.gameObject.name);
                    field.SetFieldValue("<unknown>");
                }
            }
        }

        void OnEnable()
        {
            ARSession.stateChanged += OnSessionStateChanged;
        }

        void OnDisable()
        {
            ReleaseResources();
        }

        void OnDestroy()
        {
            ReleaseResources();
        }

        void ReleaseResources()
        {
            ARSession.stateChanged -= OnSessionStateChanged;

            m_IsCameraInitialized = false;

            if (m_CameraTexture != null)
            {
                Destroy(m_CameraTexture);
                m_CameraTexture = null;
            }
        }

        void OnSessionStateChanged(ARSessionStateChangedEventArgs args)
        {
            if (args.state >= ARSessionState.SessionTracking && !m_IsCameraInitialized)
            {
                m_IsCameraInitialized = true;
            }

            else if (args.state < ARSessionState.SessionTracking && m_IsCameraInitialized)
            {
                m_IsCameraInitialized = false;
            }
        }

        void Update()
        {
            if (!m_IsCameraInitialized)
            {
                return;
            }

            if (m_CameraManager.TryAcquireLatestCpuImage(out XRCpuImage cpuImage))
            {
                m_LastKnownImageDimensions = cpuImage.dimensions;
                {
                    UpdateTexture(cpuImage);
                    UpdateDataFields(cpuImage);
                    UpdateImageResolutionDisplay();
                }
                cpuImage.Dispose();
            }
        }

        unsafe void UpdateTexture(XRCpuImage cpuImage)
        {
            if (m_ConversionDimensions.x == 0 || m_ConversionDimensions.y == 0)
            {
                m_ConversionDimensions.x = cpuImage.width;
                m_ConversionDimensions.y = cpuImage.height;
            }

            var conversionParams = new XRCpuImage.ConversionParams
            {
                inputRect = new RectInt(0, 0, cpuImage.width, cpuImage.height),
                outputDimensions = new Vector2Int(m_ConversionDimensions.x, m_ConversionDimensions.y),
                outputFormat = TextureFormat.RGBA32,
                transformation = m_ConversionTransform
            };

            var hasValidTexture = m_CameraTexture != null;

            if (hasValidTexture && (m_CameraTexture.width != conversionParams.outputDimensions.x || 
                                    m_CameraTexture.height != conversionParams.outputDimensions.y))
            {
                Destroy(m_CameraTexture);
                hasValidTexture = false;
            }

            if (!hasValidTexture)
            {
                m_CameraTexture = new Texture2D(conversionParams.outputDimensions.x, conversionParams.outputDimensions.y, 
                    conversionParams.outputFormat, false);
            }

            var rawTextureData = m_CameraTexture.GetRawTextureData<byte>();
            cpuImage.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);

            m_CameraTexture.Apply();
            m_RawImage.texture = m_CameraTexture;
            // NOTE: this scale is done because the "default" orientation of the CPU image coming from the Meta
            // Quest cameras is upside-down.
            m_RawImage.rectTransform.localScale = new Vector3(1, -1, 1);
            m_RawImage.enabled = true;
        }

        bool CanUpRes() => m_ConversionDimensions.x < m_LastKnownImageDimensions.x;
        bool CanDownRes() => m_ConversionDimensions.x % 2 == 0 && m_ConversionDimensions.y % 2 == 0;

        public void UpRes()
        {
            if (CanUpRes())
            {
                m_ConversionDimensions.x <<= 1;
                m_ConversionDimensions.y <<= 1;
            }
        }

        public void DownRes()
        {
            if (CanDownRes())
            {
                m_ConversionDimensions.x >>= 1;
                m_ConversionDimensions.y >>= 1;
            }
        }

        bool IsMirrorX() => (m_ConversionTransform & XRCpuImage.Transformation.MirrorX) != 0;
        bool IsMirrorY() => (m_ConversionTransform & XRCpuImage.Transformation.MirrorY) != 0;
        
        /// <summary>
        /// NOTE: "MirrorX" really means to "flip vertically" since the mirroring is done along the X axis and
        /// does NOT mean to flip horizontally
        /// </summary>
        public void HandleMirrorXToggle(bool mirrorX)
        {
            mirrorX = m_MirrorXToggle.isOn;
            m_MirrorXToggle.targetGraphic.enabled = !mirrorX;
            m_MirrorXToggle.graphic.enabled = mirrorX;
            SetConversionTransform(mirrorX, IsMirrorY());
        }

        /// <summary>
        /// NOTE: "MirrorY" really means to "flip horizontally" since the mirroring is done along the Y axis
        /// and does NOT mean to flip vertically
        /// </summary>
        public void HandleMirrorYToggle(bool mirrorY)
        {
            mirrorY = m_MirrorYToggle.isOn;
            m_MirrorYToggle.targetGraphic.enabled = !mirrorY;
            m_MirrorYToggle.graphic.enabled = mirrorY;
            SetConversionTransform(IsMirrorX(), mirrorY);
        }

        void SetConversionTransform(bool mirrorX, bool mirrorY)
        {
            m_ConversionTransform = (mirrorX, mirrorY) switch
            {
                (false, false) => XRCpuImage.Transformation.None,
                (false, true) => XRCpuImage.Transformation.MirrorY,
                (true, false) => XRCpuImage.Transformation.MirrorX,
                (true, true) => XRCpuImage.Transformation.MirrorX | XRCpuImage.Transformation.MirrorY
            };
        }

        void UpdateDataFields(XRCpuImage cpuImage)
        {
            var success = m_CameraManager.TryGetIntrinsics(out var intrinsics);
            
            if (success && m_ErrorText.IsActive())
                m_ErrorText.gameObject.SetActive(false);

            int fieldIndex = 0;
            foreach (var field in m_DataFieldElements)
            {
                if (field != null && fieldIndex < m_ValueSetFunctions.Count && m_ValueSetFunctions[fieldIndex] != null)
                {
                    m_ValueSetFunctions[fieldIndex].Invoke(field, cpuImage, intrinsics);
                }
                fieldIndex++;
            }
        }

        void UpdateImageResolutionDisplay()
        {
            m_ImageResolutionValue.text = $"{m_ConversionDimensions.x} x  {m_ConversionDimensions.y}";
        }
    }
}
#endif
