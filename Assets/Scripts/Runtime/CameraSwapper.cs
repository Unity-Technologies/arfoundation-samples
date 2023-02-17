namespace UnityEngine.XR.ARFoundation.Samples
{
    public class CameraSwapper : MonoBehaviour
    {
        [SerializeField]
        ARCameraManager m_CameraManager;

        /// <summary>
        /// The camera manager for switching the camera direction.
        /// </summary>
        public ARCameraManager cameraManager
        {
            get => m_CameraDirection.cameraManager;
            set => m_CameraDirection.cameraManager = value;
        }

        CameraDirection m_CameraDirection;

        void Awake()
        {
            m_CameraDirection = new CameraDirection(m_CameraManager);
        }

        public void SwapCamera()
        {
            m_CameraDirection.Toggle();
        }
    }
}
