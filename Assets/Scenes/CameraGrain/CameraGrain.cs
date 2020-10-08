using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(Renderer))]
    public class CameraGrain : MonoBehaviour
    {
        [SerializeField]
        ARCameraManager m_CameraManager;
        public ARCameraManager cameraManager
        {
            get => m_CameraManager;
            set => m_CameraManager = value;
        }

        Renderer m_Renderer;

        void Start()
        {
            if (m_CameraManager == null)
            {
                m_CameraManager = FindObjectOfType<ARCameraManager>();
            }

            m_Renderer = GetComponent<Renderer>();
            m_CameraManager.frameReceived += OnReceivedFrame;
        }

        void OnDisable()
        {
            m_CameraManager.frameReceived -= OnReceivedFrame;
        }

        void OnReceivedFrame(ARCameraFrameEventArgs eventArgs)
        {
            if (m_Renderer && eventArgs.cameraGrainTexture)
            {
                m_Renderer.material.SetTexture("_NoiseTex", eventArgs.cameraGrainTexture);
                m_Renderer.material.SetFloat("_NoiseIntensity", eventArgs.noiseIntensity);
            }
        }
    }
}
