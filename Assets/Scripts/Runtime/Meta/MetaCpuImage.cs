#if METAOPENXR_2_6_0_1_OR_NEWER && (UNITY_EDITOR || UNITY_ANDROID)
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.OpenXR.Features.Meta;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MetaCpuImage : MonoBehaviour
    {
        /// <summary>
        /// Which camera view to display. Left/Right use a single panel; Both uses left and right eye panels.
        /// </summary>
        public enum DisplayMode
        {
            Left,
            Right,
            Both
        }

        [SerializeField]
        [Tooltip("Current view: Left (single CpuImage), Right, or Both (stereo left + right panels).")]
        DisplayMode m_DisplayMode = DisplayMode.Left;

        [SerializeField]
        [Tooltip("Single-image panel. Used for Left or Right view (one eye at a time). Disabled in Both (stereo) mode.")]
        RawImage m_RawImage;

        [SerializeField]
        [Tooltip("Left eye panel. Used only in Both (stereo) mode. Disabled for Left or Right view.")]
        RawImage m_LeftEyeRawImage;

        [SerializeField]
        [Tooltip("Right eye panel. Used only in Both (stereo) mode. Disabled for Left or Right view.")]
        RawImage m_RightEyeRawImage;

        [SerializeField]
        [Tooltip("Optional dropdown to switch view (Left, Right, Both). Assign and add 3 options in the inspector.")]
        TMP_Dropdown m_ViewDropdown;

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
        Texture2D m_LeftEyeTexture;
        Texture2D m_RightEyeTexture;
        Vector2Int m_LastKnownImageDimensions;

        bool m_IsCameraInitialized;
        Vector2Int m_ConversionDimensions;
        XRCpuImage.Transformation m_ConversionTransform = XRCpuImage.Transformation.None;

        void Awake()
        {
            Assert.IsTrue(m_RawImage != null, "RawImage is not assigned in the Inspector.");
            Assert.IsTrue(m_LeftEyeRawImage != null, "LeftEyeRawImage is not assigned in the Inspector. Required for stereo (Both) display mode.");
            Assert.IsTrue(m_RightEyeRawImage != null, "RightEyeRawImage is not assigned in the Inspector. Required for stereo (Both) display mode.");
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
            if (m_ViewDropdown != null)
            {
                m_ViewDropdown.onValueChanged.AddListener(OnViewDropdownChanged);
                m_ViewDropdown.SetValueWithoutNotify((int)m_DisplayMode);
            }
        }

        void OnDisable()
        {
            ARSession.stateChanged -= OnSessionStateChanged;
            if (m_ViewDropdown != null)
                m_ViewDropdown.onValueChanged.RemoveListener(OnViewDropdownChanged);
            ReleaseResources();
        }

        void OnDestroy()
        {
            ReleaseResources();
        }

        void ReleaseResources()
        {
            m_IsCameraInitialized = false;

            if (m_CameraTexture != null)
            {
                Destroy(m_CameraTexture);
                m_CameraTexture = null;
            }

            if (m_LeftEyeTexture != null)
            {
                Destroy(m_LeftEyeTexture);
                m_LeftEyeTexture = null;
            }

            if (m_RightEyeTexture != null)
            {
                Destroy(m_RightEyeTexture);
                m_RightEyeTexture = null;
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
                return;

            UpdateViewVisibility();

            var metaSubsystem = m_CameraManager.subsystem as MetaOpenXRCameraSubsystem;

            switch (m_DisplayMode)
            {
                case DisplayMode.Left:
                    UpdateSingleLeftView(metaSubsystem);
                    break;
                case DisplayMode.Right:
                    UpdateRightView(metaSubsystem);
                    break;
                case DisplayMode.Both:
                    UpdateStereoView(metaSubsystem);
                    break;
            }
        }

        void UpdateViewVisibility()
        {
            bool singlePanel = (m_DisplayMode == DisplayMode.Left || m_DisplayMode == DisplayMode.Right);
            bool both = (m_DisplayMode == DisplayMode.Both);

            if (m_RawImage != null)
                m_RawImage.enabled = singlePanel;
            if (m_LeftEyeRawImage != null)
                m_LeftEyeRawImage.enabled = both;
            if (m_RightEyeRawImage != null)
                m_RightEyeRawImage.enabled = both;
        }

        void UpdateSingleLeftView(MetaOpenXRCameraSubsystem metaSubsystem)
        {
            if (m_RawImage == null)
                return;
            if (metaSubsystem != null && metaSubsystem.TryAcquireLatestCpuImageForPosition(MetaOpenXRCameraSubsystem.CameraPosition.LeftEye, out XRCpuImage.Cinfo leftCinfo))
            {
                using (var leftImage = new XRCpuImage(metaSubsystem.cpuImageApi, leftCinfo))
                {
                    m_LastKnownImageDimensions = leftImage.dimensions;
                    UpdateTexture(leftImage, ref m_CameraTexture, m_RawImage);
                    UpdateDataFieldsForCamera(metaSubsystem, MetaOpenXRCameraSubsystem.CameraPosition.LeftEye, leftImage);
                    UpdateImageResolutionDisplay(false);
                }
                return;
            }

            if (m_CameraManager.TryAcquireLatestCpuImage(out XRCpuImage cpuImage))
            {
                m_LastKnownImageDimensions = cpuImage.dimensions;
                UpdateTexture(cpuImage, ref m_CameraTexture, m_RawImage);
                UpdateDataFields(cpuImage);
                UpdateImageResolutionDisplay(false);
                cpuImage.Dispose();
            }
        }

        void UpdateRightView(MetaOpenXRCameraSubsystem metaSubsystem)
        {
            if (m_RawImage == null)
                return;
            if (metaSubsystem == null || !metaSubsystem.TryAcquireLatestCpuImageForPosition(MetaOpenXRCameraSubsystem.CameraPosition.RightEye, out XRCpuImage.Cinfo rightCinfo))
                return;
            using (var rightImage = new XRCpuImage(metaSubsystem.cpuImageApi, rightCinfo))
            {
                m_LastKnownImageDimensions = rightImage.dimensions;
                UpdateTexture(rightImage, ref m_CameraTexture, m_RawImage);
                UpdateDataFieldsForCamera(metaSubsystem, MetaOpenXRCameraSubsystem.CameraPosition.RightEye, rightImage);
                UpdateImageResolutionDisplay(false);
            }
        }

        void UpdateStereoView(MetaOpenXRCameraSubsystem metaSubsystem)
        {
            if (metaSubsystem == null)
                return;
            if (m_LeftEyeRawImage == null || m_RightEyeRawImage == null)
            {
                Debug.LogError("MetaCpuImage: LeftEyeRawImage and RightEyeRawImage must be assigned in the Inspector for stereo (Both) display mode.");
                return;
            }

            // Prefer synchronized stereo pair (same capture request)
            if (metaSubsystem.TryAcquireLatestStereoCpuImagePair(out MetaOpenXRCameraSubsystem.XRCpuImagePair pair))
            {
                using (var leftImage = new XRCpuImage(metaSubsystem.cpuImageApi, pair.leftEyeImageCinfo))
                using (var rightImage = new XRCpuImage(metaSubsystem.cpuImageApi, pair.rightEyeImageCinfo))
                {
                    m_LastKnownImageDimensions = leftImage.dimensions;
                    UpdateTexture(leftImage, ref m_LeftEyeTexture, m_LeftEyeRawImage);
                    UpdateTexture(rightImage, ref m_RightEyeTexture, m_RightEyeRawImage);
                    UpdateDataFieldsForCamera(metaSubsystem, MetaOpenXRCameraSubsystem.CameraPosition.LeftEye, leftImage);
                }
                UpdateImageResolutionDisplay(true);
                return;
            }

            // Fallback: acquire left and right independently
            bool gotLeft = metaSubsystem.TryAcquireLatestCpuImageForPosition(MetaOpenXRCameraSubsystem.CameraPosition.LeftEye, out XRCpuImage.Cinfo leftCinfo);
            bool gotRight = metaSubsystem.TryAcquireLatestCpuImageForPosition(MetaOpenXRCameraSubsystem.CameraPosition.RightEye, out XRCpuImage.Cinfo rightCinfo);

            if (gotLeft)
            {
                using (var leftImage = new XRCpuImage(metaSubsystem.cpuImageApi, leftCinfo))
                {
                    m_LastKnownImageDimensions = leftImage.dimensions;
                    UpdateTexture(leftImage, ref m_LeftEyeTexture, m_LeftEyeRawImage);
                    UpdateDataFieldsForCamera(metaSubsystem, MetaOpenXRCameraSubsystem.CameraPosition.LeftEye, leftImage);
                }
            }

            if (gotRight)
            {
                using (var rightImage = new XRCpuImage(metaSubsystem.cpuImageApi, rightCinfo))
                    UpdateTexture(rightImage, ref m_RightEyeTexture, m_RightEyeRawImage);
            }

            UpdateImageResolutionDisplay(true);
        }

        void OnViewDropdownChanged(int index)
        {
            if (index >= 0 && index <= 2)
                m_DisplayMode = (DisplayMode)index;
        }

        /// <summary>
        /// Call from a UI dropdown to switch view. 0 = Left, 1 = Right, 2 = Both.
        /// </summary>
        public void SetViewMode(int index)
        {
            if (index >= 0 && index <= 2)
            {
                m_DisplayMode = (DisplayMode)index;
                if (m_ViewDropdown != null)
                    m_ViewDropdown.SetValueWithoutNotify(index);
            }
        }

        unsafe void UpdateTexture(XRCpuImage cpuImage, ref Texture2D texture, RawImage rawImage)
        {
            if (rawImage == null)
                return;

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

            var hasValidTexture = texture != null;

            if (hasValidTexture && (texture.width != conversionParams.outputDimensions.x ||
                                    texture.height != conversionParams.outputDimensions.y))
            {
                Destroy(texture);
                texture = null;
                hasValidTexture = false;
            }

            if (texture == null)
            {
                texture = new Texture2D(conversionParams.outputDimensions.x, conversionParams.outputDimensions.y,
                    conversionParams.outputFormat, false);
            }

            var rawTextureData = texture.GetRawTextureData<byte>();
            cpuImage.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);

            texture.Apply();
            rawImage.texture = texture;
            // NOTE: this scale is done because the "default" orientation of the CPU image coming from the Meta
            // Quest cameras is upside-down.
            rawImage.rectTransform.localScale = new Vector3(1, -1, 1);
            rawImage.enabled = true;
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

            if (success && m_ErrorText != null && m_ErrorText.IsActive())
                m_ErrorText.gameObject.SetActive(false);

            int fieldIndex = 0;
            foreach (var field in m_DataFieldElements)
            {
                if (field != null && fieldIndex < m_ValueSetFunctions.Count && m_ValueSetFunctions[fieldIndex] != null)
                    m_ValueSetFunctions[fieldIndex].Invoke(field, cpuImage, intrinsics);
                fieldIndex++;
            }
        }

        void UpdateDataFieldsForCamera(MetaOpenXRCameraSubsystem metaSubsystem, MetaOpenXRCameraSubsystem.CameraPosition position, XRCpuImage cpuImage)
        {
            var success = metaSubsystem.TryGetIntrinsicsForPosition(position, out var intrinsics);

            if (success && m_ErrorText != null && m_ErrorText.IsActive())
                m_ErrorText.gameObject.SetActive(false);

            int fieldIndex = 0;
            foreach (var field in m_DataFieldElements)
            {
                if (field != null && fieldIndex < m_ValueSetFunctions.Count && m_ValueSetFunctions[fieldIndex] != null)
                    m_ValueSetFunctions[fieldIndex].Invoke(field, cpuImage, intrinsics);
                fieldIndex++;
            }
        }

        void UpdateImageResolutionDisplay(bool stereo)
        {
            if (m_ImageResolutionValue == null)
                return;

            if (stereo)
                m_ImageResolutionValue.text = $"L/R: {m_ConversionDimensions.x} x {m_ConversionDimensions.y}";
            else
                m_ImageResolutionValue.text = $"{m_ConversionDimensions.x} x  {m_ConversionDimensions.y}";
        }
    }
}
#endif
