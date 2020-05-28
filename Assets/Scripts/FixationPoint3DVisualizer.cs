using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
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
        GameObject m_FixationRayPrefab;

        public GameObject fixationRayPrefab
        {
            get => m_FixationRayPrefab;
            set => m_FixationRayPrefab = value;
        }

        GameObject m_FixationRayGameObject;

        ARFace m_Face;
        XRFaceSubsystem m_FaceSubsystem;

        void Awake()
        {
            m_Face = GetComponent<ARFace>();
        }

        void CreateEyeGameObjectsIfNecessary()
        {
            if (m_FixationRayGameObject == null && m_Face.fixationPoint != null)
            {
                m_FixationRayGameObject = Instantiate(m_FixationRayPrefab, m_Face.transform);
                m_FixationRayGameObject.SetActive(false);
            }
        }

        void SetVisible(bool visible)
        {
            if (m_FixationRayGameObject != null)
                m_FixationRayGameObject.SetActive(visible);
        }

        void OnEnable()
        {
            var faceManager = FindObjectOfType<ARFaceManager>();
            if (faceManager != null && faceManager.subsystem != null && faceManager.descriptor.supportsEyeTracking)
            {
                m_FaceSubsystem = (XRFaceSubsystem)faceManager.subsystem;
                m_Face.updated += OnUpdated;
            }
            else
            {
                enabled = false;
            }
        }

        void OnDisable()
        {
            m_Face.updated -= OnUpdated;
            SetVisible(false);
        }

        void OnUpdated(ARFaceUpdatedEventArgs eventArgs)
        {
            CreateEyeGameObjectsIfNecessary();
            SetVisible((m_Face.trackingState == TrackingState.Tracking) && (ARSession.state > ARSessionState.Ready));
            UpdateFixationPoint();
        }

        void UpdateFixationPoint()
        {
            if (m_FixationRayGameObject != null)
            {
                m_FixationRayGameObject.transform.LookAt(m_Face.fixationPoint.position);
            }
        }
    }
}