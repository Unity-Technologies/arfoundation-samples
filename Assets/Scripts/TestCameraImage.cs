using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// This component tests getting the latest camera image
/// and converting it to RGBA format. If successful,
/// it displays the image on the screen as a RawImage
/// and also displays information about the image.
///
/// This is useful for computer vision applications where
/// you need to access the raw pixels from camera image
/// on the CPU.
///
/// This is different from the ARCameraBackground component, which
/// efficiently displays the camera image on the screen. If you
/// just want to blit the camera texture to the screen, use
/// the ARCameraBackground, or use Graphics.Blit to create
/// a GPU-friendly RenderTexture.
///
/// In this example, we get the camera image data on the CPU,
/// convert it to an RGBA format, then display it on the screen
/// as a RawImage texture to demonstrate it is working.
/// This is done as an example; do not use this technique simply
/// to render the camera image on screen.
/// </summary>
public class TestCameraImage : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The ARCameraManager which will produce frame events.")]
    ARCameraManager m_CameraManager;

    /// <summary>
    /// Get or set the <c>ARCameraManager</c>.
    /// </summary>
    public ARCameraManager cameraManager
    {
        get { return m_CameraManager; }
        set { m_CameraManager = value; }
    }

    [SerializeField]
    RawImage m_RawCameraImage;

    /// <summary>
    /// The UI RawImage used to display the image on screen.
    /// </summary>
    public RawImage rawCameraImage
    {
        get { return m_RawCameraImage; }
        set { m_RawCameraImage = value; }
    }

     [SerializeField]
     [Tooltip("The AROcclusionManager which will produce human depth and stencil textures.")]
     AROcclusionManager m_OcclusionManager;

     public AROcclusionManager occlusionManager
     {
         get => m_OcclusionManager;
         set => m_OcclusionManager = value;
     }

     [SerializeField]
     RawImage m_RawHumanDepthImage;

     /// <summary>
     /// The UI RawImage used to display the image on screen.
     /// </summary>
     public RawImage rawHumanDepthImage
     {
         get { return m_RawHumanDepthImage; }
         set { m_RawHumanDepthImage = value; }
     }

     [SerializeField]
     RawImage m_RawHumanStencilImage;

     /// <summary>
     /// The UI RawImage used to display the image on screen.
     /// </summary>
     public RawImage rawHumanStencilImage
     {
         get { return m_RawHumanStencilImage; }
         set { m_RawHumanStencilImage = value; }
     }

    [SerializeField]
    Text m_ImageInfo;

    /// <summary>
    /// The UI Text used to display information about the image on screen.
    /// </summary>
    public Text imageInfo
    {
        get { return m_ImageInfo; }
        set { m_ImageInfo = value; }
    }

    void OnEnable()
    {
        if (m_CameraManager != null)
        {
            m_CameraManager.frameReceived += OnCameraFrameReceived;
        }
    }

    void OnDisable()
    {
        if (m_CameraManager != null)
        {
            m_CameraManager.frameReceived -= OnCameraFrameReceived;
        }
    }

    unsafe void UpdateCameraImage()
    {
        // Attempt to get the latest camera image. If this method succeeds,
        // it acquires a native resource that must be disposed (see below).
        if (!cameraManager.TryGetLatestImage(out XRCpuImage image))
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
        var format = TextureFormat.RGBA32;

        if (m_CameraTexture == null || m_CameraTexture.width != image.width || m_CameraTexture.height != image.height)
        {
            m_CameraTexture = new Texture2D(image.width, image.height, format, false);
        }

        // Convert the image to format, flipping the image across the Y axis.
        // We can also get a sub rectangle, but we'll get the full image here.
        var conversionParams = new XRCpuImage.ConversionParams(image, format, XRCpuImage.Transformation.MirrorY);

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

    void UpdateHumanDepthImage()
    {
        if (occlusionManager == null)
            return;

        // Attempt to get the latest human depth image. If this method succeeds,
        // it acquires a native resource that must be disposed (see below).
        if (!occlusionManager.TryAcquireHumanDepthCpuImage(out XRCpuImage image))
            return;

        using (image)
        {
            if (m_HumanDepthTexture == null || m_HumanDepthTexture.width != image.width || m_HumanDepthTexture.height != image.height)
            {
                m_HumanDepthTexture = new Texture2D(image.width, image.height, TextureFormat.R8, false);
            }

            var rawData = m_HumanDepthTexture.GetRawTextureData<byte>();

            rawData.CopyFrom(image.GetPlane(0).data);

            m_HumanDepthTexture.Apply();

            m_RawHumanDepthImage.texture = m_HumanDepthTexture;
        }
    }

    void UpdateRawImage(RawImage rawImage, XRCpuImage cpuImage)
    {
        var texture = rawImage.texture as Texture2D;
        if (texture == null || texture.width != cpuImage.width || texture.height != cpuImage.height)
        {
            texture = new Texture2D(cpuImage.width, cpuImage.height, cpuImage.format.AsTextureFormat(), false);
            rawImage.texture = texture;
            Debug.Log($"Texture2D has {texture.GetRawTextureData<byte>().Length} bytes.");
        }

        Debug.Log($"Plane 0 has {cpuImage.GetPlane(0).data.Length} bytes");
        texture.GetRawTextureData<byte>().CopyFrom(cpuImage.GetPlane(0).data);
        texture.Apply();
    }

    void UpdateHumanStencilImage()
    {
        if (occlusionManager == null)
            return;

        if (m_RawHumanStencilImage == null)
            return;

        // Attempt to get the latest human stencil image. If this method succeeds,
        // it acquires a native resource that must be disposed (see below).
        if (!occlusionManager.TryAcquireHumanStencilCpuImage(out XRCpuImage image))
            return;

        using (image)
        {
            var texture = m_RawHumanStencilImage.texture as Texture2D;
            if (texture == null || texture.width != image.width || texture.height != image.height)
            {
                texture = new Texture2D(image.width, image.height, TextureFormat.RFloat, false);
                m_RawHumanStencilImage.texture = texture;
                Debug.Log($"Texture2D has {texture.GetRawTextureData<byte>().Length} bytes.");
            }

            Debug.Log($"Plane 0 has {image.GetPlane(0).data.Length} bytes");
            texture.GetRawTextureData<byte>().CopyFrom(image.GetPlane(0).data);

            texture.Apply();
        }
    }

    void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        UpdateCameraImage();
        UpdateHumanDepthImage();
        UpdateHumanStencilImage();
    }

    Texture2D m_CameraTexture;

    Texture2D m_HumanDepthTexture;
}
