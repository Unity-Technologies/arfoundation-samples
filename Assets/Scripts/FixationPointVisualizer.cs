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
public class FixationPointVisualizer : MonoBehaviour
{
    [SerializeField]
    GameObject m_FixationPointPrefab;

    [SerializeField]
    GameObject m_GUIFixationReticlePrefab;

    public GameObject fixationPointPrefab
    {
        get => m_FixationPointPrefab;
        set => m_FixationPointPrefab = value;
    }

    public GameObject fixationReticlePrefab
    {
        get => m_GUIFixationReticlePrefab;
        set => m_GUIFixationReticlePrefab = value;
    }

    GameObject m_FixationPointGameObject;
    GameObject m_FixationReticleGameObject;

    Canvas m_Canvas;
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
            m_FixationPointGameObject = Instantiate(m_FixationPointPrefab, m_Face.fixationPointTransform);
            m_FixationPointGameObject.SetActive(false);

            if (FindObjectOfType<Canvas>() != null && m_FixationReticleGameObject == null)
            {
                var canvas = FindObjectOfType<Canvas>();
                Debug.Log("Found a canvas");
                m_FixationReticleGameObject = Instantiate(m_GUIFixationReticlePrefab, canvas.transform);
            }
        }
    }

    void SetVisible(bool visible)
    {
        if (m_FixationPointGameObject != null)
            m_FixationPointGameObject.SetActive(visible);
        
        if (m_FixationReticleGameObject != null)
            m_FixationReticleGameObject.SetActive(visible);
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
        UpdateScreenReticle();
    }

    void UpdateFixationPoint()
    {
        if (m_FixationPointGameObject != null)
        {
            // This is needed to update the rotation to better match the where of the fixation point
            // Debug.Log("");
        }
    }

    void UpdateScreenReticle()
    {
        var mainCamera = Camera.main;

        var fixationInViewSpace = mainCamera.WorldToViewportPoint(m_Face.fixationPointTransform.position);
        var mirrorFixationInView = new Vector3(1 - fixationInViewSpace.x, 1 - fixationInViewSpace.y, fixationInViewSpace.z);

        if (m_FixationReticleGameObject != null)
        {
            m_FixationReticleGameObject.GetComponent<RectTransform>().anchoredPosition3D = mainCamera.ViewportToScreenPoint(mirrorFixationInView);
        }
    }

}
