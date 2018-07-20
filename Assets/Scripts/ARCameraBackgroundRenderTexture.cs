using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARCameraBackground))]
public class ARCameraBackgroundRenderTexture : MonoBehaviour {

    public RenderTexture targetRenderTexture;

    private ARCameraBackground m_cameraBackground;
    private List<Texture2D> m_Textures = new List<Texture2D>();


	void Start () {
        m_cameraBackground = GetComponent<ARCameraBackground>();
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
        // NOTE: This relies on the default background material. If you change the default background
        //    material, then you may have to change the material setup here.
        
        ARSubsystemManager.cameraSubsystem.GetTextures(m_Textures);
#if UNITY_ANDROID
        // On ARCore we require a glsl shader that can access the external sampler.
        // We'll simply re-use the material that has been set up for the background render.
        // There is one RBG texture, we'll Blit it to our RenderTexture.
        Graphics.Blit(m_Textures[0], targetRenderTexture, m_cameraBackground.backgroundRenderer.backgroundMaterial);
#elif UNITY_IOS
        // On ARKit there are two textures, Y and CbCr. The shader combines these into a single RGB output.
        // We re-use the existing background material. But we have to set up the two textures first.
        Material backgroundMaterial = m_cameraBackground.backgroundRenderer.backgroundMaterial;
        backgroundMaterial.SetTexture("_textureY", m_Textures[0]);
        backgroundMaterial.SetTexture("_textureCbCr", m_Textures[1]);
        
        // We don't have a main texture, so just pass in null.
        Graphics.Blit(null, targetRenderTexture, backgroundMaterial);
#endif
    }
}
