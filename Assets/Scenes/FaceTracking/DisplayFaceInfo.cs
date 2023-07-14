using System.Text;
using Unity.Collections;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(ARFaceManager))]
    [RequireComponent(typeof(XROrigin))]
    public class DisplayFaceInfo : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The active AR Session")]
        ARSession m_Session;

        public ARSession session
        {
            get => m_Session;
            set => m_Session = value;
        }

        [SerializeField]
        [Tooltip("The Text to display the status and info regarding face tracking")]
        Text m_FaceInfoText;

        public Text faceInfoText
        {
            get => m_FaceInfoText;
            set => m_FaceInfoText = value;
        }

        [SerializeField]
        [Tooltip("The Text to display instructions to the user that face tracking is not supported")]
        Text m_InstructionsText;

        public Text instructionsText
        {
            get => m_InstructionsText;
            set => m_InstructionsText = value;
        }

        [SerializeField]
        [Tooltip("The GameObject to enable when face tracking is not available")]
        GameObject m_NotSupportedElement;

        public GameObject notSupportedElement
        {
            get => m_NotSupportedElement;
            set => m_NotSupportedElement = value;
        }

        [SerializeField]
        [Tooltip("An object whose rotation will be set according to the tracked face.")]
        Transform m_FaceControlledObject;

        public Transform faceControlledObject
        {
            get => m_FaceControlledObject;
            set => m_FaceControlledObject = value;
        }

        ARFaceManager m_FaceManager;

        ARCameraManager m_CameraManager;

        StringBuilder m_Info = new StringBuilder();

        bool m_FaceTrackingSupported;

        bool m_FaceTrackingWithWorldCameraSupported;

        void Awake()
        {
            if (m_Session == null)
#if UNITY_2023_1_OR_NEWER
                m_Session = FindAnyObjectByType<ARSession>();
#else
                m_Session = FindObjectOfType<ARSession>();
#endif

            m_FaceManager = GetComponent<ARFaceManager>();
            var camera = GetComponent<XROrigin>().Camera;
            m_CameraManager = camera ? camera.GetComponent<ARCameraManager>() : null;
        }

        void OnEnable()
        {
            Application.onBeforeRender += OnBeforeRender;

            // Detect face tracking with world-facing camera support
            var subsystem = m_Session ? m_Session.subsystem : null;
            if (subsystem != null)
            {
                var configs = subsystem.GetConfigurationDescriptors(Allocator.Temp);
                if (configs.IsCreated)
                {
                    using (configs)
                    {
                        foreach (var config in configs)
                        {
                            if (config.capabilities.All(Feature.FaceTracking))
                            {
                                m_FaceTrackingSupported = true;
                            }

                            if (config.capabilities.All(Feature.WorldFacingCamera | Feature.FaceTracking))
                            {
                                m_FaceTrackingWithWorldCameraSupported = true;
                            }
                        }
                    }
                }
            }
        }

        void OnDisable()
        {
            Application.onBeforeRender -= OnBeforeRender;
        }

        void OnBeforeRender()
        {
            if (m_FaceControlledObject == null)
                return;

            foreach (var face in m_FaceManager.trackables)
            {
                if (face.trackingState == TrackingState.Tracking)
                {
                    m_FaceControlledObject.transform.rotation = face.transform.rotation;
                    var camera = m_CameraManager.GetComponent<Camera>();
                    m_FaceControlledObject.transform.position = camera.transform.position + camera.transform.forward * 0.5f;
                }
            }
        }

        void Update()
        {
            m_Info.Clear();

            if (m_FaceManager.subsystem != null)
            {
                m_Info.Append($"Supported number of tracked faces: {m_FaceManager.supportedFaceCount}\n");
                m_Info.Append($"Max number of faces to track: {m_FaceManager.currentMaximumFaceCount}\n");
                m_Info.Append($"Number of tracked faces: {m_FaceManager.trackables.count}\n");
            }

            if (m_CameraManager)
            {
                m_Info.Append($"Requested camera facing direction: {m_CameraManager.requestedFacingDirection}\n");
                m_Info.Append($"Current camera facing direction: {m_CameraManager.currentFacingDirection}\n");
            }

            m_Info.Append($"Requested tracking mode: {m_Session.requestedTrackingMode}\n");
            m_Info.Append($"Current tracking mode: {m_Session.currentTrackingMode}\n");

            if (!m_FaceTrackingSupported)
            {
                if (m_InstructionsText)
                {
                    m_InstructionsText.text = "Face tracking is not supported.\n";
                }
                else
                {
                    m_Info.Append("Face tracking is not supported.\n");
                }
            }
            else if (m_CameraManager.requestedFacingDirection == CameraFacingDirection.World && !m_FaceTrackingWithWorldCameraSupported)
            {
                m_Info.Append("Face tracking in world facing camera mode is not supported.\n");
            }

            if (m_FaceControlledObject)
            {
                m_FaceControlledObject.gameObject.SetActive(m_CameraManager.currentFacingDirection == CameraFacingDirection.World);
            }

            if (m_NotSupportedElement)
            {
                m_NotSupportedElement.SetActive(m_CameraManager.requestedFacingDirection == CameraFacingDirection.World && !m_FaceTrackingWithWorldCameraSupported);
            }

            if (m_FaceInfoText)
            {
                m_FaceInfoText.text = m_Info.ToString();
            }
        }
    }
}
