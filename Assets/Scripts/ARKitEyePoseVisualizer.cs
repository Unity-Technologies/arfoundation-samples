using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.XR.ARKit;
#endif

/// <summary>
/// Visualizes the eye poses for an <see cref="ARFace"/>.
/// </summary>
/// <remarks>
/// Face space is the space where the origin is the transform of an <see cref="ARFace"/>.
/// </remarks>
[RequireComponent(typeof(ARFace))]
public class ARKitEyePoseVisualizer : MonoBehaviour
{
    [SerializeField]
    GameObject m_EyePrefab;

    public GameObject eyePrefab
    {
        get => m_EyePrefab;
        set => m_EyePrefab = value;
    }

    GameObject m_LeftEyeGameObject;
    GameObject m_RightEyeGameObject;

    ARFace m_Face;
#if UNITY_IOS && !UNITY_EDITOR
    XRFaceSubsystem m_FaceSubsystem;
#endif

    void Awake()
    {
        m_Face = GetComponent<ARFace>();
        CreateEyeGameObjects();
    }

    void CreateEyeGameObjects()
    {
        m_LeftEyeGameObject = Instantiate(m_EyePrefab, m_Face.transform);
        m_RightEyeGameObject = Instantiate(m_EyePrefab, m_Face.transform);

        m_LeftEyeGameObject.SetActive(false);
        m_RightEyeGameObject.SetActive(false);
    }

    void SetVisible(bool visible)
    {
        m_LeftEyeGameObject.SetActive(visible);
        m_RightEyeGameObject.SetActive(visible);
    }

    void UpdateVisibility()
    {
        var visible =
            enabled &&
            (m_Face.trackingState == TrackingState.Tracking) &&
#if UNITY_IOS && !UNITY_EDITOR
            m_FaceSubsystem.supportedEyeTracking &&
#endif
            (ARSession.state > ARSessionState.Ready);

        SetVisible(visible);
    }

    void OnEnable()
    {
#if UNITY_IOS && !UNITY_EDITOR
        var faceManager = FindObjectOfType<ARFaceManager>();
        if (faceManager != null)
        {
            m_FaceSubsystem = (XRFaceSubsystem)faceManager.subsystem;
        }
#endif
        UpdateVisibility();
        m_Face.updated += OnUpdated;
    }

    void OnUpdated(ARFaceUpdatedEventArgs eventArgs)
    {
        UpdateVisibility();
        UpdateEyeGazeFeatures();
    }

    void UpdateEyeGazeFeatures()
    {
#if UNITY_IOS && !UNITY_EDITOR
        var leftEyePose = new Pose();
        var rightEyePose = new Pose();

        if (m_FaceSubsystem.supportedEyeTracking)
        {
            if (m_FaceSubsystem.TryGetLeftEyePose(m_Face.trackableId, ref leftEyePose) && m_FaceSubsystem.TryGetRightEyePose(m_Face.trackableId, ref rightEyePose))
            {
                m_LeftEyeGameObject.transform.localPosition = leftEyePose.position;
                m_LeftEyeGameObject.transform.localRotation = leftEyePose.rotation;
                m_RightEyeGameObject.transform.localPosition = rightEyePose.position;
                m_RightEyeGameObject.transform.localRotation = rightEyePose.rotation;
            }
            else
            {
                Debug.Log("Failed to get the face's eye poses.");
            }
        }
#endif
    }
}
