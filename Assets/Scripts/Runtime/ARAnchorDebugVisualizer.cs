using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(ARAnchor))]
    [RequireComponent(typeof(DebugInfoDisplayController))]
    public class ARAnchorDebugVisualizer : MonoBehaviour
    {
        static readonly Vector3 k_CanvasVerticalOffset = new(0, 0.1f, 0);

        [Header("Debug Options")]
        [SerializeField, Tooltip("Show trackableId visualizer.")]
        bool m_ShowTrackableId;

        [SerializeField, Tooltip("Show sessionId visualizer.")]
        bool m_ShowSessionIdType = true;

        [SerializeField, Tooltip("Show tracking state visualizer.")]
        bool m_ShowTrackingState = true;

        [SerializeField, Tooltip("Show the hit type the raycast hit to place this anchor if a raycast was used.")]
        bool m_ShowRaycastTrackableHitType = true;

        [SerializeField, Tooltip("Show the trackable the anchor is attached to if the feature is supported.")]
        bool m_ShowTrackableAttachedTo = true;

        [SerializeField, HideInInspector]
        ARAnchor m_ARAnchor;

        [SerializeField, HideInInspector]
        DebugInfoDisplayController m_DebugInfoDisplayController;

        Guid m_CurrentSubsystemSessionId;
        public Guid CurrentSubsystemSessionId
        {
            get => m_CurrentSubsystemSessionId;
            set => m_CurrentSubsystemSessionId = value;
        }

        bool m_IsAnchorAttachedToTrackable;
        public bool IsAnchorAttachedToTrackable
        {
            get => m_IsAnchorAttachedToTrackable;
            set => m_IsAnchorAttachedToTrackable = value;
        }

        TrackableId m_TrackableId;
        Guid m_AnchorSessionId;
        Guid m_PreviousSubsystemSessionId;
        TrackingState m_TrackingState;
        bool m_IsPlacedWithRaycast;
        TrackableType m_RaycastTrackableHitType;

        /// <summary>
        /// Sets the flag for visualizing if the anchor was placed using an ARRaycast.
        /// </summary>
        /// <param name="isPlacedWithRaycast"><see langword="true"/> if the anchor was placed using a raycast, otherwise <see langword="false"/>.</param>
        /// <param name="trackableHitType">The trackable that was hit with a raycast
        /// if a raycast was used to place the anchor.</param>
        public void SetAnchorCreationMethod(bool isPlacedWithRaycast, TrackableType trackableHitType)
        {
            m_IsPlacedWithRaycast = isPlacedWithRaycast;
            m_RaycastTrackableHitType = trackableHitType;
        }

        void Reset()
        {
            m_ARAnchor = GetComponent<ARAnchor>();
            m_DebugInfoDisplayController = GetComponent<DebugInfoDisplayController>();
        }

        void Awake()
        {
            if (m_ARAnchor == null)
                m_ARAnchor = GetComponent<ARAnchor>();

            if (m_DebugInfoDisplayController == null)
                m_DebugInfoDisplayController = GetComponent<DebugInfoDisplayController>();

            if (!m_ShowTrackableId && !m_ShowSessionIdType && !m_ShowTrackingState && !m_ShowTrackableAttachedTo && !m_ShowRaycastTrackableHitType)
                m_DebugInfoDisplayController.Show(false);
        }

        void Start()
        {
            m_DebugInfoDisplayController.SetBottomPivot();
        }

        void Update()
        {
            UpdateDebugInfo();
        }

        void UpdateDebugInfo()
        {
            m_DebugInfoDisplayController.SetPosition(m_ARAnchor.transform.position + k_CanvasVerticalOffset);

            if (m_ARAnchor.trackableId == m_TrackableId &&
                m_ARAnchor.sessionId == m_AnchorSessionId &&
                m_CurrentSubsystemSessionId == m_PreviousSubsystemSessionId &&
                m_ARAnchor.trackingState == m_TrackingState)
                return;

            m_TrackableId = m_ARAnchor.trackableId;
            m_AnchorSessionId = m_ARAnchor.sessionId;
            m_PreviousSubsystemSessionId = m_CurrentSubsystemSessionId;
            m_TrackingState = m_ARAnchor.trackingState;

            if (m_ShowTrackableId)
                m_DebugInfoDisplayController.AppendDebugEntry("TrackableId:", m_TrackableId.ToString());

            if (m_ShowSessionIdType)
            {
                var sessionIdType = m_CurrentSubsystemSessionId == Guid.Empty ||
                    m_AnchorSessionId.Equals(m_CurrentSubsystemSessionId) ? "Local" : "Remote";
                m_DebugInfoDisplayController.AppendDebugEntry("SessionId Type:", sessionIdType);
            }

            if (m_ShowTrackingState)
                m_DebugInfoDisplayController.AppendDebugEntry("Tracking State:", m_TrackingState.ToString());

            if (m_ShowRaycastTrackableHitType)
            {
                if (m_IsPlacedWithRaycast)
                    m_DebugInfoDisplayController.AppendDebugEntry("Raycast Hit:", m_RaycastTrackableHitType.ToString());
                else
                    m_DebugInfoDisplayController.AppendDebugEntry("Raycast Hit:", "N/A");
            }

            if (m_ShowTrackableAttachedTo)
            {
                m_DebugInfoDisplayController.AppendDebugEntry("Attached:", m_IsAnchorAttachedToTrackable.ToString());
            }

            m_DebugInfoDisplayController.RefreshDisplayInfo();
        }
    }
}
