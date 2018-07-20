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
        ARSubsystemManager.cameraSubsystem.GetTextures(m_Textures);
        
        // Re-use the material that has been set up for the background render.
        // NOTE: This relies on the default background material. If you change the default background
        //    material this may not work the way you expect
        Graphics.Blit(m_Textures[0], targetRenderTexture, m_cameraBackground.backgroundRenderer.backgroundMaterial);
    }
}
