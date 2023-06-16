using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Monitors an <see cref="XRController"/> and activates or deactivates a GameObject whenever the controller is
    /// enabled or disabled, respectively.
    /// </summary>
    public class ControllerDeviceMonitor : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The controller device to monitor. Choose either LeftHand or RightHand.")]
        XRNode m_ControllerDevice = XRNode.RightHand;

        [SerializeField]
        [Tooltip("The GameObject to activate or deactivate when the controller device is enabled or disabled, respectively.")]
        GameObject m_DeviceGameObject;

        /// <summary>
        /// The controller device to monitor. Valid values are either <c>LeftHand</c> or <c>RightHand</c>.
        /// </summary>
        public XRNode controllerDevice
        {
            get => m_ControllerDevice;
            set => m_ControllerDevice = value;
        }

        /// <summary>
        /// The GameObject to activate or deactivate when the controller device is enabled or disabled, respectively.
        /// </summary>
        public GameObject deviceGameObject
        {
            get => m_DeviceGameObject;
            set => m_DeviceGameObject = value;
        }

        void OnEnable()
        {
            UpdateDeviceGameObject(GetXRController());
            InputSystem.InputSystem.onDeviceChange += ProcessDeviceChange;
        }

        void OnDisable()
        {
            InputSystem.InputSystem.onDeviceChange -= ProcessDeviceChange;
        }

        void ProcessDeviceChange(InputSystem.InputDevice inputDevice, InputDeviceChange change)
        {
            var xrController = GetXRController();
            if (xrController != null && inputDevice != xrController)
                return;

            switch (change)
            {
                case InputDeviceChange.Added:
                case InputDeviceChange.Removed:
                case InputDeviceChange.Disconnected:
                case InputDeviceChange.Reconnected:
                case InputDeviceChange.Enabled:
                case InputDeviceChange.Disabled:
                    UpdateDeviceGameObject(xrController);
                    break;
                default:
                    break;
            }
        }

        XRController GetXRController()
        {
            switch (m_ControllerDevice)
            {
                case XRNode.LeftHand:
                    return XRController.leftHand;
                case XRNode.RightHand:
                    return XRController.rightHand;
                default:
                    return null;
            }
        }

        void UpdateDeviceGameObject(XRController deviceToCheck)
        {
            bool controllerState = deviceToCheck is {enabled: true};
            if (m_DeviceGameObject != null && controllerState != m_DeviceGameObject.activeSelf)
            {
                m_DeviceGameObject.SetActive(controllerState);
            }
        }
    }
}
