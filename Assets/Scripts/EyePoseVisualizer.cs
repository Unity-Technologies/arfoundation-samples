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
public class EyePoseVisualizer : MonoBehaviour
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
    XRFaceSubsystem m_FaceSubsystem;

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
            m_FaceSubsystem.SubsystemDescriptor.supportsEyeTracking &&
            (ARSession.state > ARSessionState.Ready);

        SetVisible(visible);
    }

    void OnEnable()
    {
        var faceManager = FindObjectOfType<ARFaceManager>();
        if (faceManager != null)
        {
            m_FaceSubsystem = (XRFaceSubsystem)faceManager.subsystem;
        }
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
        if (m_FaceSubsystem.SubsystemDescriptor.supportsEyeTracking)
        {
            m_LeftEyeGameObject.transform.localPosition = m_Face.leftEyePose.Value.position;
            m_LeftEyeGameObject.transform.localRotation = m_Face.leftEyePose.Value.rotation;
            m_RightEyeGameObject.transform.localPosition = m_Face.rightEyePose.Value.position;
            m_RightEyeGameObject.transform.localRotation = m_Face.rightEyePose.Value.rotation;
        }
        else
        {
            Debug.Log("This subsystem does not support eye tracking.");
        }
    }
}
