using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class CameraGrain: MonoBehaviour
    {
        [SerializeField]
        ARCameraManager m_CameraManager;
        public ARCameraManager cameraManager
        {
            get { return m_CameraManager; }
            set { m_CameraManager = value; }
        }

        Renderer m_Renderer;

        void Start()
        {
            if(m_CameraManager == null)
            {
                m_CameraManager = FindObjectOfType (typeof(ARCameraManager)) as ARCameraManager;
            }

            m_Renderer = GetComponent<Renderer>();
            m_CameraManager.frameReceived += OnReceivedFrame;
        }

        void OnDisable()
        {
            m_CameraManager.frameReceived -= OnReceivedFrame;
        }

        void OnReceivedFrame(ARCameraFrameEventArgs eventArgs){
            if(m_Renderer != null && eventArgs.cameraGrainTexture != null)
            {
                m_Renderer.material.SetTexture("_NoiseTex", eventArgs.cameraGrainTexture);
                m_Renderer.material.SetFloat("_NoiseIntensity", eventArgs.noiseIntensity);
            }
        }
    }
}