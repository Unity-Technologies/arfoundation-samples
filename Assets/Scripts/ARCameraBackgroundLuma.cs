using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARCameraBackground))]
public class ARCameraBackgroundRenderTexture : MonoBehaviour
{
    [Tooltip("The RenderTexture the AR camera background will be copied into. Use a smaller texture for better performance.")]
    public RenderTexture m_TargetRenderTexture;

    public Text luma601Text;
    public Text luma709Text;

    private ARCameraBackground m_CameraBackground;

    // Cache values
    private Rect m_CameraBackgroundRect;
    private Texture2D m_CameraBackgroundTexture;
    private List<Texture2D> m_Textures = new List<Texture2D>();

    void Start()
    {
        m_CameraBackground = GetComponent<ARCameraBackground>();

        Debug.Assert(m_TargetRenderTexture, "The target render texture hasn't been set.");

        // Cache the Rect used for reading the pixels from the background RenderTexture
        m_CameraBackgroundRect = new Rect(0, 0, m_TargetRenderTexture.width, m_TargetRenderTexture.height);

        // Cache the Texture2D that the pixels will be read into
        m_CameraBackgroundTexture = new Texture2D(m_TargetRenderTexture.width, m_TargetRenderTexture.height, TextureFormat.ARGB32, false);
    }

    private void OnEnable()
    {
        ARSubsystemManager.cameraFrameReceived += ARSubsystemManager_cameraFrameReceived;
    }

    private void OnDisable()
    {
        ARSubsystemManager.cameraFrameReceived -= ARSubsystemManager_cameraFrameReceived;
    }

    private void ARSubsystemManager_cameraFrameReceived(ARCameraFrameEventArgs cameraFrameEventArgs)
    {

        // Copy the background to our target RenderTexture
        // We re-use the material that has been set up for the background renderer.
        // The background renderer deals with glsl external samplers on ARCore and YCrCb on ARKit.
        // If you change the default background material this may not work the way you expect

        // ARKit has two textures, Y and CrCb. They're already set up in the material.
        // There is no main texture, so we can leave the source texture as null
        Texture2D sourceTexture = null;

#if UNITY_ANDROID
        // ARCore has one RGB texture.
        ARSubsystemManager.cameraSubsystem.GetTextures(m_Textures);
        sourceTexture = m_Textures[0];
#endif

        Graphics.Blit(sourceTexture, m_TargetRenderTexture, m_CameraBackground.backgroundRenderer.backgroundMaterial);

        // Grab the pixels from the RenderTexture so we can calculate luma.
        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = m_TargetRenderTexture;
        m_CameraBackgroundTexture.ReadPixels(m_CameraBackgroundRect, 0, 0, false);
        m_CameraBackgroundTexture.Apply();
        RenderTexture.active = currentActiveRT;

        Color[] cameraBackgroundColors = m_CameraBackgroundTexture.GetPixels();

        // Calculate Luma to get different scene brightness estimations
        // https://en.wikipedia.org/wiki/Luma_(video)
        double luma601 = 0;
        double luma709 = 0;
        double countInv = 1 / (double)(cameraBackgroundColors.Length);
        for (int i = 0; i < cameraBackgroundColors.Length; i++)
        {
            Color c = cameraBackgroundColors[i];
            luma601 += (0.299 * c.r + 0.587 * c.g + 0.114 * c.b) * countInv;
            luma709 += (0.2126 * c.r + 0.7152 * c.g + 0.0722 * c.b) * countInv;
        }

        luma601Text.text = string.Format("{0:0.000}", luma601);
        luma709Text.text = string.Format("{0:0.000}", luma709);
    }
}
