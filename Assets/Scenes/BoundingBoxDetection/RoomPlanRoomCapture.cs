using System.Text;
using TMPro;
#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RoomPlanRoomCapture : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The active AR Camera Manager.")]
        ARCameraManager m_CameraManager;

        [SerializeField]
        [Tooltip("The GameObject of the button used to start and stop room capture.")]
        GameObject m_RoomCaptureButton;

        [SerializeField]
        [Tooltip("The text of the room capture button.")]
        TextMeshProUGUI m_CurrentRoomCaptureButtonText;

        [SerializeField]
        [Tooltip("The GameObject for the status information and the room capture instruction.")]
        GameObject m_RoomCaptureInfo;

        [SerializeField]
        [Tooltip("The text label to display room capture instructions.")]
        TextMeshProUGUI m_CurrentInstructionLoggerText;

        ARBoundingBoxManager m_BoundingBoxManager;
        bool m_SupportsRoomCapture;

        void Reset()
        {
            m_CameraManager = FindAnyObjectByType<ARCameraManager>();
            m_BoundingBoxManager = FindAnyObjectByType<ARBoundingBoxManager>();
        }

        void Awake()
        {
            if (m_CameraManager == null)
            {
                m_CameraManager = FindAnyObjectByType<ARCameraManager>();
            }

            if (m_BoundingBoxManager == null)
            {
                m_BoundingBoxManager = FindAnyObjectByType<ARBoundingBoxManager>();
            }

#if UNITY_IOS && !UNITY_EDITOR
            if (m_RoomCaptureInfo != null)
            {
                m_RoomCaptureInfo.SetActive(true);
            }

            var version = iOS.Device.systemVersion;
            var major = int.Parse(version.Split(".")[0]);
            if (major < 17)
            {
                if (m_CurrentInstructionLoggerText != null)
                {
                    m_CurrentInstructionLoggerText.text = "Please upgrade to iOS 17 or newer to enable this feature.";
                }
                if (m_RoomCaptureButton != null)
                {
                    m_RoomCaptureButton.SetActive(false);
                }
                return;
            }

            if (m_RoomCaptureButton != null)
            {
                m_RoomCaptureButton.SetActive(true);
            }
            m_SupportsRoomCapture = true;
#else
            if (m_RoomCaptureButton != null)
            {
                m_RoomCaptureButton.SetActive(false);
            }
            if (m_RoomCaptureInfo != null)
            {
                m_RoomCaptureInfo.SetActive(false);
            }
            gameObject.SetActive(false);
#endif
        }

        void OnEnable()
        {
            if (!m_SupportsRoomCapture)
            {
                return;
            }

            if (m_CameraManager == null)
            {
                enabled = false;
                if (m_CurrentInstructionLoggerText != null)
                {
                    m_CurrentInstructionLoggerText.text = "ARCameraManager component is not found.";
                }
                if (m_RoomCaptureButton != null)
                {
                    m_RoomCaptureButton.SetActive(false);
                }
                return;
            }

            m_CameraManager.frameReceived += OnCameraFrameReceived;

            if (m_BoundingBoxManager == null)
            {
                enabled = false;
                if (m_CurrentInstructionLoggerText != null)
                {
                    m_CurrentInstructionLoggerText.text = "ARBoundingBoxManager component is not found.";
                }
                if (m_RoomCaptureButton != null)
                {
                    m_RoomCaptureButton.SetActive(false);
                }
                return;
            }

#if UNITY_IOS
            if (m_BoundingBoxManager.subsystem is not RoomPlanBoundingBoxSubsystem subsystem)
            {
                enabled = false;
                if (m_CurrentInstructionLoggerText != null)
                {
                    m_CurrentInstructionLoggerText.text = "Room capture could not be set up without RoomPlanBoundingBoxSubsystem.";
                }
                if (m_RoomCaptureButton != null)
                {
                    m_RoomCaptureButton.SetActive(false);
                }
                return;
            }

            if (!subsystem.SetupRoomCapture())
            {
                enabled = false;
                if (m_CurrentInstructionLoggerText != null)
                {
                    m_CurrentInstructionLoggerText.text = "Room capture failed to set up.\nPlease ensure the device equipped with a LiDAR scanner.";
                }
                if (m_RoomCaptureButton != null)
                {
                    m_RoomCaptureButton.SetActive(false);
                }
            }
#endif
        }

        void OnDisable()
        {
            if (m_CameraManager != null)
            {
                m_CameraManager.frameReceived -= OnCameraFrameReceived;
            }
        }

        void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
        {
            if (!m_SupportsRoomCapture)
            {
                return;
            }

#if UNITY_IOS
            if (m_BoundingBoxManager.subsystem is not RoomPlanBoundingBoxSubsystem subsystem || !subsystem.IsRoomCapturing())
            {
                return;
            }

            subsystem.GetRoomCaptureInstruction(out XRBoundingBoxInstructions instruction);
            if (m_CurrentInstructionLoggerText != null)
            {
                m_CurrentInstructionLoggerText.text = instruction.ToString();
            }
#endif
        }

        public void ToggleRoomCaptureButton()
        {
            if (!m_SupportsRoomCapture)
            {
                return;
            }

#if UNITY_IOS
            if (m_BoundingBoxManager.subsystem is not RoomPlanBoundingBoxSubsystem subsystem)
            {
                return;
            }

            if (subsystem.IsRoomCapturing())
            {
                subsystem.StopRoomCapture();

                if (m_RoomCaptureInfo != null)
                {
                    m_RoomCaptureInfo.SetActive(false);
                }
                if (m_RoomCaptureButton != null)
                {
                    m_RoomCaptureButton.SetActive(false);
                }
            }
            else
            {
                subsystem.StartRoomCapture();
                m_CurrentRoomCaptureButtonText.text = "Stop Room Capture";
                m_CurrentInstructionLoggerText.text = string.Empty;
            }
#endif
        }
    }
}
