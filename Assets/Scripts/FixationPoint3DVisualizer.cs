using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Visualizes the eye gaze position in face space for an <see cref="ARFace"/>.
/// </summary>
/// <remarks>
/// Face space is the space where the origin is the transform of an <see cref="ARFace"/>.
/// </remarks>
[RequireComponent(typeof(ARFace))]
public class FixationPoint3DVisualizer : MonoBehaviour
{
    [SerializeField]
    GameObject m_FixationPointPrefab;

    public GameObject fixationPointPrefab
    {
        get => m_FixationPointPrefab;
        set => m_FixationPointPrefab = value;
    }

    GameObject m_FixationPointGameObject;

    ARFace m_Face;
    XRFaceSubsystem m_FaceSubsystem;

    void Awake()
    {
        m_Face = GetComponent<ARFace>();
    }

    void GetOrCreateEyeGameObjects()
    {
        if (m_FixationPointGameObject == null && m_Face.fixationPointTransform != null)
        {
            m_FixationPointGameObject = Instantiate(m_FixationPointPrefab, m_Face.transform);
            m_FixationPointGameObject.SetActive(false);
        }
    }

    void SetVisible(bool visible)
    {
        if (m_FixationPointGameObject != null)
            m_FixationPointGameObject.SetActive(visible);
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
        GetOrCreateEyeGameObjects();
        UpdateVisibility();
        UpdateFixationPoint();
    }

    void UpdateFixationPoint()
    {
        if (m_FixationPointGameObject != null)
        {
            m_FixationPointGameObject.transform.LookAt(m_Face.fixationPointTransform.position);
        }
    }

}
