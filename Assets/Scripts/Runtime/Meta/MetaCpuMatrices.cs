#if METAOPENXR_2_4_0_PRE_1_OR_NEWER
using System;
using TMPro;
using UnityEngine.Assertions;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections.LowLevel.Unsafe;
using Unity.XR.CoreUtils;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MetaCpuMatrices : MonoBehaviour
    {
        [Header("AR Foundation")]
        [SerializeField] 
        ARCameraManager m_CameraManager;

        [Header("Scene Objects")]
        [SerializeField] 
        Camera m_MainCamera;
        [SerializeField] 
        RawImage m_RawImage;
        [SerializeField] 
        Shader m_MetaCpuShader;

        [Header("UI Displays")] 
        [SerializeField] 
        TextMeshProUGUI m_ProjectionMatrixText;
        [SerializeField] 
        TextMeshProUGUI m_DisplayMatrixText;

        [Header("UI Controls")]
        [SerializeField] 
        Button m_ProjectionMatrixToggleButton;
        [SerializeField] 
        Button m_DisplayMatrixToggleButton;

        [SerializeField]
        TextMeshProUGUI m_ErrorText;

        TextMeshProUGUI m_ProjectionButtonText;
        TextMeshProUGUI m_DisplayButtonText;

        const string k_ShaderDisplayTransform = "_UnityDisplayTransform";
        const string k_ShaderMainTex = "_MainTex";

        Material m_MetaCpuMaterial;
        Texture2D m_CameraTexture;
        bool m_IsCameraInitialized;
        bool m_UseNativeDisplayMatrix;
        bool m_UseNativeProjectionMatrix;

        void Awake()
        {
            Assert.IsNotNull(m_CameraManager, "AR Camera Manager is not assigned.");
            Assert.IsNotNull(m_MainCamera, "Main Camera is not assigned.");
            Assert.IsNotNull(m_RawImage, "Background Raw Image is not assigned in the Inspector.");
            Assert.IsNotNull(m_MetaCpuShader, "Shader`Unlit/MetaCpuBackgroundShader`is not assigned in the Inspector.");

            m_MetaCpuMaterial = new Material(m_MetaCpuShader);
            m_RawImage.material = m_MetaCpuMaterial;

            m_UseNativeDisplayMatrix = true;
            m_UseNativeProjectionMatrix = true;
            
            if(m_ProjectionMatrixToggleButton != null)
                m_ProjectionButtonText = m_ProjectionMatrixToggleButton.GetComponentInChildren<TextMeshProUGUI>();
            
            if(m_DisplayMatrixToggleButton != null)
                m_DisplayButtonText = m_DisplayMatrixToggleButton.GetComponentInChildren<TextMeshProUGUI>();
        }

        void Start()
        {
            UpdateToggleButtonText();
        }
        
        void OnEnable()
        {
            m_CameraManager.frameReceived += OnCameraFrameReceived;
            ARSession.stateChanged += OnSessionStateChanged;
        }

        void OnDisable()
        {
            m_CameraManager.frameReceived -= OnCameraFrameReceived;
            ARSession.stateChanged -= OnSessionStateChanged;
            if (m_CameraTexture != null) Destroy(m_CameraTexture);
        }
        
        void OnCameraFrameReceived(ARCameraFrameEventArgs args)
        {
            if (!m_IsCameraInitialized || m_MetaCpuMaterial == null) return;
            
            if (m_CameraManager.TryAcquireLatestCpuImage(out var cpuImage))
            {
                UpdateCameraTexture(cpuImage);
                cpuImage.Dispose();

                if (m_ErrorText.IsActive())
                    m_ErrorText.gameObject.SetActive(false);
            }

            Matrix4x4 finalDisplayMatrix;

            if (m_UseNativeProjectionMatrix && args.projectionMatrix.HasValue)
            {
                m_MainCamera.projectionMatrix = args.projectionMatrix.Value;
            }
            else
            {
                m_MainCamera.ResetProjectionMatrix();
            }
            var finalProjectionMatrix = m_MainCamera.projectionMatrix;

            if (m_UseNativeDisplayMatrix && args.displayMatrix.HasValue)
            {
                finalDisplayMatrix = args.displayMatrix.Value;
            }
            else
            {
                finalDisplayMatrix = Matrix4x4.identity;
            }
            m_MetaCpuMaterial.SetMatrix(k_ShaderDisplayTransform, finalDisplayMatrix);

            UpdateMatrixText(finalProjectionMatrix, finalDisplayMatrix);
        }
        
        unsafe void UpdateCameraTexture(XRCpuImage cpuImage)
        {
            var conversionParams = new XRCpuImage.ConversionParams
            {
                inputRect = new RectInt(0, 0, cpuImage.width, cpuImage.height),
                outputDimensions = new Vector2Int(cpuImage.width, cpuImage.height),
                outputFormat = TextureFormat.RGBA32,
                transformation = XRCpuImage.Transformation.None
            };
            
            if (m_CameraTexture == null || m_CameraTexture.width != cpuImage.width || m_CameraTexture.height != cpuImage.height)
            {
                m_CameraTexture = new Texture2D(conversionParams.outputDimensions.x, conversionParams.outputDimensions.y, conversionParams.outputFormat, false);
            }

            var rawTextureData = m_CameraTexture.GetRawTextureData<byte>();
            cpuImage.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
            m_CameraTexture.Apply();

            m_RawImage.texture = m_CameraTexture;
            m_MetaCpuMaterial.SetTexture(k_ShaderMainTex, m_CameraTexture);

            if (!m_RawImage.enabled)
                m_RawImage.enabled = true;
        }

        void OnSessionStateChanged(ARSessionStateChangedEventArgs args)
        {
            m_IsCameraInitialized = (args.state >= ARSessionState.SessionTracking);
        }

        public void ToggleProjectionMatrix()
        {
            m_UseNativeProjectionMatrix = !m_UseNativeProjectionMatrix;
            UpdateToggleButtonText();
        }

        public void ToggleDisplayMatrix()
        {
            m_UseNativeDisplayMatrix = !m_UseNativeDisplayMatrix;
            UpdateToggleButtonText();
        }

        void UpdateToggleButtonText()
        {
            if (m_ProjectionButtonText != null)
            {
                m_ProjectionButtonText.text = m_UseNativeProjectionMatrix ? "Projection: NATIVE" : "Projection: DEFAULT";
            }
            if (m_DisplayButtonText != null)
            {
                m_DisplayButtonText.text = m_UseNativeDisplayMatrix ? "Display: NATIVE" : "Display: DEFAULT";
            }
        }
        
        void UpdateMatrixText(Matrix4x4 projectionMatrix, Matrix4x4 displayMatrix)
        {
            m_ProjectionMatrixText.text = $"Projection Matrix:\n{projectionMatrix.ToString("F3")}";
            m_DisplayMatrixText.text = $"Display Matrix:\n{displayMatrix.ToString("F3")}";
        }

        void OnDestroy()
        {
            if (m_MetaCpuMaterial != null)
            {
                UnityObjectUtils.Destroy(m_MetaCpuMaterial);
            }
        }

#if UNITY_EDITOR
        void Reset()
        {
            m_CameraManager = FindAnyObjectByType<ARCameraManager>();
            
            if (m_CameraManager != null)
            {
                m_MainCamera = m_CameraManager.GetComponent<Camera>();
            }
            var cpuImageObject = GameObject.Find("CpuImage");
            if (cpuImageObject != null)
            {
                m_RawImage = cpuImageObject.GetComponent<RawImage>();
            }

            m_RawImage = GetComponentInChildren<RawImage>();
            m_MetaCpuShader = Shader.Find("Unlit/MetaCpuShader");

            var projTextObject = GameObject.Find("ProjectionMatrixText");
            if (projTextObject != null)
                m_ProjectionMatrixText = projTextObject.GetComponent<TextMeshProUGUI>();

            var dispTextObject = GameObject.Find("DisplayMatrixText");
            if (dispTextObject != null)
                m_DisplayMatrixText = dispTextObject.GetComponent<TextMeshProUGUI>();

            var projButtonObject = GameObject.Find("ProjectionToggleButton");
            if (projButtonObject != null)
                m_ProjectionMatrixToggleButton = projButtonObject.GetComponent<Button>();

            var dispButtonObject = GameObject.Find("DisplayToggleButton");
            if (dispButtonObject != null)
                m_DisplayMatrixToggleButton = dispButtonObject.GetComponent<Button>();
        }
#endif
    }
}
#endif // METAOPENXR_2_4_0_PRE_1_OR_NEWER
