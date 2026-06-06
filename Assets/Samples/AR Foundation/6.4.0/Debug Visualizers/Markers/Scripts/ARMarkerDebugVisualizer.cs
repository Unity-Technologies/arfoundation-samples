using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.PackageSamples.DebugVisualizers
{
    [RequireComponent(typeof(ARMarker))]
    [RequireComponent(typeof(MarkerDebugInfo))]
    public class ARMarkerDebugVisualizer : MonoBehaviour
    {
        static readonly int k_LineRendererColorPropertyId = Shader.PropertyToID("_Color");

        [SerializeField, ReadOnlyField]
        MarkerDebugInfo m_MarkerDebugInfo;

        [SerializeField, ReadOnlyField]
        ARMarker m_ARMarker;

        [Header("Prefabs")]
        [SerializeField, Tooltip("The prefab to visualize the plane orientation.")]
        GameObject m_TriAxisGizmoPrefab;

        [Header("Scene References")]
        [SerializeField]
        LineRenderer m_LineRenderer;

        [Header("Debug Options")]
        [SerializeField, Tooltip("Show plane normal visualizer.")]
        bool m_ShowMarkerNormal = true;

        [SerializeField, Tooltip("Show trackableId visualizer.")]
        bool m_ShowTrackableId;

        [SerializeField, Tooltip("Show tracking state visualizer.")]
        bool m_ShowTrackingState = true;

        [SerializeField, Tooltip("Show marker type visualizer.")]
        bool m_ShowMarkerType = true;

        [SerializeField, Tooltip("Show marker ID visualizer.")]
        bool m_ShowMarkerId = true;

        [SerializeField, Tooltip("Show marker encoded data buffer visualizer.")]
        bool m_ShowEncodedDataType = true;

        [SerializeField, Tooltip("The marker outline color when tracking state is tracking.")]
        Color m_TrackingOutlineColor = new(88f / 255f, 195f / 255f, 1, 1f);

        [SerializeField, Tooltip("The marker outline color when tracking state is limited.")]
        Color m_LimitedTrackingOutlineColor = new(246f / 255f, 205f / 255f, 105f / 255f, 1f);

        GameObject m_MarkerNormalVisualizer;
        TrackableId m_TrackableId;
        Pose m_Pose;
        uint m_MarkerId;
        XRMarkerType m_MarkerType;
        TrackingState m_TrackingState;
        XRSpatialBufferType m_SpatialBufferType;

        void Reset()
        {
            m_MarkerDebugInfo = GetComponent<MarkerDebugInfo>();
            m_LineRenderer = GetComponentInChildren<LineRenderer>();
            m_ARMarker = GetComponent<ARMarker>();
        }

        void Awake()
        {
            if (m_MarkerDebugInfo == null)
                m_MarkerDebugInfo = GetComponent<MarkerDebugInfo>();

            m_MarkerDebugInfo.marker = m_ARMarker;

            if (m_ARMarker == null)
                m_ARMarker = GetComponent<ARMarker>();

            if (m_ShowMarkerNormal && m_TriAxisGizmoPrefab == null)
            {
                Debug.LogWarning(
                    $"{nameof(m_ShowMarkerNormal)} is enabled but {nameof(m_TriAxisGizmoPrefab)} is not assigned. " +
                    $"To show the marker normal vector visualizer assign a prefab to the {nameof(m_TriAxisGizmoPrefab)} " +
                    "in the inspector.", this);
            }

            if (m_TriAxisGizmoPrefab != null)
            {
                m_MarkerNormalVisualizer = Instantiate(m_TriAxisGizmoPrefab, transform);
                m_MarkerNormalVisualizer.SetActive(false);
            }
        }

        void Start()
        {
            m_LineRenderer.positionCount = 4;
            m_LineRenderer.startWidth = 0.0025f;
            m_LineRenderer.endWidth = 0.0025f;
            m_LineRenderer.loop = true;

            // We can set these immediately because we know they are won't change
            m_TrackableId = m_ARMarker.trackableId;
            m_MarkerId = m_ARMarker.markerId;
            m_MarkerType = m_ARMarker.markerType;

            m_MarkerDebugInfo.ShowTrackableId(m_ShowTrackableId, m_TrackableId);
            m_MarkerDebugInfo.ShowMarkerType(m_ShowMarkerType, m_MarkerType);
            m_MarkerDebugInfo.ShowMarkerId(m_ShowMarkerId, m_MarkerId);
            m_MarkerDebugInfo.ShowEncodedDataType(m_ShowEncodedDataType, m_SpatialBufferType);

            UpdateEncodedData();
            m_MarkerDebugInfo.Refresh();
        }

        void Update()
        {
            UpdateTrackingState();
            UpdateEncodedData();
            UpdatePosition();
            UpdatePlaneNormal();
            UpdateOutlineSize();
        }

        void OnDestroy()
        {
            if (m_MarkerNormalVisualizer != null)
                Destroy(m_MarkerNormalVisualizer);
        }

        void UpdateTrackingState()
        {
            if (m_ARMarker.trackingState == m_TrackingState)
                return;

            m_TrackingState = m_ARMarker.trackingState;
            m_MarkerDebugInfo.ShowTrackingState(m_ShowTrackingState, m_TrackingState);
            UpdateOutlineColor();
        }

        void UpdateEncodedData()
        {
            if (m_SpatialBufferType is XRSpatialBufferType.String or XRSpatialBufferType.Uint8)
                return;

            m_SpatialBufferType = m_ARMarker.dataBuffer.bufferType;
            m_MarkerDebugInfo.ShowEncodedDataType(m_ShowEncodedDataType, m_SpatialBufferType);

            switch (m_SpatialBufferType)
            {
                case XRSpatialBufferType.String:
                    var stringResult = m_ARMarker.TryGetStringData();
                    m_MarkerDebugInfo.ShowEncodedStringData(true, stringResult.value);
                    break;
                case XRSpatialBufferType.Uint8:
                    var bytesResult = m_ARMarker.TryGetBytesData();
                    m_MarkerDebugInfo.ShowEncodedBytesData(true, bytesResult.value);
                    break;
                default:
                    m_MarkerDebugInfo.ShowEncodedStringData(true, $"<{m_SpatialBufferType.ToString()}>");
                    break;
            }

            m_MarkerDebugInfo.Refresh();
        }

        void UpdatePosition()
        {
            transform.position = m_ARMarker.pose.position;
            transform.rotation = m_ARMarker.pose.rotation;
        }

        void UpdatePlaneNormal()
        {
            if (m_MarkerNormalVisualizer != null &&
                m_ShowMarkerNormal != m_MarkerNormalVisualizer.activeSelf)
                m_MarkerNormalVisualizer.SetActive(m_ShowMarkerNormal);

            if (!m_ShowMarkerNormal || m_MarkerNormalVisualizer == null)
                return;

            m_MarkerNormalVisualizer.transform.position = m_ARMarker.pose.position;
            m_MarkerNormalVisualizer.transform.rotation = m_ARMarker.transform.rotation;
        }

        void UpdateOutlineSize()
        {
            var halfSize = m_ARMarker.size * 0.5f;

            var localPosition = new Vector3(-halfSize.x, 0, -halfSize.y);
            var worldPosition = transform.TransformPoint(localPosition);
            m_LineRenderer.SetPosition(0, worldPosition);

            localPosition = new Vector3(-halfSize.x, 0, halfSize.y);
            worldPosition = transform.TransformPoint(localPosition);
            m_LineRenderer.SetPosition(1, worldPosition);

            localPosition = new Vector3(halfSize.x, 0, halfSize.y);
            worldPosition = transform.TransformPoint(localPosition);
            m_LineRenderer.SetPosition(2, worldPosition);

            localPosition = new Vector3(halfSize.x, 0, -halfSize.y);
            worldPosition = transform.TransformPoint(localPosition);
            m_LineRenderer.SetPosition(3, worldPosition);
        }

        void UpdateOutlineColor()
        {
            var color = m_TrackingState == TrackingState.Tracking ? m_TrackingOutlineColor : m_LimitedTrackingOutlineColor;
            m_LineRenderer.material.SetColor(k_LineRendererColorPropertyId, color);
        }
    }
}
