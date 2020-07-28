using System.Text;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// This component displays a picture-in-picture view of the environment depth texture, the human depth texture, or
    /// the human stencil texture.
    /// </summary>
    public class DisplayDepthImage : MonoBehaviour
    {
        /// <summary>
        /// The display mode for the texture widget. Values must match the UI dropdown.
        /// </summary>
        enum DisplayMode
        {
            EnvironmentDepth = 0,
            HumanDepth = 1,
            HumanStencil = 2,
        }

        /// <summary>
        /// Name of the texture rotation property in the shader.
        /// </summary>
        const string k_TextureRotationName = "_TextureRotation";

        /// <summary>
        /// Name of the max distance property in the shader.
        /// </summary>
        const string k_MaxDistanceName = "_MaxDistance";

        /// <summary>
        /// The default texture aspect ratio.
        /// </summary>
        const float k_DefaultTextureAspectRadio = 1.0f;

        /// <summary>
        /// ID of the texture rotation property in the shader.
        /// </summary>
        static readonly int k_TextureRotationId = Shader.PropertyToID(k_TextureRotationName);

        /// <summary>
        /// ID of the max distance  property in the shader.
        /// </summary>
        static readonly int k_MaxDistanceId = Shader.PropertyToID(k_MaxDistanceName);

        /// <summary>
        /// A string builder for construction of strings.
        /// </summary>
        readonly StringBuilder m_StringBuilder = new StringBuilder();

        /// <summary>
        /// The current screen orientation remembered so that we are only updating the raw image layout when it changes.
        /// </summary>
        ScreenOrientation m_CurrentScreenOrientation;

        /// <summary>
        /// The current texture aspect ratio remembered so that we can resize the raw image layout when it changes.
        /// </summary>
        float m_TextureAspectRatio = k_DefaultTextureAspectRadio;

        /// <summary>
        /// The mode indicating which texture to display.
        /// </summary>
        DisplayMode m_DisplayMode = DisplayMode.EnvironmentDepth;

        /// <summary>
        /// Get or set the <c>AROcclusionManager</c>.
        /// </summary>
        public AROcclusionManager occlusionManager
        {
            get => m_OcclusionManager;
            set => m_OcclusionManager = value;
        }

        [SerializeField]
        [Tooltip("The AROcclusionManager which will produce frame events.")]
        AROcclusionManager m_OcclusionManager;

        /// <summary>
        /// The UI RawImage used to display the image on screen.
        /// </summary>
        public RawImage rawImage
        {
            get => m_RawImage;
            set => m_RawImage = value;
        }

        [SerializeField]
        RawImage m_RawImage;

        /// <summary>
        /// The UI Text used to display information about the image on screen.
        /// </summary>
        public Text imageInfo
        {
            get => m_ImageInfo;
            set => m_ImageInfo = value;
        }

        [SerializeField]
        Text m_ImageInfo;

        /// <summary>
        /// The depth material for rendering depth textures.
        /// </summary>
        public Material depthMaterial
        {
            get => m_DepthMaterial;
            set => m_DepthMaterial = value;
        }

        [SerializeField]
        Material m_DepthMaterial;

        /// <summary>
        /// The stencil material for rendering stencil textures.
        /// </summary>
        public Material stencilMaterial
        {
            get => m_StencilMaterial;
            set => m_StencilMaterial = value;
        }

        [SerializeField]
        Material m_StencilMaterial;

        /// <summary>
        /// The max distance value for the shader when showing an environment depth texture.
        /// </summary>
        public float maxEnvironmentDistance
        {
            get => m_MaxEnvironmentDistance;
            set => m_MaxEnvironmentDistance = value;
        }

        [SerializeField]
        float m_MaxEnvironmentDistance = 8.0f;

        /// <summary>
        /// The max distance value for the shader when showing an human depth texture.
        /// </summary>
        public float maxHumanDistance
        {
            get => m_MaxHumanDistance;
            set => m_MaxHumanDistance = value;
        }

        [SerializeField]
        float m_MaxHumanDistance = 3.0f;

        void OnEnable()
        {
            // When enabled, get the current screen orientation, and update the raw image UI.
            m_CurrentScreenOrientation = Screen.orientation;
            UpdateRawImage();
        }

        void Update()
        {
            // If we are on a device that does supports neither human stencil, human depth, nor environment depth,
            // display a message about unsupported functionality and return.
            Debug.Assert(m_OcclusionManager != null, "no occlusion manager");
            switch (m_DisplayMode)
            {
                case DisplayMode.HumanDepth:
                case DisplayMode.HumanStencil:
                    if ((m_OcclusionManager.descriptor?.supportsHumanSegmentationStencilImage == false)
                        && (m_OcclusionManager.descriptor?.supportsHumanSegmentationDepthImage == false))
                    {
                        if (m_ImageInfo != null)
                        {
                            m_ImageInfo.text = "Human segmentation is not supported on this device.";
                        }

                        m_RawImage.texture = null;
                        if (!Mathf.Approximately(m_TextureAspectRatio, k_DefaultTextureAspectRadio))
                        {
                            m_TextureAspectRatio = k_DefaultTextureAspectRadio;
                            UpdateRawImage();
                        }

                        return;
                    }
                    break;
                case DisplayMode.EnvironmentDepth :
                default:
                    if (m_OcclusionManager.descriptor?.supportsEnvironmentDepthImage == false)
                    {
                        if (m_ImageInfo != null)
                        {
                            m_ImageInfo.text = "Environment depth is not supported on this device.";
                        }

                        m_RawImage.texture = null;
                        if (!Mathf.Approximately(m_TextureAspectRatio, k_DefaultTextureAspectRadio))
                        {
                            m_TextureAspectRatio = k_DefaultTextureAspectRadio;
                            UpdateRawImage();
                        }

                        return;
                    }
                    break;
            }

            // Get all of the occlusion textures.
            Texture2D humanStencil = m_OcclusionManager.humanStencilTexture;
            Texture2D humanDepth = m_OcclusionManager.humanDepthTexture;
            Texture2D envDepth = m_OcclusionManager.environmentDepthTexture;

            // Display some text information about each of the textures.
            m_StringBuilder.Clear();
            LogTextureInfo(m_StringBuilder, "stencil", humanStencil);
            LogTextureInfo(m_StringBuilder, "depth", humanDepth);
            LogTextureInfo(m_StringBuilder, "env", envDepth);
            if (m_ImageInfo != null)
            {
                m_ImageInfo.text = m_StringBuilder.ToString();
            }
            else
            {
                Debug.Log(m_StringBuilder.ToString());
            }

            // Decide which to display based on the current mode.
            Texture2D displayTexture;
            switch (m_DisplayMode)
            {
                case DisplayMode.HumanStencil:
                    displayTexture = humanStencil;
                    break;
                case DisplayMode.HumanDepth:
                    displayTexture = humanDepth;
                    break;
                case DisplayMode.EnvironmentDepth:
                default:
                    displayTexture = envDepth;
                    break;
            }

            // Assign the texture to display to the raw image.
            Debug.Assert(m_RawImage != null, "no raw image");
            m_RawImage.texture = displayTexture;

            // Get the aspect ratio for the current texture.
            float textureAspectRatio = (displayTexture == null) ? 1.0f : ((float)displayTexture.width / (float)displayTexture.height);

            // If the raw image needs to be updated because of a device orientation change or because of a texture
            // aspect ratio difference, then update the raw image with the new values.
            if ((m_CurrentScreenOrientation != Screen.orientation)
                || !Mathf.Approximately(m_TextureAspectRatio, textureAspectRatio))
            {
                m_CurrentScreenOrientation = Screen.orientation;
                m_TextureAspectRatio = textureAspectRatio;
                UpdateRawImage();
            }
        }

        /// <summary>
        /// Create log information about the given texture.
        /// </summary>
        /// <param name="stringBuilder">The string builder to which to append the texture information.</param>
        /// <param name="textureName">The semantic name of the texture for logging purposes.</param>
        /// <param name="texture">The texture for which to log information.</param>
        void LogTextureInfo(StringBuilder stringBuilder, string textureName, Texture2D texture)
        {
            stringBuilder.Append($"texture : {textureName}\n");
            if (texture == null)
            {
                stringBuilder.Append("   <null>\n");
            }
            else
            {
                stringBuilder.Append($"   format : {texture.format}\n");
                stringBuilder.Append($"   width  : {texture.width}\n");
                stringBuilder.Append($"   height : {texture.height}\n");
                stringBuilder.Append($"   mipmap : {texture.mipmapCount}\n");
            }
        }

        /// <summary>
        /// Update the raw image with the current configurations.
        /// </summary>
        void UpdateRawImage()
        {
            Debug.Assert(m_RawImage != null, "no raw image");

            // Determine the raw imge rectSize preserving the texture aspect ratio, matching the screen orientation,
            // and keeping a minimum dimension size.
            float minDimension = 480.0f;
            float maxDimension = Mathf.Round(minDimension * m_TextureAspectRatio);
            Vector2 rectSize;
            float rotation;
            switch (m_CurrentScreenOrientation)
            {
                case ScreenOrientation.LandscapeRight:
                    rectSize = new Vector2(maxDimension, minDimension);
                    rotation = 180.0f;
                    break;
                case ScreenOrientation.LandscapeLeft:
                    rectSize = new Vector2(maxDimension, minDimension);
                    rotation = 0.0f;
                    break;
                case ScreenOrientation.PortraitUpsideDown:
                    rectSize = new Vector2(minDimension, maxDimension);
                    rotation = 90.0f;
                    break;
                case ScreenOrientation.Portrait:
                default:
                    rectSize = new Vector2(minDimension, maxDimension);
                    rotation = 270.0f;
                    break;
            }

            // Determine the raw image material and maxDistance material parameter based on the display mode.
            float maxDistance;
            Material material;
            switch (m_DisplayMode)
            {
                case DisplayMode.HumanStencil:
                    material = m_StencilMaterial;
                    maxDistance = m_MaxHumanDistance;
                    break;
                case DisplayMode.HumanDepth:
                    material = m_DepthMaterial;
                    maxDistance = m_MaxHumanDistance;
                    break;
                case DisplayMode.EnvironmentDepth:
                default:
                    material = m_DepthMaterial;
                    maxDistance = m_MaxEnvironmentDistance;
                    break;
            }

            // Update the raw image dimensions and the raw image material parameters.
            m_RawImage.rectTransform.sizeDelta = rectSize;
            material.SetFloat(k_TextureRotationId, rotation);
            material.SetFloat(k_MaxDistanceId, maxDistance);
            m_RawImage.material = material;
        }

        /// <summary>
        /// Callback when the depth mode dropdown UI has a value change.
        /// </summary>
        /// <param name="dropdown">The dropdown UI that changed.</param>
        public void OnDepthModeDropdownValueChanged(Dropdown dropdown)
        {
            // Update the display mode from the dropdown value.
            m_DisplayMode = (DisplayMode)dropdown.value;

            // XXX stinson 2020/07/20 : This is a workaround for an iOS 14 beta issue that causes environment depth and
            // human depth/stencil to not work together. This should be removed when iOS 14 exits beta.
            Debug.Assert(m_OcclusionManager != null, "no occlusion manager");
            switch (m_DisplayMode)
            {
                case DisplayMode.HumanStencil:
                    m_OcclusionManager.requestedOcclusionPreferenceMode = OcclusionPreferenceMode.PreferHumanOcclusion;
                    break;
                case DisplayMode.HumanDepth:
                    m_OcclusionManager.requestedOcclusionPreferenceMode = OcclusionPreferenceMode.PreferHumanOcclusion;
                    break;
                case DisplayMode.EnvironmentDepth:
                default:
                    m_OcclusionManager.requestedOcclusionPreferenceMode = OcclusionPreferenceMode.PreferEnvironmentOcclusion;
                    break;
            }

            // Update the raw image following the mode change.
            UpdateRawImage();
        }
    }
}
