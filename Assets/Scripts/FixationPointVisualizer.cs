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
		CreateEyeGameObjects();
	}

	void CreateEyeGameObjects()
	{
		m_FixationPointGameObject = Instantiate(m_FixationPointPrefab, m_Face.transform);
		m_FixationPointGameObject.SetActive(false);
	}

	void SetVisible(bool visible)
	{
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
		UpdateVisibility();
		UpdateFixationPoint();
	}

    void UpdateFixationPoint()
	{
        if (m_Face.fixationPoint != null)
        {
            // Often the gaze point will be the device (origin) or past the device so for demonstration
            // sake, we scale back the position to be closer (approx. 10cm) to the face and therefore visible.
            m_FixationPointGameObject.transform.localPosition = Vector3.Normalize(m_Face.fixationPoint.Value) / 10;
        }
        else
        {
            // Update onscreen text to show that eye tracking isn't supported
        }
	}

}
