using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.XR.ARKit;
#endif // UNITY_IOS && !UNITY_EDITOR

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class FeaturesReporting : MonoBehaviour
    {
        public Text autoFocusText
        {
            get => m_AutoFocusText;
            set => m_AutoFocusText = value;
        }

        [SerializeField]
        Text m_AutoFocusText;

        public Text environmentDepthText
        {
            get => m_EnvironmentDepthText;
            set => m_EnvironmentDepthText = value;
        }

        [SerializeField]
        Text m_EnvironmentDepthText;

        public Text environmentProbesText
        {
            get => m_EnvironmentProbesText;
            set => m_EnvironmentProbesText = value;
        }

        [SerializeField]
        Text m_EnvironmentProbesText;

        public Text faceTrackingText
        {
            get => m_FaceTrackingText;
            set => m_FaceTrackingText = value;
        }

        [SerializeField]
        Text m_FaceTrackingText;

        public Text humanDepthText
        {
            get => m_HumanDepthText;
            set => m_HumanDepthText = value;
        }

        [SerializeField]
        Text m_HumanDepthText;

        public Text humanStencilText
        {
            get => m_HumanStencilText;
            set => m_HumanStencilText = value;
        }

        [SerializeField]
        Text m_HumanStencilText;

        public Text lightEstimationColorText
        {
            get => m_LightEstimationColorText;
            set => m_LightEstimationColorText = value;
        }

        [SerializeField]
        Text m_LightEstimationColorText;

        public Text lightEstimationIntensityText
        {
            get => m_LightEstimationIntensityText;
            set => m_LightEstimationIntensityText = value;
        }

        [SerializeField]
        Text m_LightEstimationIntensityText;

        public Text meshingText
        {
            get => m_MeshingText;
            set => m_MeshingText = value;
        }

        [SerializeField]
        Text m_MeshingText;

        public Text meshingClassificationText
        {
            get => m_MeshingClassificationText;
            set => m_MeshingClassificationText = value;
        }

        [SerializeField]
        Text m_MeshingClassificationText;

        public Text planeTrackingText
        {
            get => m_PlaneTrackingText;
            set => m_PlaneTrackingText = value;
        }

        [SerializeField]
        Text m_PlaneTrackingText;

        public Text raycastingText
        {
            get => m_RaycastingText;
            set => m_RaycastingText = value;
        }

        [SerializeField]
        Text m_RaycastingText;

        public Text rotationAndOrientationText
        {
            get => m_RotationAndOrientationText;
            set => m_RotationAndOrientationText = value;
        }

        [SerializeField]
        Text m_RotationAndOrientationText;

        public Text worldFacingCameraText
        {
            get => m_WorldFacingCameraText;
            set => m_WorldFacingCameraText = value;
        }

        [SerializeField]
        Text m_WorldFacingCameraText;

        public Text sessionFpsText
        {
            get => m_SessionFpsText;
            set => m_SessionFpsText = value;
        }

        [SerializeField]
        Text m_SessionFpsText;

        ARCameraManager m_CameraManager;
        AREnvironmentProbeManager m_EnvironmentProbeManager;
        ARFaceManager m_FaceManager;
        ARMeshManager m_MeshManager;
        AROcclusionManager m_OcclusionManager;
        ARPlaneManager m_PlaneManager;
        ARRaycastManager m_RaycastManager;
        ARSession m_Session;

        void Awake()
        {
            m_CameraManager = FindObjectOfType<ARCameraManager>();
            m_EnvironmentProbeManager = FindObjectOfType<AREnvironmentProbeManager>();
            m_FaceManager = FindObjectOfType<ARFaceManager>();
            m_MeshManager = FindObjectOfType<ARMeshManager>();
            m_OcclusionManager = FindObjectOfType<AROcclusionManager>();
            m_PlaneManager = FindObjectOfType<ARPlaneManager>();
            m_RaycastManager = FindObjectOfType<ARRaycastManager>();
            m_Session = FindObjectOfType<ARSession>();
        }

        void Update()
        {
            bool isARSessionEnabled = m_Session && (m_Session.subsystem?.running ?? false);
            bool isCameraManagerEnabled = isARSessionEnabled && m_CameraManager && (m_CameraManager.subsystem?.running ?? false);
            bool isEnvironmentProbeManagerEnabled = isARSessionEnabled && m_EnvironmentProbeManager && (m_EnvironmentProbeManager.subsystem?.running ?? false);
            bool isFaceManagerEnabled = isARSessionEnabled && m_FaceManager && (m_FaceManager.subsystem?.running ?? false);
            bool isMeshManagerEnabled = isARSessionEnabled && m_MeshManager && (m_MeshManager.subsystem?.running ?? false);
            bool isOcclusionManagerEnabled = isARSessionEnabled && m_OcclusionManager && (m_OcclusionManager.subsystem?.running ?? false);
            bool isPlaneManagerEnabled = isARSessionEnabled && m_PlaneManager && (m_PlaneManager.subsystem?.running ?? false);
            bool isRaycastManagerEnabled = isARSessionEnabled && m_RaycastManager && (m_RaycastManager.subsystem?.running ?? false);

            SetFeatureDisplayState(m_AutoFocusText, isCameraManagerEnabled && m_CameraManager.autoFocusEnabled);
            SetFeatureDisplayState(m_EnvironmentDepthText, isOcclusionManagerEnabled && m_OcclusionManager.currentEnvironmentDepthMode.Enabled());
            SetFeatureDisplayState(m_EnvironmentProbesText, isEnvironmentProbeManagerEnabled);
            SetFeatureDisplayState(m_FaceTrackingText, isFaceManagerEnabled && (m_FaceManager.supportedFaceCount > 0));
            SetFeatureDisplayState(m_HumanDepthText, isOcclusionManagerEnabled && m_OcclusionManager.currentHumanDepthMode.Enabled());
            SetFeatureDisplayState(m_HumanStencilText, isOcclusionManagerEnabled && m_OcclusionManager.currentHumanStencilMode.Enabled());
            SetFeatureDisplayState(m_LightEstimationColorText, isCameraManagerEnabled && ((m_CameraManager.currentLightEstimation & LightEstimation.AmbientColor) == LightEstimation.AmbientColor));
            SetFeatureDisplayState(m_LightEstimationIntensityText, isCameraManagerEnabled && ((m_CameraManager.currentLightEstimation & LightEstimation.AmbientIntensity) == LightEstimation.AmbientIntensity));
            SetFeatureDisplayState(m_MeshingText, isMeshManagerEnabled);
#if UNITY_IOS && !UNITY_EDITOR
            {
                XRMeshSubsystem meshSubsystem = m_MeshManager?.subsystem as XRMeshSubsystem;
                SetFeatureDisplayState(m_MeshingClassificationText, isMeshManagerEnabled && meshSubsystem.GetClassificationEnabled());
            }
#else // UNITY_IOS && !UNITY_EDITOR
            SetFeatureDisplayState(m_MeshingClassificationText, false);
#endif // UNITY_IOS && !UNITY_EDITOR
            SetFeatureDisplayState(m_PlaneTrackingText, isPlaneManagerEnabled && (m_PlaneManager.currentDetectionMode != PlaneDetectionMode.None));
            SetFeatureDisplayState(m_RaycastingText, isRaycastManagerEnabled);
            SetFeatureDisplayState(m_RotationAndOrientationText, isARSessionEnabled && (m_Session.currentTrackingMode == TrackingMode.PositionAndRotation));
            SetFeatureDisplayState(m_WorldFacingCameraText, isCameraManagerEnabled && (m_CameraManager.currentFacingDirection == CameraFacingDirection.World));
            SetFeatureDisplayState(m_SessionFpsText, isARSessionEnabled);

            int arFrameRate = isARSessionEnabled ? (m_Session.frameRate ?? 0) : 0;
            m_SessionFpsText.text = $"AR Session FPS: {arFrameRate}";
        }

        void SetFeatureDisplayState(Text textUI, bool isActive)
        {
            textUI.color = isActive ? Color.green : Color.red;
        }
    }
}
