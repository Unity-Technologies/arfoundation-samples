using System;
using TMPro;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(ARCameraManager))]
    public class ImageStabilization : MonoBehaviour
    {
        [SerializeField]
        TMP_Text m_Information;

        [SerializeField]
        ARCameraManager m_CameraManager;

        Supported? m_Supported;
        Supported? m_SupportedOnPrevFrame;
        bool? m_EnabledOnPrevFrame;

        void OnEnable()
        {
            if (m_Information == null)
            {
                Debug.LogError($"Null reference in {nameof(ImageStabilization)} Inspector.", this);
                enabled = false;
            }

            if (ARSession.state == ARSessionState.SessionTracking)
                m_Supported = m_CameraManager.descriptor.supportsImageStabilization;
            else
                ARSession.stateChanged += OnSessionStateChange;
        }

        void OnSessionStateChange(ARSessionStateChangedEventArgs args)
        {
            if (args.state != ARSessionState.SessionTracking)
                return;

            m_Supported = m_CameraManager.descriptor.supportsImageStabilization;
            ARSession.stateChanged -= OnSessionStateChange;
        }

        public void ToggleImageStabilization()
        {
            m_Supported = m_CameraManager.descriptor.supportsImageStabilization;

            if (m_Supported == Supported.Supported)
            {
                m_CameraManager.imageStabilizationRequested = !m_CameraManager.imageStabilizationRequested;
            }
            else
            {
                m_CameraManager.imageStabilizationRequested = false;
            }
        }

        void Update()
        {
            var enabledThisFrame = m_CameraManager.imageStabilizationEnabled;

            if (m_SupportedOnPrevFrame != m_Supported || m_EnabledOnPrevFrame != enabledThisFrame)
            {
                var enabledString = m_CameraManager.imageStabilizationEnabled ? "On" : "Off";
                m_Information.text = $"Support: {m_Supported.ToString()}\nStabilization: {enabledString}";
                m_SupportedOnPrevFrame = m_Supported;
                m_EnabledOnPrevFrame = enabledThisFrame;
            }
        }

        void Reset()
        {
            InitializeSerializedFields();
        }

        [ContextMenu("Initialize Serialized Fields")]
        void InitializeSerializedFields()
        {
            m_CameraManager = GetComponent<ARCameraManager>();
        }
    }
}
