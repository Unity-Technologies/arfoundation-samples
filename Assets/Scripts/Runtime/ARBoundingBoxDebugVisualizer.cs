using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(ARBoundingBox))]
    [RequireComponent(typeof(DebugInfoDisplayController))]
    [RequireComponent(typeof(BoundingBoxEdgeVisualizer))]
    public class ARBoundingBoxDebugVisualizer : MonoBehaviour
    {
        static readonly Vector3 k_CanvasVerticalOffset = new(0, 0.15f, 0);

        [Header("References")]
        [SerializeField, Tooltip("The prefab to visualize the bounding box's orientation.")]
        GameObject m_OrientationVisualizerPrefab;

        [FormerlySerializedAs("m_BoundingBoxMeshRenderer")]
        [FormerlySerializedAs("m_BoundingBoxVisualizer")]
        [SerializeField, Tooltip("The mesh renderer of the bounding box visualizer.")]
        MeshRenderer m_MeshRenderer;

        [FormerlySerializedAs("m_TmpOverlayShader")]
        [SerializeField, Tooltip("The Text Mesh Pro material for drawing overlay text to always render debug text info on top of Geometry.")]
        Material m_TmpOverlayMaterial;

        [Header("Debug Options")]
        [SerializeField, Tooltip("Show the trackableId visualizer.")]
        bool m_ShowTrackableId;

        [SerializeField, Tooltip("Show the bounding box position and orientation visualizer.")]
        bool m_ShowOrientation = true;

        [SerializeField, Tooltip("Show the classifications visualizer.")]
        bool m_ShowClassifications = true;

        [SerializeField, Tooltip("Show the tracking state visualizer.")]
        bool m_ShowTrackingState = true;

        [Header("Tracking state visualization settings")]

        [SerializeField, Tooltip("The mesh color when the tracking state is set to tracking")]
        Color m_TrackingMeshColor;

        [SerializeField, Tooltip("The outline color gradient when the tracking state is set to tracking.")]
        Gradient m_TrackingOutlineGradient;

        [Space]

        [SerializeField, Tooltip("The outline color gradient when the tracking state is set to limited.")]
        Gradient m_LimitedTrackingOutlineGradient;

        [Space]

        [SerializeField, Tooltip("The texture the bounding box will have when the tracking state is set to none.")]
        Texture m_NoneTrackingTexture;

        [SerializeField, Tooltip("The mesh color when the tracking state is set to none.")]
        Color m_NoneTrackingMeshColor;

        [SerializeField, Tooltip("The mesh texture color when the tracking state is set to none.")]
        Color m_NoneTrackingMeshTextureColor;

        [SerializeField, Tooltip("The outline color gradient when the tracking state is set to none.")]
        Gradient m_NoneTrackingOutlineGradient;

        [SerializeField, HideInInspector]
        BoundingBoxEdgeVisualizer m_BoundingBoxEdgeVisualizer;

        [SerializeField, HideInInspector]
        DebugInfoDisplayController m_DebugInfoDisplayController;

        ARBoundingBox m_ARBoundingBox;
        GameObject m_OrientationVisualizer;
        TrackableId m_TrackableId;
        BoundingBoxClassifications m_Classifications;
        TrackingState m_TrackingState;

        void Reset()
        {
            m_BoundingBoxEdgeVisualizer = GetComponent<BoundingBoxEdgeVisualizer>();
            m_DebugInfoDisplayController = GetComponent<DebugInfoDisplayController>();
            m_ARBoundingBox = GetComponent<ARBoundingBox>();
        }

        void Awake()
        {
            if (m_BoundingBoxEdgeVisualizer == null)
                m_BoundingBoxEdgeVisualizer = GetComponent<BoundingBoxEdgeVisualizer>();

            if (m_DebugInfoDisplayController == null)
                m_DebugInfoDisplayController = GetComponent<DebugInfoDisplayController>();

            if (!m_ShowClassifications && !m_ShowTrackableId && !m_ShowTrackingState)
                m_DebugInfoDisplayController.Show(false);

            if (m_ARBoundingBox == null)
                m_ARBoundingBox = GetComponent<ARBoundingBox>();

            if (m_MeshRenderer == null)
                Debug.LogError($"{nameof(m_MeshRenderer)} is null.");

            if (m_ShowOrientation && m_OrientationVisualizerPrefab == null)
            {
                Debug.LogWarning($"{nameof(m_ShowOrientation)} is enabled but {nameof(m_OrientationVisualizerPrefab)} is not assigned. To show the bounding box orientation visualizer assign a prefab to the {nameof(m_OrientationVisualizerPrefab)} in the inspector.", this);
            }

            if (m_OrientationVisualizerPrefab != null)
            {
                m_OrientationVisualizer = Instantiate(m_OrientationVisualizerPrefab, transform);
                m_OrientationVisualizer.SetActive(false);
            }
        }

        void Start()
        {
            if (m_ShowOrientation)
                m_DebugInfoDisplayController.SetBottomPivot();
            else
                m_DebugInfoDisplayController.SetCenterPivot();
        }

        void OnDestroy()
        {
            if (m_OrientationVisualizer != null)
                Destroy(m_OrientationVisualizer);
        }

        void Update()
        {
            UpdateDebugInfo();
            UpdateVisualizers();
        }

        void UpdateDebugInfo()
        {
            var canvasPosition = m_ARBoundingBox.transform.position;
            canvasPosition += m_ShowOrientation ? k_CanvasVerticalOffset : Vector3.zero;
            m_DebugInfoDisplayController.SetPosition(canvasPosition);

            if (m_ARBoundingBox.trackableId == m_TrackableId &&
                m_ARBoundingBox.classifications == m_Classifications &&
                m_ARBoundingBox.trackingState == m_TrackingState)
                return;

            m_TrackableId = m_ARBoundingBox.trackableId;
            m_Classifications = m_ARBoundingBox.classifications;
            m_TrackingState = m_ARBoundingBox.trackingState;
            UpdateTrackingStateVisualization(m_TrackingState);

            if (m_ShowTrackableId)
                m_DebugInfoDisplayController.AppendDebugEntry("TrackableId:", m_TrackableId.ToString());

            if (m_ShowClassifications)
                m_DebugInfoDisplayController.AppendDebugEntry("Classifications:", m_Classifications.ToString());

            if (m_ShowTrackingState)
                m_DebugInfoDisplayController.AppendDebugEntry("Tracking State:", m_TrackingState.ToString());

            m_DebugInfoDisplayController.RefreshDisplayInfo();
        }

        void UpdateVisualizers()
        {
            UpdateVisualizersEnabledState();
            UpdateVisualizerTransforms();
        }

        void UpdateVisualizersEnabledState()
        {
            if (m_OrientationVisualizer != null && m_ShowOrientation != m_OrientationVisualizer.activeSelf)
            {
                m_OrientationVisualizer.SetActive(m_ShowOrientation);
            }
        }

        void UpdateVisualizerTransforms()
        {
            if (m_MeshRenderer != null)
            {
                m_MeshRenderer.transform.localScale = m_ARBoundingBox.size;
            }

            if (m_ShowOrientation && m_OrientationVisualizer != null)
            {
                var boundingBoxPose = m_ARBoundingBox.pose;
                m_OrientationVisualizer.transform.position = boundingBoxPose.position;
                m_OrientationVisualizer.transform.rotation = boundingBoxPose.rotation;
            }
        }

        void UpdateTrackingStateVisualization(TrackingState trackingState)
        {
            switch (trackingState)
            {
                case TrackingState.Tracking:
                    m_MeshRenderer.material.SetTexture("_MainTex", default);
                    m_MeshRenderer.material.mainTextureScale = new(1, 1);
                    m_MeshRenderer.material.SetColor("_Color", m_TrackingMeshColor);
                    m_MeshRenderer.material.SetColor("_TexColorTint", m_TrackingMeshColor);
                    m_BoundingBoxEdgeVisualizer.SetGradient(m_TrackingOutlineGradient);
                    break;
                case TrackingState.Limited:
                    m_MeshRenderer.material.SetTexture("_MainTex", default);
                    m_MeshRenderer.material.mainTextureScale = new(1, 1);
                    m_MeshRenderer.material.SetColor("_Color", m_TrackingMeshColor);
                    m_MeshRenderer.material.SetColor("_TexColorTint", m_TrackingMeshColor);
                    m_BoundingBoxEdgeVisualizer.SetGradient(m_TrackingOutlineGradient);
                    break;
                case TrackingState.None:
                    m_MeshRenderer.material.SetTexture("_MainTex", m_NoneTrackingTexture);
                    m_MeshRenderer.material.mainTextureScale = new(2, 2);
                    m_MeshRenderer.material.SetColor("_Color", m_NoneTrackingMeshColor);
                    m_MeshRenderer.material.SetColor("_TexColorTint", m_NoneTrackingMeshTextureColor);
                    m_BoundingBoxEdgeVisualizer.SetGradient(m_NoneTrackingOutlineGradient);
                    break;
            }
        }
    }
}
