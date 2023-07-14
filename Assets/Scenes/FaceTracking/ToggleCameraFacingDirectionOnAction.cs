using UnityEngine.InputSystem;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class ToggleCameraFacingDirectionOnAction : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The input action that triggers the camera to change facing directions")]
        InputActionProperty m_InputAction;

        [SerializeField]
        [Tooltip("The camera manager that is used to flip the camera facing direction")]
        ARCameraManager m_CameraManager;

        CameraDirection m_CameraDirection;

        public InputActionProperty inputAction
        {
            get => m_InputAction;
            set => m_InputAction = value;
        }

        public ARCameraManager cameraManager
        {
            get => m_CameraDirection.cameraManager;
            set => m_CameraDirection.cameraManager = value;
        }

        void OnEnable()
        {
            if (m_InputAction.action != null)
                m_InputAction.action.performed += OnActionPerformed;
        }

        void Awake()
        {
            m_CameraDirection = new CameraDirection(m_CameraManager);
        }

        void OnDisable()
        {
            if (m_InputAction.action != null)
                m_InputAction.action.performed -= OnActionPerformed;
        }

        void OnActionPerformed(InputAction.CallbackContext context)
        {
            m_CameraDirection.Toggle();
        }
    }
}
