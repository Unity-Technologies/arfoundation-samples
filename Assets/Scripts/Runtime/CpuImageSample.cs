using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// <para>This component tests getting the latest camera image and converting it to RGBA format. If successful,
    /// it displays the image on the screen as a Raw Image and also displays information about the image.
    /// This is useful for computer vision applications where you need to access the raw pixels from camera image
    /// on the CPU.</para>
    /// <para>This is different from the ARCameraBackground component, which efficiently displays the camera image on the screen.
    /// If you just want to blit the camera texture to the screen, use the ARCameraBackground, or use Graphics.Blit to create
    /// a GPU-friendly RenderTexture.</para>
    /// <para>In this example, we get the camera image data on the CPU, convert it to an RGBA format, then display it on the screen
    /// as a RawImage texture to demonstrate it is working. This is done as an example; do not use this technique simply
    /// to render the camera image on screen.</para>
    /// </summary>
    public class CpuImageSample : MonoBehaviour
    {
        CameraDirection m_CameraDirection;
        Texture2D m_CameraTexture;
        XRCpuImage.Transformation m_Transformation = XRCpuImage.Transformation.MirrorY;

        [SerializeField]
        [Tooltip("The ARCameraManager which will produce frame events.")]
        ARCameraManager m_CameraManager;

        [SerializeField]
        [Tooltip("The AROcclusionManager which will produce human depth and stencil textures.")]
        AROcclusionManager m_OcclusionManager;

        [SerializeField]
        [Tooltip("The ARSession will select the camera configuration chooser.")]
        ARSession m_Session;

        [SerializeField]
        RawImage m_RawCameraImage;

        /// <summary>
        /// Get or set the UI RawImage used to display the image on screen.
        /// </summary>
        public RawImage rawCameraImage
        {
            get => m_RawCameraImage;
            set => m_RawCameraImage = value;
        }

        [SerializeField]
        RawImage m_RawHumanDepthImage;

        /// <summary>
        /// The UI RawImage used to display the image on screen.
        /// </summary>
        public RawImage rawHumanDepthImage
        {
            get => m_RawHumanDepthImage;
            set => m_RawHumanDepthImage = value;
        }

        [SerializeField]
        RawImage m_RawHumanStencilImage;

        /// <summary>
        /// The UI RawImage used to display the image on screen.
        /// </summary>
        public RawImage rawHumanStencilImage
        {
            get => m_RawHumanStencilImage;
            set => m_RawHumanStencilImage = value;
        }

        [SerializeField]
        RawImage m_RawEnvironmentDepthImage;

        /// <summary>
        /// The UI RawImage used to display the image on screen.
        /// </summary>
        public RawImage rawEnvironmentDepthImage
        {
            get => m_RawEnvironmentDepthImage;
            set => m_RawEnvironmentDepthImage = value;
        }

        [SerializeField]
        RawImage m_RawEnvironmentDepthConfidenceImage;

        /// <summary>
        /// The UI RawImage used to display the image on screen.
        /// </summary>
        public RawImage rawEnvironmentDepthConfidenceImage
        {
            get => m_RawEnvironmentDepthConfidenceImage;
            set => m_RawEnvironmentDepthConfidenceImage = value;
        }

        [SerializeField]
        Text m_ImageInfo;

        /// <summary>
        /// The UI Text used to display information about the image on screen.
        /// </summary>
        public Text imageInfo
        {
            get => m_ImageInfo;
            set => m_ImageInfo = value;
        }

        [HideInInspector]
        [SerializeField]
        Button m_TransformationButton;

        /// <summary>
        /// The button that controls transformation selection.
        /// </summary>
        public Button transformationButton
        {
            get => m_TransformationButton;
            set => m_TransformationButton = value;
        }
        
        [HideInInspector]
        [SerializeField]
        Button m_SwapCameraButton;

        /// <summary>
        /// The button that controls camera swapping.
        /// </summary>
        public Button swapCameraButton
        {
            get => m_SwapCameraButton;
            set => m_SwapCameraButton = value;
        }
        
        delegate bool TryAcquireDepthImageDelegate(out XRCpuImage image);
        
        static readonly ConfigurationChooser s_PreferCameraConfigurationChooser = new PreferCameraConfigurationChooser();

        /// <summary>
        /// Cycles the image transformation to the next case.
        /// </summary>
        public void CycleTransformation()
        {
            m_Transformation = m_Transformation switch
            {
                XRCpuImage.Transformation.None => XRCpuImage.Transformation.MirrorX,
                XRCpuImage.Transformation.MirrorX => XRCpuImage.Transformation.MirrorY,
                XRCpuImage.Transformation.MirrorY => XRCpuImage.Transformation.MirrorX | XRCpuImage.Transformation.MirrorY,
                _ => XRCpuImage.Transformation.None
            };

            if (m_TransformationButton)
            {
                m_TransformationButton.GetComponentInChildren<Text>().text = m_Transformation.ToString();
            }
        }

        /// <summary>
        /// Swaps the CPU Camera between World (back) and User (front). 
        /// </summary>
        public void SwapCamera()
        {
            m_CameraDirection.Toggle();

            if (m_SwapCameraButton)
            {
                UpdateSwapCameraButtonText();
            }
        }

        void UpdateSwapCameraButtonText()
        {
            m_SwapCameraButton.GetComponentInChildren<Text>().text = m_CameraDirection.cameraManager.requestedFacingDirection.ToString();
        }
        
        void Reset()
        { 
            m_Session = FindAnyObjectByType<ARSession>(); 
        }

        void OnEnable()
        {
            if (m_CameraManager == null)
            {
                Debug.LogException(new NullReferenceException(
                    $"Serialized properties were not initialized on {name}'s {nameof(CpuImageSample)} component."), this);
                return;
            }

            m_CameraManager.frameReceived += OnCameraFrameReceived;
            
            m_Session.subsystem.configurationChooser = s_PreferCameraConfigurationChooser;
            
            m_CameraDirection = new CameraDirection(m_CameraManager);
            UpdateSwapCameraButtonText();   
        }
        

        void OnDisable()
        {
            if (m_CameraManager != null)
                m_CameraManager.frameReceived -= OnCameraFrameReceived;
        }

        void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
        {
            UpdateCameraImage();
            UpdateDepthImage(m_OcclusionManager.TryAcquireHumanDepthCpuImage, m_RawHumanDepthImage);
            UpdateDepthImage(m_OcclusionManager.TryAcquireHumanStencilCpuImage, m_RawHumanStencilImage);
            UpdateDepthImage(m_OcclusionManager.TryAcquireEnvironmentDepthCpuImage, m_RawEnvironmentDepthImage);
            UpdateDepthImage(m_OcclusionManager.TryAcquireEnvironmentDepthConfidenceCpuImage, m_RawEnvironmentDepthConfidenceImage);
        }

        unsafe void UpdateCameraImage()
        {
            // Attempt to get the latest camera image. If this method succeeds,
            // it acquires a native resource that must be disposed (see below).
            if (!m_CameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
            {
                return;
            }

            // Display some information about the camera image
            m_ImageInfo.text = string.Format(
                "Image info:\n\twidth: {0}\n\theight: {1}\n\tplaneCount: {2}\n\ttimestamp: {3}\n\tformat: {4}",
                image.width, image.height, image.planeCount, image.timestamp, image.format);

            // Once we have a valid XRCpuImage, we can access the individual image "planes"
            // (the separate channels in the image). XRCpuImage.GetPlane provides
            // low-overhead access to this data. This could then be passed to a
            // computer vision algorithm. Here, we will convert the camera image
            // to an RGBA texture and draw it on the screen.

            // Choose an RGBA format.
            // See XRCpuImage.FormatSupported for a complete list of supported formats.
            const TextureFormat format = TextureFormat.RGBA32;

            if (m_CameraTexture == null || m_CameraTexture.width != image.width || m_CameraTexture.height != image.height)
                m_CameraTexture = new Texture2D(image.width, image.height, format, false);

            // Convert the image to format, flipping the image across the Y axis.
            // We can also get a sub rectangle, but we'll get the full image here.
            var conversionParams = new XRCpuImage.ConversionParams(image, format, m_Transformation);

            // Texture2D allows us write directly to the raw texture data
            // This allows us to do the conversion in-place without making any copies.
            var rawTextureData = m_CameraTexture.GetRawTextureData<byte>();
            try
            {
                image.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
            }
            finally
            {
                // We must dispose of the XRCpuImage after we're finished
                // with it to avoid leaking native resources.
                image.Dispose();
            }

            // Apply the updated texture data to our texture
            m_CameraTexture.Apply();

            // Set the RawImage's texture so we can visualize it.
            m_RawCameraImage.texture = m_CameraTexture;
        }

        /// <summary>
        /// Calls <paramref name="tryAcquireDepthImageDelegate"/> and renders the resulting depth image contents to <paramref name="rawImage"/>.
        /// </summary>
        /// <param name="tryAcquireDepthImageDelegate">The method to call to acquire a depth image.</param>
        /// <param name="rawImage">The Raw Image to use to render the depth image to the screen.</param>
        void UpdateDepthImage(TryAcquireDepthImageDelegate tryAcquireDepthImageDelegate, RawImage rawImage)
        {
            if (tryAcquireDepthImageDelegate(out XRCpuImage cpuImage))
            {
                // XRCpuImages, if successfully acquired, must be disposed.
                // You can do this with a using statement as shown below, or by calling its Dispose() method directly.
                using (cpuImage)
                {
                    UpdateRawImage(rawImage, cpuImage, m_Transformation);
                }
            }
            else
            {
                rawImage.enabled = false;
            }
        }

        static void UpdateRawImage(RawImage rawImage, XRCpuImage cpuImage, XRCpuImage.Transformation transformation)
        {
            // Get the texture associated with the UI.RawImage that we wish to display on screen.
            var texture = rawImage.texture as Texture2D;

            // If the texture hasn't yet been created, or if its dimensions have changed, (re)create the texture.
            // Note: Although texture dimensions do not normally change frame-to-frame, they can change in response to
            //    a change in the camera resolution (for camera images) or changes to the quality of the human depth
            //    and human stencil buffers.
            if (texture == null || texture.width != cpuImage.width || texture.height != cpuImage.height)
            {
                texture = new Texture2D(cpuImage.width, cpuImage.height, cpuImage.format.AsTextureFormat(), false);
                rawImage.texture = texture;
            }

            // For display, we need to mirror about the vertical access.
            var conversionParams = new XRCpuImage.ConversionParams(cpuImage, cpuImage.format.AsTextureFormat(), transformation);

            // Get the Texture2D's underlying pixel buffer.
            var rawTextureData = texture.GetRawTextureData<byte>();

            // Make sure the destination buffer is large enough to hold the converted data (they should be the same size)
            Debug.Assert(rawTextureData.Length == cpuImage.GetConvertedDataSize(conversionParams.outputDimensions, conversionParams.outputFormat),
                "The Texture2D is not the same size as the converted data.");

            // Perform the conversion.
            cpuImage.Convert(conversionParams, rawTextureData);

            // "Apply" the new pixel data to the Texture2D.
            texture.Apply();

            // Make sure it's enabled.
            rawImage.enabled = true;
        }
    }
}
