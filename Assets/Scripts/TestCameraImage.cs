using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARExtensions;
using UnityEngine.XR.ARFoundation;
#if !UNITY_2018_2_OR_NEWER
using Unity.Collections;
#endif

/// <summary>
/// This component tests getting the latest camera image
/// and converting it to RGBA format. If successful,
/// it displays the image on the screen as a RawImage
/// and also displays information about the image.
/// </summary>
public class TestCameraImage : MonoBehaviour
{
    [SerializeField]
    RawImage m_RawImage;

    /// <summary>
    /// The UI RawImage used to display the image on screen.
    /// </summary>
    public RawImage rawImage
    {
        get { return m_RawImage; }
        set { m_RawImage = value; }
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

    Texture2D m_Texture;

    void OnEnable()
    {
        ARSubsystemManager.cameraFrameReceived += OnCameraFrameReceived;
    }

    void OnDisable()
    {
        ARSubsystemManager.cameraFrameReceived -= OnCameraFrameReceived;
    }

    unsafe void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        // Attempt to get the latest camera image. If this method succeeds,
        // it acquires a native resource that must be disposed (see below).
        CameraImage image;
        if (!ARSubsystemManager.cameraSubsystem.TryGetLatestImage(out image))
            return;

        // Display some information about the camera image
        m_ImageInfo.text = string.Format(
            "Image info:\n\twidth: {0}\n\theight: {1}\n\tplaneCount: {2}\n\ttimestamp: {3}\n\tformat: {4}",
            image.width, image.height, image.planeCount, image.timestamp, image.format);

        // Choose an RGBA format.
        // See CameraImage.FormatSupported for a complete list of supported formats.
        var format = TextureFormat.RGBA32;

        if (m_Texture == null)
            m_Texture = new Texture2D(image.width, image.height, format, false);

        // Convert the image to format, flipping the image across the Y axis.
        // We can also get a sub rectangle, but we'll get the full image here.
        var conversionParams = new CameraImageConversionParams(image, format, CameraImageTransformation.MirrorY);

#if UNITY_2018_2_OR_NEWER
        // In 2018.2+, Texture2D allows us write directly to the raw texture data
        // This allows us to do the conversion in-place without making any copies.
        var rawTextureData = m_Texture.GetRawTextureData<byte>();
        image.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
#else
        // In 2018.1, Texture2D didn't have this feature, so we'll create
        // a temporary buffer and perform the conversion using that data.
        int size = image.GetConvertedDataSize(conversionParams);
        var rawTextureData = new NativeArray<byte>(size, Allocator.Temp);
        var ptr = new IntPtr(rawTextureData.GetUnsafePtr());
        image.Convert(conversionParams, ptr, rawTextureData.Length);
        m_Texture.LoadRawTextureData(ptr, rawTextureData.Length);
        rawTextureData.Dispose();
#endif

        // We must dispose of the CameraImage after we're finished
        // with it to avoid leaking native resources.
        image.Dispose();

        // Apply the updated texture data to our texture
        m_Texture.Apply();

        // Set the RawImage's texture so we can visualize it.
        m_RawImage.texture = m_Texture;
    }
}
