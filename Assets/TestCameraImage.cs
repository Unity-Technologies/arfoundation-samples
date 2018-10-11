using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARExtensions;
using UnityEngine.XR.ARFoundation;

public class TestCameraImage : MonoBehaviour
{
    [SerializeField]
    RawImage m_RawImage;

    [SerializeField]
    Text m_ImageInfo;

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
        CameraImage image;
        if (!ARSubsystemManager.cameraSubsystem.TryGetLatestImage(out image))
            return;

        m_ImageInfo.text = string.Format(
            "Image info:\n\twidth: {0}\n\theight: {1}\n\tplaneCount: {2}\n\ttimestamp: {3}\n\tformat: {4}",
            image.width, image.height, image.planeCount, image.timestamp, image.format);

        if (m_Texture == null)
            m_Texture = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);

        var conversionParams = new CameraImageConversionParams(image, TextureFormat.RGBA32, CameraImageTransformation.MirrorY);
        var rawTextureData = m_Texture.GetRawTextureData<byte>();
        image.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
        image.Dispose();
        m_Texture.Apply();
        m_RawImage.texture = m_Texture;
    }
}
