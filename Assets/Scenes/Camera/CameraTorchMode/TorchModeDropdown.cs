using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(Dropdown))]
    public class TorchModeDropdown : MonoBehaviour
    {
        [SerializeField]
        ARCameraManager m_CameraManager;

        [SerializeField]
        GameObject m_NotSupportedLabel;

        Dropdown m_Dropdown;

        const string k_OnString = "On";
        const string k_OffString = "Off";

        public ARCameraManager cameraManager
        {
            get => m_CameraManager;
            set => m_CameraManager = value;
        }

        public UnityEvent<bool> torchModeSupported;

        void Reset()
        {
            m_CameraManager = FindAnyObjectByType<ARCameraManager>();
        }

        private void OnEnable()
        {
            if (m_CameraManager != null)
            {
                m_CameraManager = FindAnyObjectByType<ARCameraManager>();
            }
            if (m_CameraManager == null)
            {
                Debug.LogError("Failed to find ARCameraManager in current scene. As a result, this component will be disabled.", this);
                enabled = false;
            }
        }

        void Awake()
        {
            m_Dropdown = GetComponent<Dropdown>();
            m_Dropdown.ClearOptions();
            m_Dropdown.onValueChanged.AddListener(
                _ =>
                {
                    OnDropdownValueChanged(m_Dropdown);
                }
            );
            ARSession.stateChanged += SessionChanged;
        }

        void Start()
        {
            PopulateDropdown();
        }

        void PopulateDropdown()
        {
            var names = new List<string> { k_OffString, k_OnString };
            m_Dropdown.AddOptions(names);
        }

        void OnDropdownValueChanged(Dropdown dropdown)
        {
            if (m_CameraManager == null)
                return;

            var selectedOption = dropdown.options[dropdown.value].text;
            XRCameraTorchMode requestedMode = XRCameraTorchMode.Off;

            if (selectedOption == k_OnString)
            {
                requestedMode = XRCameraTorchMode.On;
            }

            m_CameraManager.requestedCameraTorchMode = requestedMode;
        }

        void SessionChanged(ARSessionStateChangedEventArgs change)
        {
            if (m_CameraManager == null)
                return;

            if ((change.state == ARSessionState.Ready
                || change.state == ARSessionState.SessionTracking)
                && m_CameraManager.DoesCurrentCameraSupportTorch())
            {
                if (torchModeSupported != null)
                    torchModeSupported.Invoke(true);
                if (m_NotSupportedLabel != null)
                    m_NotSupportedLabel.SetActive(false);
            }
            else
            {
                if (torchModeSupported != null)
                    torchModeSupported.Invoke(false);
                if (m_NotSupportedLabel != null)
                    m_NotSupportedLabel.SetActive(true);
            }
        }
    }
}
