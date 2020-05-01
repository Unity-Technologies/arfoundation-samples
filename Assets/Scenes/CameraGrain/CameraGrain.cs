using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CameraGrain: MonoBehaviour
{
    ARCameraManager m_CameraManager;
    Renderer m_Renderer;

    void Start()
    {
        m_CameraManager = FindObjectOfType (typeof(ARCameraManager)) as ARCameraManager;
        m_Renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if(m_Renderer != null && m_CameraManager.cameraGrainTexture != null)
        {
            m_Renderer.material.SetTexture("_NoiseTex", m_CameraManager.cameraGrainTexture);
            m_Renderer.material.SetFloat("_NoiseIntensity", m_CameraManager.noiseIntensity);
        }
    }

}