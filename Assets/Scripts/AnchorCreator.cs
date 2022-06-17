using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(ARAnchorManager))]
    [RequireComponent(typeof(ARRaycastManager))]
    public class AnchorCreator : PressInputBase
    {
        [SerializeField]
        GameObject m_Prefab;

        public GameObject prefab
        {
            get => m_Prefab;
            set => m_Prefab = value;
        }

        public void RemoveAllAnchors()
        {
            Logger.Log($"Removing all anchors ({m_Anchors.Count})");
            foreach (var anchor in m_Anchors)
            {
                Destroy(anchor.gameObject);
            }
            m_Anchors.Clear();
        }

        protected override void Awake()
        {
            base.Awake();
            m_RaycastManager = GetComponent<ARRaycastManager>();
            m_AnchorManager = GetComponent<ARAnchorManager>();

            if (m_AnchorManager.subsystem == null)
            {
                enabled = false;
                Debug.LogWarning($"No active XRAnchorSubsystem is available, so {typeof(AnchorCreator).FullName} will not be enabled.");
            }
        }

        void SetAnchorText(ARAnchor anchor, string text)
        {
            var canvasTextManager = anchor.GetComponent<CanvasTextManager>();
            if (canvasTextManager)
            {
                canvasTextManager.text = text;
            }
        }

        ARAnchor CreateAnchor(in ARRaycastHit hit)
        {
            ARAnchor anchor = null;

            // If we hit a plane, try to "attach" the anchor to the plane
            if (hit.trackable is ARPlane plane)
            {
                var planeManager = GetComponent<ARPlaneManager>();
                if (planeManager != null)
                {
                    Logger.Log("Creating anchor attachment.");

                    if (m_Prefab != null)
                    {
                        var oldPrefab = m_AnchorManager.anchorPrefab;
                        m_AnchorManager.anchorPrefab = m_Prefab;
                        anchor = m_AnchorManager.AttachAnchor(plane, hit.pose);
                        m_AnchorManager.anchorPrefab = oldPrefab;
                    }
                    else
                    {
                        anchor = m_AnchorManager.AttachAnchor(plane, hit.pose);
                    }

                    SetAnchorText(anchor, $"Attached to plane {plane.trackableId}");
                    return anchor;
                }
            }

            // Otherwise, just create a regular anchor at the hit pose
            Logger.Log("Creating regular anchor.");

            if (m_Prefab != null)
            {
                // Note: the anchor can be anywhere in the scene hierarchy
                var gameObject = Instantiate(m_Prefab, hit.pose.position, hit.pose.rotation);

                // Make sure the new GameObject has an ARAnchor component
                anchor = ComponentUtils.GetOrAddIf<ARAnchor>(gameObject, true);
            }
            else
            {
                var gameObject = new GameObject("Anchor");
                gameObject.transform.SetPositionAndRotation(hit.pose.position, hit.pose.rotation);
                anchor = gameObject.AddComponent<ARAnchor>();
            }

            SetAnchorText(anchor, $"Anchor (from {hit.hitType})");

            return anchor;
        }

        protected override void OnPress(Vector3 position)
        {
            // Raycast against planes and feature points
            const TrackableType trackableTypes =
                TrackableType.FeaturePoint |
                TrackableType.PlaneWithinPolygon;

            // Perform the raycast
            if (m_RaycastManager.Raycast(position, s_Hits, trackableTypes))
            {
                // Raycast hits are sorted by distance, so the first one will be the closest hit.
                var hit = s_Hits[0];

                // Create a new anchor
                var anchor = CreateAnchor(hit);
                if (anchor != null)
                {
                    // Remember the anchor so we can remove it later.
                    m_Anchors.Add(anchor);
                }
                else
                {
                    Logger.Log("Error creating anchor");
                }
            }
        }

        static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        List<ARAnchor> m_Anchors = new List<ARAnchor>();

        ARRaycastManager m_RaycastManager;

        ARAnchorManager m_AnchorManager;
    }
}
