using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class ThermalStateHandling : MonoBehaviour
    {
        /// <summary>
        /// The environment probe manager.
        /// </summary>
        AREnvironmentProbeManager m_EnvironmentProbeManager;

        /// <summary>
        /// The face manager.
        /// </summary>
        ARFaceManager m_FaceManager;

        /// <summary>
        /// The mesh manager.
        /// </summary>
        ARMeshManager m_MeshManager;

        /// <summary>
        /// The occlusion manager.
        /// </summary>
        AROcclusionManager m_OcclusionManager;

        /// <summary>
        /// The AR session.
        /// </summary>
        ARSession m_Session;

        /// <summary>
        /// The component from which to query the thermal state.
        /// </summary>
        public ThermalStateForIOS thermalStateForIOS
        {
            get => m_ThermalStateForIOS;
            set => m_ThermalStateForIOS = value;
        }

        [SerializeField]
        ThermalStateForIOS m_ThermalStateForIOS;

        /// <summary>
        /// The text UI to update with the current thermal state.
        /// </summary>
        public Text thermalStateText
        {
            get => m_ThermalStateText;
            set => m_ThermalStateText = value;
        }

        [SerializeField]
        public Text m_ThermalStateText;

        /// <summary>
        /// On awake, find all of the AR components.
        /// </summary>
        void Awake()
        {
#if UNITY_2023_1_OR_NEWER
            m_EnvironmentProbeManager = FindAnyObjectByType<AREnvironmentProbeManager>();
            m_FaceManager = FindAnyObjectByType<ARFaceManager>();
            m_MeshManager = FindAnyObjectByType<ARMeshManager>();
            m_OcclusionManager = FindAnyObjectByType<AROcclusionManager>();
            m_Session = FindAnyObjectByType<ARSession>();
#else
            m_EnvironmentProbeManager = FindObjectOfType<AREnvironmentProbeManager>();
            m_FaceManager = FindObjectOfType<ARFaceManager>();
            m_MeshManager = FindObjectOfType<ARMeshManager>();
            m_OcclusionManager = FindObjectOfType<AROcclusionManager>();
            m_Session = FindObjectOfType<ARSession>();
#endif
        }

        /// <summary>
        /// On enable, update the current thermal state text, set the AR features, and subscribe to the thermal state
        /// change event.
        /// </summary>
        void OnEnable()
        {
            Debug.Assert(m_ThermalStateForIOS != null, "ThermalStateForIOS cannot be null");
            Debug.Assert(m_ThermalStateText != null, "ThermalStateText cannot be null");

            ThermalStateForIOS.ThermalState thermalState = m_ThermalStateForIOS.currentThermalState;
            m_ThermalStateText.text = $"Thermal State: {thermalState}";

            SetARFeaturesBasedOnThermalState(thermalState);

            m_ThermalStateForIOS.stateChanged += OnThermalStateChanged;
        }

        /// <summary>
        /// On disable, update the current thermal state text, and unsubscribe to the thermal state change event.
        /// </summary>
        void OnDisable()
        {
            Debug.Assert(m_ThermalStateForIOS != null, "ThermalStateForIOS cannot be null");
            Debug.Assert(m_ThermalStateText != null, "ThermalStateText cannot be null");

            m_ThermalStateText.text = $"Thermal State: {ThermalStateForIOS.ThermalState.Unknown}";

            m_ThermalStateForIOS.stateChanged -= OnThermalStateChanged;
        }

        /// <summary>
        /// When the thermal state changes, update the thermal state text UI, and set the AR features based on the
        /// current thermal state.
        /// </summary>
        void OnThermalStateChanged(ThermalStateForIOS.ThermalStateChange thermalStateChangeArgs)
        {
            m_ThermalStateText.text = $"Thermal State: {thermalStateChangeArgs.currentThermalState}";
            SetARFeaturesBasedOnThermalState(thermalStateChangeArgs.currentThermalState);
        }

        /// <summary>
        /// Enable/disable AR features based on the thermal state in an effort to reduce power in elevated thermal
        /// states.
        /// </summary>
        /// <remarks>
        /// Note that these particular features for each thermal state are just for demo purposes only. Developers
        /// should decide which features are most critical for their own apps.
        /// </summary>
        void SetARFeaturesBasedOnThermalState(ThermalStateForIOS.ThermalState thermalState)
        {
            switch (thermalState)
            {
                case ThermalStateForIOS.ThermalState.Nominal:
                    ToggleEnvironmentProbeFeature(true);
                    ToggleFaceTrackingFeature(true);
                    ToggleMeshingFeature(true);
                    ToggleHumanSegmentationFeatures(true);
                    ToggleEnvironmentDepthFeature(true);
                    ToggleARSession(true);
                    break;
                case ThermalStateForIOS.ThermalState.Fair:
                    ToggleEnvironmentProbeFeature(false);
                    ToggleFaceTrackingFeature(false);
                    ToggleMeshingFeature(false);
                    ToggleHumanSegmentationFeatures(false);
                    ToggleEnvironmentDepthFeature(true);
                    ToggleARSession(true);
                    break;
                case ThermalStateForIOS.ThermalState.Serious:
                    ToggleEnvironmentProbeFeature(false);
                    ToggleFaceTrackingFeature(false);
                    ToggleMeshingFeature(false);
                    ToggleHumanSegmentationFeatures(false);
                    ToggleEnvironmentDepthFeature(false);
                    ToggleARSession(true);
                    break;
                case ThermalStateForIOS.ThermalState.Critical:
                    ToggleEnvironmentProbeFeature(false);
                    ToggleFaceTrackingFeature(false);
                    ToggleMeshingFeature(false);
                    ToggleHumanSegmentationFeatures(false);
                    ToggleEnvironmentDepthFeature(false);
                    ToggleARSession(false);
                    break;
                case ThermalStateForIOS.ThermalState.Unknown:
                default:
                    ToggleEnvironmentProbeFeature(true);
                    ToggleFaceTrackingFeature(true);
                    ToggleMeshingFeature(true);
                    ToggleHumanSegmentationFeatures(true);
                    ToggleEnvironmentDepthFeature(true);
                    ToggleARSession(true);
                    break;
            }
        }

        void ToggleEnvironmentProbeFeature(bool active)
        {
            Debug.Assert(m_EnvironmentProbeManager != null, "environment probe manager not found");
            m_EnvironmentProbeManager.enabled = active;
        }

        void ToggleFaceTrackingFeature(bool active)
        {
            Debug.Assert(m_FaceManager != null, "face manager not found");
            m_FaceManager.enabled = active;
        }

        void ToggleMeshingFeature(bool active)
        {
            Debug.Assert(m_MeshManager != null, "mesh manager not found");
            m_MeshManager.enabled = active;
        }

        void ToggleHumanSegmentationFeatures(bool active)
        {
            Debug.Assert(m_OcclusionManager != null, "occlusion manager not found");
            m_OcclusionManager.requestedHumanDepthMode = active ? HumanSegmentationDepthMode.Best : HumanSegmentationDepthMode.Disabled;
            m_OcclusionManager.requestedHumanStencilMode = active ? HumanSegmentationStencilMode.Best : HumanSegmentationStencilMode.Disabled;
        }

        void ToggleEnvironmentDepthFeature(bool active)
        {
            Debug.Assert(m_OcclusionManager != null, "occlusion manager not found");
            m_OcclusionManager.requestedEnvironmentDepthMode = active ? EnvironmentDepthMode.Best : EnvironmentDepthMode.Disabled;
        }

        void ToggleARSession(bool active)
        {
            Debug.Assert(m_MeshManager != null, "AR session not found");
            m_Session.enabled = active;
        }
    }
}
