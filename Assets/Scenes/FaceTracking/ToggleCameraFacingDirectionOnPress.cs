namespace UnityEngine.XR.ARFoundation.Samples
{
    public class ToggleCameraFacingDirectionOnPress : PressInputBase
    {
        [SerializeField]
        ARCameraManager m_CameraManager;

        public ARCameraManager cameraManager
        {
            get => m_CameraDirection.cameraManager;
            set => m_CameraDirection.cameraManager = value;
        }

        CameraDirection m_CameraDirection;

        protected override void Awake()
        {
            base.Awake();
            m_CameraDirection = new CameraDirection(m_CameraManager);
        }

        protected override void OnPressBegan(Vector3 position)
        {
            m_CameraDirection.Toggle();
        }
    }
}
