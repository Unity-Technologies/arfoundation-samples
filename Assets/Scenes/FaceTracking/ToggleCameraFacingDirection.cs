using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class ToggleCameraFacingDirection : PressInputBase
    {
        [SerializeField]
        ARCameraManager m_CameraManager;

        public ARCameraManager cameraManager
        {
            get => m_CameraManager;
            set => m_CameraManager = value;
        }

        [SerializeField]
        ARSession m_Session;

        public ARSession session
        {
            get => m_Session;
            set => m_Session = value;
        }

        protected override void OnPressBegan(Vector3 position)
        {
            if (m_CameraManager == null || m_Session == null)
                return;


            if (m_CameraManager.requestedFacingDirection == CameraFacingDirection.User)
            {
                m_CameraManager.requestedFacingDirection = CameraFacingDirection.World;
            }
            else
            {
                m_CameraManager.requestedFacingDirection = CameraFacingDirection.User;
            }
        }
    }
}
