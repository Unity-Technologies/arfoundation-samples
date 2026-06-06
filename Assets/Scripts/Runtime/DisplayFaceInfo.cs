using System.Text;
using Unity.Collections;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.OpenXR;
#if ANDROIDOPENXR_1_3_0_1_OR_NEWER && UNITY_ANDROID
using UnityEngine.XR.OpenXR.Features.Android;
#endif // ANDROIDOPENXR_1_3_0_1_OR_NEWER && UNITY_ANDROID
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

#if ANDROIDOPENXR_1_3_0_1_OR_NEWER && UNITY_ANDROID
        AndroidOpenXRFaceSubsystem m_AndroidOpenXRFaceSubsystem;
#endif // ANDROIDOPENXR_1_3_0_1_OR_NEWER && UNITY_ANDROID

        ARCameraManager m_CameraManager;

        StringBuilder m_Info = new StringBuilder();

        bool m_FaceTrackingSupported;

        bool m_FaceTrackingWithWorldCameraSupported;

        void Awake()
        {
            if (m_Session == null)
                m_Session = FindAnyObjectByType<ARSession>();

            m_FaceManager = GetComponent<ARFaceManager>();
            var camera = GetComponent<XROrigin>().Camera;
            m_CameraManager = camera ? camera.GetComponent<ARCameraManager>() : null;
        }

        void OnEnable()
        {
#pragma warning disable UDR0005 // Subscribing in OnEnable and unsubscribing in OnDisable is valid
            Application.onBeforeRender += OnBeforeRender;
#pragma warning restore UDR0005

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

#if ANDROIDOPENXR_1_3_0_1_OR_NEWER && UNITY_ANDROID
            m_AndroidOpenXRFaceSubsystem = m_FaceManager.subsystem as AndroidOpenXRFaceSubsystem;
            if (m_AndroidOpenXRFaceSubsystem == null)
            {
                Debug.LogError(
$"No " + nameof(AndroidOpenXRFaceSubsystem) + " is loaded. Will be unable to check face calibration state.", this);
            }
#endif // ANDROIDOPENXR_1_3_0_1_OR_NEWER && UNITY_ANDROID
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
#if ANDROIDOPENXR_1_3_0_1_OR_NEWER && UNITY_ANDROID
                if(m_AndroidOpenXRFaceSubsystem != null)
                {
                    var result = m_AndroidOpenXRFaceSubsystem.TryGetIsFaceTrackingCalibrated();
                    m_Info.Append("Calibration: Check status - ")
                          .Append(result.status)
                          .Append(" | Check value - ")
                          .Append(result.value ? "Calibrated" : "Uncalibrated")
                          .AppendLine();
                }
#endif // ANDROIDOPENXR_1_3_0_1_OR_NEWER && UNITY_ANDROID

                m_Info.Append("Supported number of tracked faces: ");
                m_Info.Append(m_FaceManager.supportedFaceCount);
                m_Info.AppendLine();

                m_Info.Append("Max number of faces to track: ");
                m_Info.Append(m_FaceManager.currentMaximumFaceCount);
                m_Info.AppendLine();

                m_Info.Append("Number of tracked faces: ");
                m_Info.Append(m_FaceManager.trackables);
                m_Info.AppendLine();

            }

            if (m_CameraManager)
            {
                m_Info.Append("Requested camera facing direction: ");
                m_Info.Append(m_CameraManager.requestedFacingDirection);
                m_Info.AppendLine();

                m_Info.Append("Current camera facing direction: ");
                m_Info.Append(m_CameraManager.currentFacingDirection);
                m_Info.AppendLine();

            }

            m_Info.Append("Requested tracking mode: ");
            m_Info.Append(m_Session.requestedTrackingMode);
            m_Info.AppendLine();

            m_Info.Append("Current tracking mode: ");
            m_Info.Append(m_Session.currentTrackingMode);
            m_Info.AppendLine();


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
