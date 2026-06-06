#if METAOPENXR_2_6_0_1_OR_NEWER && (UNITY_EDITOR || UNITY_ANDROID)
using System;
using TMPro;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.OpenXR.Features.Meta;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MetaGpuImage : MonoBehaviour
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

        [Header("View")]
        [SerializeField]
        [Tooltip("Current view: Left, Right, or Both (stereo left + right panels).")]
        DisplayMode m_DisplayMode = DisplayMode.Left;

        [SerializeField]
        [Tooltip("Single-image panel. Used for Left or Right view. Disabled in Both (stereo) mode.")]
        RawImage m_RawImage;

        [SerializeField]
        [Tooltip("Left eye panel. Used only in Both (stereo) mode.")]
        RawImage m_LeftEyeRawImage;

        [SerializeField]
        [Tooltip("Right eye panel. Used only in Both (stereo) mode.")]
        RawImage m_RightEyeRawImage;

        [SerializeField]
        [Tooltip("Optional dropdown to switch view (Left, Right, Both). Assign and add 3 options in the inspector.")]
        TMP_Dropdown m_ViewDropdown;

        [Header("Shader")]
        [SerializeField]
        [Tooltip("Optional shader to apply when 'Apply Extra Shader' toggle is on.")]
        Shader m_ExtraShader;

        [SerializeField]
        [Tooltip("When on, the displayed texture(s) use m_ExtraShader; when off, use default Unlit texture.")]
        Toggle m_ApplyExtraShaderToggle;

        [Header("UI")]
        [SerializeField]
        [Tooltip("Displays the current image resolution.")]
        TextMeshProUGUI m_ImageResolutionValue;

        [SerializeField]
        [Tooltip("Displays error messages.")]
        TextMeshProUGUI m_ErrorText;

        [Header("AR")]
        [SerializeField]
        [Tooltip("The AR Camera Manager (used for subsystem access).")]
        ARCameraManager m_CameraManager;

        // Single-eye: one descriptor and one texture
        XRTextureDescriptor m_CurrentTextureDescriptor;
        Texture2D m_SingleTexture;
        bool m_AcquiredSingle;

        // Stereo: pair of descriptors and textures
        MetaOpenXRCameraSubsystem.XRTextureDescriptorPair m_CurrentStereoPair;
        Texture2D m_LeftTexture;
        Texture2D m_RightTexture;
        bool m_AcquiredStereo;

        // Track which camera acquired so we release the same one (display mode can change before release)
        MetaOpenXRCameraSubsystem.CameraPosition m_AcquiredSingleCameraPosition;
        Camera m_AcquiredFromCamera;

        Material m_DefaultMaterial;
        Material m_DefaultMaterialRight; // stereo right panel (separate instance so it can show different texture)
        Material m_ExtraMaterial;
        Material m_ExtraMaterialRight;

        int m_ConsecutiveAcquisitionFailures;
        const int k_AcquisitionFailureThreshold = 5;

        int m_LastAcquireFrame = -1;

        static readonly int k_MainTexId = Shader.PropertyToID("_MainTex");

        static readonly Rect k_DisplayUvRect = new Rect(0, 1, 1, -1);

        void Awake()
        {
            Assert.IsTrue(m_RawImage != null, "RawImage is not assigned.");
            CreateMaterials();
        }

        void OnEnable()
        {
            if (m_DefaultMaterial == null)
                CreateMaterials();
            m_ConsecutiveAcquisitionFailures = 0;
            RenderPipelineManager.beginCameraRendering += HandleBeforeRender;
            RenderPipelineManager.endCameraRendering += HandleAfterRender;
            if (m_ViewDropdown != null)
            {
                m_ViewDropdown.onValueChanged.AddListener(OnViewDropdownChanged);
                m_ViewDropdown.SetValueWithoutNotify((int)m_DisplayMode);
            }
            if (m_ApplyExtraShaderToggle != null)
                m_ApplyExtraShaderToggle.onValueChanged.AddListener(OnApplyExtraShaderToggleChanged);
        }

        void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= HandleBeforeRender;
            RenderPipelineManager.endCameraRendering -= HandleAfterRender;
            if (m_ViewDropdown != null)
                m_ViewDropdown.onValueChanged.RemoveListener(OnViewDropdownChanged);
            if (m_ApplyExtraShaderToggle != null)
                m_ApplyExtraShaderToggle.onValueChanged.RemoveListener(OnApplyExtraShaderToggleChanged);
            var metaSubsystem = m_CameraManager?.subsystem as MetaOpenXRCameraSubsystem;
            if (m_AcquiredStereo)
            {
                metaSubsystem?.ReleaseStereoGpuImagePair(m_CurrentStereoPair);
                m_AcquiredStereo = false;
            }
            else if (m_AcquiredSingle)
            {
                metaSubsystem?.ReleaseGpuImageForPosition(m_AcquiredSingleCameraPosition, m_CurrentTextureDescriptor);
                m_AcquiredSingle = false;
            }
            m_AcquiredFromCamera = null;
            if (m_RawImage != null)
            {
                m_RawImage.material = null;
                m_RawImage.texture = null;
            }
            if (m_LeftEyeRawImage != null)
            {
                m_LeftEyeRawImage.material = null;
                m_LeftEyeRawImage.texture = null;
            }
            if (m_RightEyeRawImage != null)
            {
                m_RightEyeRawImage.material = null;
                m_RightEyeRawImage.texture = null;
            }
            DestroyTextures();
            DestroyMaterials();
        }

        void OnViewDropdownChanged(int index)
        {
            if (index >= 0 && index <= 2)
                m_DisplayMode = (DisplayMode)index;
        }

        void OnApplyExtraShaderToggleChanged(bool on)
        {
            ApplyMaterialToActiveViews();
        }

        void UpdateViewVisibility()
        {
            bool singlePanel = m_DisplayMode == DisplayMode.Left || m_DisplayMode == DisplayMode.Right;
            bool both = m_DisplayMode == DisplayMode.Both;
            if (m_RawImage != null)
                m_RawImage.enabled = singlePanel;
            if (m_LeftEyeRawImage != null)
                m_LeftEyeRawImage.enabled = both;
            if (m_RightEyeRawImage != null)
                m_RightEyeRawImage.enabled = both;
        }

        void HandleBeforeRender(ScriptableRenderContext context, Camera cam)
        {
            if (cam.cameraType != CameraType.Game)
                return;

            var metaSubsystem = m_CameraManager?.subsystem as MetaOpenXRCameraSubsystem;
            if (metaSubsystem == null)
                return;

            if (Time.frameCount == m_LastAcquireFrame)
                return;
            m_LastAcquireFrame = Time.frameCount;

            UpdateViewVisibility();

            if (m_DisplayMode == DisplayMode.Both)
            {
                if (metaSubsystem.TryAcquireLatestStereoGpuImagePair(out m_CurrentStereoPair))
                {
                    m_AcquiredFromCamera = cam;
                    m_ConsecutiveAcquisitionFailures = 0;
                    ClearAcquisitionError();
                    m_AcquiredStereo = true;
                    UpdateExternalTextureFromDescriptor(m_CurrentStereoPair.leftEyeDescriptor, ref m_LeftTexture);
                    UpdateExternalTextureFromDescriptor(m_CurrentStereoPair.rightEyeDescriptor, ref m_RightTexture);
                    UpdateStereoMaterialsAndUI();
                }
                else
                    ReportAcquisitionFailureIfRepeated();
            }
            else
            {
                m_AcquiredSingleCameraPosition = m_DisplayMode == DisplayMode.Left
                    ? MetaOpenXRCameraSubsystem.CameraPosition.LeftEye
                    : MetaOpenXRCameraSubsystem.CameraPosition.RightEye;
                if (metaSubsystem.TryAcquireLatestGpuImageForPosition(m_AcquiredSingleCameraPosition, out m_CurrentTextureDescriptor))
                {
                    m_AcquiredFromCamera = cam;
                    m_ConsecutiveAcquisitionFailures = 0;
                    ClearAcquisitionError();
                    m_AcquiredSingle = true;
                    UpdateExternalTextureFromDescriptor(m_CurrentTextureDescriptor, ref m_SingleTexture);
                    UpdateSingleMaterialsAndUI();
                }
                else
                    ReportAcquisitionFailureIfRepeated();
            }
        }

        void ClearAcquisitionError()
        {
            if (m_ErrorText != null)
            {
                m_ErrorText.text = string.Empty;
                if (m_ErrorText.gameObject.activeSelf)
                    m_ErrorText.gameObject.SetActive(false);
            }
        }

        void ReportAcquisitionFailureIfRepeated()
        {
            m_ConsecutiveAcquisitionFailures++;
            if (m_ErrorText == null || m_ConsecutiveAcquisitionFailures < k_AcquisitionFailureThreshold)
                return;
            m_ErrorText.text = "Camera image unavailable. Acquisition failed repeatedly.";
            if (!m_ErrorText.gameObject.activeSelf)
                m_ErrorText.gameObject.SetActive(true);
        }

        void HandleAfterRender(ScriptableRenderContext context, Camera cam)
        {
            var metaSubsystem = m_CameraManager?.subsystem as MetaOpenXRCameraSubsystem;
            if (metaSubsystem == null)
                return;

            if (m_AcquiredFromCamera != cam)
                return;

            if (m_AcquiredStereo)
            {
                metaSubsystem.ReleaseStereoGpuImagePair(m_CurrentStereoPair);
                m_AcquiredStereo = false;
            }
            else if (m_AcquiredSingle)
            {
                metaSubsystem.ReleaseGpuImageForPosition(m_AcquiredSingleCameraPosition, m_CurrentTextureDescriptor);
                m_AcquiredSingle = false;
            }

            m_AcquiredFromCamera = null;
        }

        void UpdateExternalTextureFromDescriptor(XRTextureDescriptor desc, ref Texture2D texture)
        {
            if (desc.nativeTexture == IntPtr.Zero)
            {
                if (texture != null)
                {
                    Destroy(texture);
                    texture = null;
                }
                return;
            }

            bool needsRecreate =
                texture == null ||
                texture.width != desc.width ||
                texture.height != desc.height ||
                texture.format != desc.format;

            if (needsRecreate)
            {
                if (texture != null)
                    Destroy(texture);
                texture = Texture2D.CreateExternalTexture(
                    desc.width,
                    desc.height,
                    desc.format,
                    false,
                    false,
                    desc.nativeTexture
                );
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.filterMode = FilterMode.Bilinear;
                if (m_ErrorText != null && m_ErrorText.IsActive())
                    m_ErrorText.gameObject.SetActive(false);
            }
            else
            {
                texture.UpdateExternalTexture(desc.nativeTexture);
            }
        }

        void CreateMaterials()
        {
            var shader = Shader.Find("Unlit/Texture");
            if (shader == null)
                shader = Shader.Find("UI/Default");
            if (shader != null)
            {
                m_DefaultMaterial = new Material(shader);
                m_DefaultMaterialRight = new Material(shader);
            }
            else
            {
                Debug.LogError("[MetaGpuImage] Default shader could not be found.");
            }

            if (m_ExtraShader != null)
            {
                m_ExtraMaterial = new Material(m_ExtraShader);
                m_ExtraMaterialRight = new Material(m_ExtraShader);
            }
        }

        bool UseExtraShader()
        {
            return m_ApplyExtraShaderToggle != null && m_ApplyExtraShaderToggle.isOn && m_ExtraMaterial != null;
        }

        Material GetActiveMaterial()
        {
            return UseExtraShader() ? m_ExtraMaterial : m_DefaultMaterial;
        }

        void ApplyMaterialToActiveViews()
        {
            int propId = k_MainTexId;
            if (m_DisplayMode == DisplayMode.Both)
            {
                Material leftMat = GetActiveMaterialForStereoLeft();
                Material rightMat = GetActiveMaterialForStereoRight();
                if (leftMat != null && m_LeftTexture != null && m_LeftEyeRawImage != null)
                {
                    leftMat.SetTexture(propId, m_LeftTexture);
                    m_LeftEyeRawImage.material = leftMat;
                    m_LeftEyeRawImage.uvRect = k_DisplayUvRect;
                }
                if (rightMat != null && m_RightTexture != null && m_RightEyeRawImage != null)
                {
                    rightMat.SetTexture(propId, m_RightTexture);
                    m_RightEyeRawImage.material = rightMat;
                    m_RightEyeRawImage.uvRect = k_DisplayUvRect;
                }
            }
            else if (m_SingleTexture != null && m_RawImage != null)
            {
                var mat = GetActiveMaterial();
                if (mat != null)
                {
                    mat.SetTexture(propId, m_SingleTexture);
                    m_RawImage.material = mat;
                    m_RawImage.uvRect = k_DisplayUvRect;
                }
            }
        }

        Material GetActiveMaterialForStereoLeft()
        {
            return UseExtraShader() ? m_ExtraMaterial : m_DefaultMaterial;
        }

        Material GetActiveMaterialForStereoRight()
        {
            return UseExtraShader() ? m_ExtraMaterialRight : m_DefaultMaterialRight;
        }

        void UpdateSingleMaterialsAndUI()
        {
            if (m_DefaultMaterial == null || m_SingleTexture == null)
                return;

            m_DefaultMaterial.SetTexture(k_MainTexId, m_SingleTexture);
            if (m_ExtraMaterial != null)
            {
                m_ExtraMaterial.SetTexture(k_MainTexId, m_SingleTexture);
                m_ExtraMaterial.SetVector("_TexelSize", new Vector4(1f / m_CurrentTextureDescriptor.width, 1f / m_CurrentTextureDescriptor.height, 0f, 0f));
            }

            if (m_RawImage != null)
            {
                var mat = GetActiveMaterial();
                if (mat != null)
                {
                    m_RawImage.material = mat;
                    m_RawImage.uvRect = k_DisplayUvRect;
                }
            }

            if (m_ImageResolutionValue != null)
                m_ImageResolutionValue.text = $"{m_CurrentTextureDescriptor.width} x {m_CurrentTextureDescriptor.height}";
        }

        void UpdateStereoMaterialsAndUI()
        {
            if (m_DefaultMaterial == null || m_DefaultMaterialRight == null)
                return;

            if (m_LeftTexture != null)
            {
                m_DefaultMaterial.SetTexture(k_MainTexId, m_LeftTexture);
                if (m_ExtraMaterial != null)
                {
                    m_ExtraMaterial.SetTexture(k_MainTexId, m_LeftTexture);
                    m_ExtraMaterial.SetVector("_TexelSize", new Vector4(1f / m_CurrentStereoPair.leftEyeDescriptor.width, 1f / m_CurrentStereoPair.leftEyeDescriptor.height, 0f, 0f));
                }
                if (m_LeftEyeRawImage != null)
                {
                    var leftMat = GetActiveMaterialForStereoLeft();
                    if (leftMat != null)
                    {
                        m_LeftEyeRawImage.material = leftMat;
                        m_LeftEyeRawImage.uvRect = k_DisplayUvRect;
                    }
                }
            }

            if (m_RightTexture != null)
            {
                m_DefaultMaterialRight.SetTexture(k_MainTexId, m_RightTexture);
                if (m_ExtraMaterialRight != null)
                {
                    m_ExtraMaterialRight.SetTexture(k_MainTexId, m_RightTexture);
                    m_ExtraMaterialRight.SetVector("_TexelSize", new Vector4(1f / m_CurrentStereoPair.rightEyeDescriptor.width, 1f / m_CurrentStereoPair.rightEyeDescriptor.height, 0f, 0f));
                }
                if (m_RightEyeRawImage != null)
                {
                    var rightMat = GetActiveMaterialForStereoRight();
                    if (rightMat != null)
                    {
                        m_RightEyeRawImage.material = rightMat;
                        m_RightEyeRawImage.uvRect = k_DisplayUvRect;
                    }
                }
            }

            if (m_ImageResolutionValue != null && m_LeftTexture != null)
                m_ImageResolutionValue.text = $"L/R: {m_CurrentStereoPair.leftEyeDescriptor.width} x {m_CurrentStereoPair.leftEyeDescriptor.height}";
        }

        void DestroyTextures()
        {
            if (m_SingleTexture != null)
            {
                Destroy(m_SingleTexture);
                m_SingleTexture = null;
            }
            if (m_LeftTexture != null)
            {
                Destroy(m_LeftTexture);
                m_LeftTexture = null;
            }
            if (m_RightTexture != null)
            {
                Destroy(m_RightTexture);
                m_RightTexture = null;
            }
        }

        void DestroyMaterials()
        {
            if (m_DefaultMaterial != null)
            {
                Destroy(m_DefaultMaterial);
                m_DefaultMaterial = null;
            }
            if (m_DefaultMaterialRight != null)
            {
                Destroy(m_DefaultMaterialRight);
                m_DefaultMaterialRight = null;
            }
            if (m_ExtraMaterial != null)
            {
                Destroy(m_ExtraMaterial);
                m_ExtraMaterial = null;
            }
            if (m_ExtraMaterialRight != null)
            {
                Destroy(m_ExtraMaterialRight);
                m_ExtraMaterialRight = null;
            }
        }

        /// <summary>
        /// Call from UI to set view mode. 0 = Left, 1 = Right, 2 = Both.
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
    }
}
#endif
