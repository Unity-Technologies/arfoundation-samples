namespace UnityEngine.XR.ARFoundation.Samples
{
    public class CameraDirection
    {
        ARCameraManager m_CameraManager;

        /// <summary>
        /// The camera manager for switching the camera direction.
        /// </summary>
        public ARCameraManager cameraManager
        {
            get => m_CameraManager;
            set => m_CameraManager = value;
        }

        public CameraDirection(ARCameraManager cameraManager)
        {
            m_CameraManager = cameraManager;
        }

        /// <summary>
        /// Toggle the requested camera facing direction.
        /// </summary>
        public void Toggle()
        {
            if (m_CameraManager == null)
            {
                Debug.LogError("camera manager cannot be null");
                return;
            }

            CameraFacingDirection newFacingDirection;
            switch (m_CameraManager.requestedFacingDirection)
            {
                case CameraFacingDirection.World:
                    newFacingDirection = CameraFacingDirection.User;
                    break;
                case CameraFacingDirection.User:
                case CameraFacingDirection.None:
                default:
                    newFacingDirection = CameraFacingDirection.World;
                    break;
            }

            Debug.Log($"Switching ARCameraManager.requestedFacingDirection from {m_CameraManager.requestedFacingDirection} to {newFacingDirection}");
            m_CameraManager.requestedFacingDirection = newFacingDirection;
        }
    }
}
