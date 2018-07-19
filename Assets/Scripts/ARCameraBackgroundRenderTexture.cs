using System.Collections;
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
#if UNITY_ANDROID
        Graphics.Blit(m_Textures[0], targetRenderTexture, m_cameraBackground.backgroundRenderer.backgroundMaterial);
#elif UNITY_IOS
        Material backgroundMaterial = m_cameraBackground.backgroundRenderer.backgroundMaterial;
        m_ClearMaterial.SetTexture("_textureY", m_Textures[0]);
        m_ClearMaterial.SetTexture("_textureCbCr", m_Textures[1]);
        Graphics.Blit(null, targetRenderTexture, backgroundMaterial);
#endif
    }
}
