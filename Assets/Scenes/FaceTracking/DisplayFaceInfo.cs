using System.Text;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARSession))]
[RequireComponent(typeof(ARFaceManager))]
[RequireComponent(typeof(ARSessionOrigin))]
public class DisplayFaceInfo : MonoBehaviour
{
    [SerializeField]
    Text m_FaceInfoText;

    public Text faceInfoText
    {
        get => m_FaceInfoText;
        set => m_FaceInfoText = value;
    }

    [SerializeField]
    Text m_InstructionsText;

    public Text instructionsText
    {
        get => m_InstructionsText;
        set => m_InstructionsText = value;
    }

    ARSession m_Session;

    ARFaceManager m_FaceManager;

    ARCameraManager m_CameraManager;

    StringBuilder m_Info = new StringBuilder();

    bool m_FaceTrackingSupported;

    bool m_FaceTrackingWithWorldCameraSupported;

    void Awake()
    {
        m_FaceManager = GetComponent<ARFaceManager>();
        m_Session = GetComponent<ARSession>();
        m_CameraManager = GetComponent<ARSessionOrigin>().camera?.GetComponent<ARCameraManager>();
    }

    void OnEnable()
    {
        // Detect face tracking with world-facing camera support
        var subsystem = m_Session?.subsystem;
        if (subsystem != null)
        {
            var configs = subsystem.GetConfigurationDescriptors(Allocator.Temp);
            if (configs.IsCreated)
            {
                using (configs)
                {
                    foreach (var config in configs)
                    {
                        if (config.capabilities.All(Feature.FaceDetection))
                        {
                            m_FaceTrackingSupported = true;
                        }

                        if (config.capabilities.All(Feature.WorldFacingCamera | Feature.FaceDetection))
                        {
                            m_FaceTrackingWithWorldCameraSupported = true;
                        }
                    }
                }
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

        if (!m_FaceTrackingSupported)
        {
            if (m_InstructionsText)
            {
                m_InstructionsText.text = "Face tracking is not supported.";
            }
            else
            {
                m_Info.Append("Face tracking is not supported.");
            }
        }
        else if (m_CameraManager.requestedFacingDirection == CameraFacingDirection.World && !m_FaceTrackingWithWorldCameraSupported)
        {
            m_Info.Append("Face tracking in world facing camera mode is not supported.");
        }

        if (m_FaceInfoText)
        {
            m_FaceInfoText.text = m_Info.ToString();
        }
    }
}
