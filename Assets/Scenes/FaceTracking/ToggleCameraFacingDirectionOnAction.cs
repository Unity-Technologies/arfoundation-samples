using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class ToggleCameraFacingDirectionOnAction : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The input action that triggers the camera to change facing directions")]
        InputActionReferences m_InputActions;

        [SerializeField]
        [Tooltip("The camera manager that is used to flip the camera facing direction")]
        ARCameraManager m_CameraManager;

        CameraDirection m_CameraDirection;

        public InputActionProperty inputAction
        {
            get => m_InputActions.screenTap;
            set => m_InputActions.screenTap = value;
        }

        public ARCameraManager cameraManager
        {
            get => m_CameraDirection.cameraManager;
            set => m_CameraDirection.cameraManager = value;
        }

        void OnEnable()
        {
            if (m_InputActions.screenTap.action != null)
                m_InputActions.screenTap.action.performed += OnActionPerformed;
        }

        void Awake()
        {
            m_CameraDirection = new CameraDirection(m_CameraManager);
        }

        void OnDisable()
        {
            if (m_InputActions.screenTap.action != null)
                m_InputActions.screenTap.action.performed -= OnActionPerformed;
        }

        void OnActionPerformed(InputAction.CallbackContext context)
        {
            m_CameraDirection.Toggle();
        }
    }
}
