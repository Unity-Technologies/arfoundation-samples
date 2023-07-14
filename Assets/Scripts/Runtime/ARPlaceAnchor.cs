using System.Collections.Generic;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class ARPlaceAnchor : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The enabled Anchor Manager in the scene.")]
        ARAnchorManager m_AnchorManager;

        [SerializeField]
        [Tooltip("The prefab to be instantiated for each anchor.")]
        GameObject m_Prefab;

        [SerializeField]
        [Tooltip("The Scriptable Object Asset that contains the ARRaycastHit event.")]
        ARRaycastHitEventAsset m_RaycastHitEvent;

        List<ARAnchor> m_Anchors = new();

        public ARAnchorManager anchorManager
        {
            get => m_AnchorManager;
            set => m_AnchorManager = value;
        }

        public GameObject prefab
        {
            get => m_Prefab;
            set => m_Prefab = value;
        }

        public void RemoveAllAnchors()
        {
            foreach (var anchor in m_Anchors)
            {
                Destroy(anchor.gameObject);
            }
            m_Anchors.Clear();
        }

        // Runs when the reset option is called in the context menu in-editor, or when first created.
        void Reset()
        {
            if (m_AnchorManager == null)
#if UNITY_2023_1_OR_NEWER
                m_AnchorManager = FindAnyObjectByType<ARAnchorManager>();
#else
                m_AnchorManager = FindObjectOfType<ARAnchorManager>();
#endif
        }

        void OnEnable()
        {
            if (m_AnchorManager == null)
#if UNITY_2023_1_OR_NEWER
                m_AnchorManager = FindAnyObjectByType<ARAnchorManager>();
#else
                m_AnchorManager = FindObjectOfType<ARAnchorManager>();
#endif

            if ((m_AnchorManager ? m_AnchorManager.subsystem : null) == null)
            {
                enabled = false;
                Debug.LogWarning($"No XRAnchorSubsystem was found in {nameof(ARPlaceAnchor)}'s {nameof(m_AnchorManager)}, so this script will be disabled.", this);
                return;
            }

            if (m_RaycastHitEvent == null)
            {
                enabled = false;
                Debug.LogWarning($"{nameof(m_RaycastHitEvent)} field on {nameof(ARPlaceAnchor)} component of {name} is not assigned.", this);
                return;
            }

            m_RaycastHitEvent.eventRaised += CreateAnchor;
        }

        void OnDisable()
        {
            if (m_RaycastHitEvent != null)
                m_RaycastHitEvent.eventRaised -= CreateAnchor;
        }

        void CreateAnchor(object sender, ARRaycastHit arRaycastHit)
        {
            ARAnchor anchor;

            // If we hit a plane, try to "attach" the anchor to the plane
            if (m_AnchorManager.descriptor.supportsTrackableAttachments && arRaycastHit.trackable is ARPlane plane)
            {
                if (m_Prefab != null)
                {
                    var oldPrefab = m_AnchorManager.anchorPrefab;
                    m_AnchorManager.anchorPrefab = m_Prefab;
                    anchor = m_AnchorManager.AttachAnchor(plane, arRaycastHit.pose);
                    m_AnchorManager.anchorPrefab = oldPrefab;
                }
                else
                {
                    anchor = m_AnchorManager.AttachAnchor(plane, arRaycastHit.pose);
                }

                FinalizePlacedAnchor(anchor, $"Attached to plane {plane.trackableId}");
                return;
            }

            // Otherwise, just create a regular anchor at the hit pose
            if (m_Prefab != null)
            {
                // Note: the anchor can be anywhere in the scene hierarchy
                var anchorPrefab = Instantiate(m_Prefab, arRaycastHit.pose.position, arRaycastHit.pose.rotation);
                anchor = ComponentUtils.GetOrAddIf<ARAnchor>(anchorPrefab, true);
            }
            else
            {
                var anchorPrefab = new GameObject("Anchor");
                anchorPrefab.transform.SetPositionAndRotation(arRaycastHit.pose.position, arRaycastHit.pose.rotation);
                anchor = anchorPrefab.AddComponent<ARAnchor>();
            }

            FinalizePlacedAnchor(anchor, $"Anchor (from {arRaycastHit.hitType})");
        }

        void FinalizePlacedAnchor(ARAnchor anchor, string text)
        {
            var canvasTextManager = anchor.GetComponent<CanvasTextManager>();
            if (canvasTextManager != null)
            {
                canvasTextManager.text = text;
            }
            m_Anchors.Add(anchor);
        }
    }
}
