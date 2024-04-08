using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    [RequireComponent(typeof(ARPlaneMeshVisualizer))]
    [RequireComponent(typeof(ARPlane))]
    [RequireComponent(typeof(DebugInfoDisplayController))]
    public class ARPlaneDebugVisualizer : MonoBehaviour
    {
        static readonly Vector3 k_CanvasVerticalOffset = new(0, 0.15f, 0);

        [Header("References")]
        [SerializeField, Tooltip("The prefab to visualize the plane orientation.")]
        GameObject m_PlaneNormalPrefab;

        [SerializeField, Tooltip("Material used for planes that also have bounding boxes " +
             "to handle z fighting visual artifacts.")]
        Material m_SpecialPlaneMaterial;

        [SerializeField]
        LineRenderer m_Outline;

        [Header("Debug Options")]
        [SerializeField, Tooltip("Show plane normal visualizer.")]
        bool m_ShowPlaneNormal = true;

        [SerializeField, Tooltip("Show trackableId visualizer.")]
        bool m_ShowTrackableId = true;

        [SerializeField, Tooltip("Show classifications visualizer.")]
        bool m_ShowClassifications = true;

        [SerializeField, Tooltip("Show alignment visualizer.")]
        bool m_ShowAlignment = true;

        [SerializeField, Tooltip("Show tracking state visualizer.")]
        bool m_ShowTrackingState = true;

        [Header("Tracking state visualization settings")]
        [SerializeField, Tooltip("The texture the plane will have when the tracking state is set to tracking.")]
        Texture m_TrackingTexture;

        [SerializeField, Tooltip("The mesh color used for planes when the tracking state is set to tracking")]
        Color m_TrackingMeshColor;

        [SerializeField, Tooltip("The outline color gradient when the tracking state is set to tracking.")]
        Gradient m_TrackingOutlineGradient;

        [Space]

        [SerializeField, Tooltip("The outline color gradient when the tracking state is set to limited.")]
        Gradient m_LimitedTrackingOutlineGradient;

        [Space]

        [SerializeField, Tooltip("The texture the plane will have when the tracking state is set to none.")]
        Texture m_NoneTrackingTexture;

        [SerializeField, Tooltip("The mesh color used for planes when the tracking state is set to none.")]
        Color m_NoneTrackingMeshColor;
        
        [SerializeField, Tooltip("The outline color gradient when the tracking state is set to none.")]
        Gradient m_NoneTrackingOutlineGradient;

        [SerializeField, HideInInspector]
        ARPlaneMeshVisualizer m_ARPlaneMeshVisualizer;

        [SerializeField, HideInInspector]
        DebugInfoDisplayController m_DebugInfoDisplayController;

        [SerializeField, HideInInspector]
        ARPlane m_ARPlane;

        [SerializeField, HideInInspector]
        MeshRenderer m_MeshRenderer;

        public ARPlaneMeshVisualizer arPlaneMeshVisualizer => m_ARPlaneMeshVisualizer;

        GameObject m_PlaneNormalVisualizer;
        TrackableId m_TrackableId;
        PlaneClassifications m_Classifications;
        PlaneAlignment m_Alignment;
        TrackingState m_TrackingState;

        void Reset()
        {
            m_ARPlaneMeshVisualizer = GetComponent<ARPlaneMeshVisualizer>();
            m_DebugInfoDisplayController = GetComponent<DebugInfoDisplayController>();
            m_ARPlane = GetComponent<ARPlane>();
            m_MeshRenderer = GetComponent<MeshRenderer>();
        }

        void Awake()
        {
            if (m_ARPlaneMeshVisualizer == null)
                m_ARPlaneMeshVisualizer = GetComponent<ARPlaneMeshVisualizer>();

            if (m_DebugInfoDisplayController == null)
                m_DebugInfoDisplayController = GetComponent<DebugInfoDisplayController>();

            if (!m_ShowAlignment && !m_ShowClassifications && !m_ShowTrackableId && !m_ShowTrackingState)
                m_DebugInfoDisplayController.Show(false);

            if (m_ARPlane == null)
                m_ARPlane = GetComponent<ARPlane>();

            if (m_MeshRenderer == null)
                m_MeshRenderer = GetComponent<MeshRenderer>();

            if ((m_ARPlane.classifications & PlaneClassifications.Couch) == PlaneClassifications.Couch || 
                (m_ARPlane.classifications & PlaneClassifications.Table) == PlaneClassifications.Table)
            {
                m_MeshRenderer.material = m_SpecialPlaneMaterial;
            }

            if (m_ShowPlaneNormal && m_PlaneNormalPrefab == null)
            {
                Debug.LogWarning($"{nameof(m_ShowPlaneNormal)} is enabled but {nameof(m_PlaneNormalPrefab)} is not assigned. To show the plane normal vector visualizer assign a prefab to the {nameof(m_PlaneNormalPrefab)} in the inspector.", this);
            }

            if (m_PlaneNormalPrefab != null)
            {
                m_PlaneNormalVisualizer = Instantiate(m_PlaneNormalPrefab, transform);
                m_PlaneNormalVisualizer.SetActive(false);
            }
        }

        void Start()
        {
            if (m_ShowPlaneNormal)
                m_DebugInfoDisplayController.SetBottomPivot();
            else
                m_DebugInfoDisplayController.SetCenterPivot();
        }

        void Update()
        {
            UpdateDebugInfo();
            UpdatePlaneNormal();
        }

        void OnDestroy()
        {
            if (m_PlaneNormalVisualizer != null)
                Destroy(m_PlaneNormalVisualizer);
        }

        void UpdateDebugInfo()
        {
            var canvasPosition = m_ARPlane.center;
            canvasPosition += m_ShowPlaneNormal ? k_CanvasVerticalOffset : Vector3.zero;
            m_DebugInfoDisplayController.SetPosition(canvasPosition);

            if (m_ARPlane.trackableId == m_TrackableId &&
                m_ARPlane.classifications == m_Classifications &&
                m_ARPlane.alignment == m_Alignment &&
                m_ARPlane.trackingState == m_TrackingState)
                return;

            m_TrackableId = m_ARPlane.trackableId;
            m_Classifications = m_ARPlane.classifications;
            m_Alignment = m_ARPlane.alignment;
            UpdateTrackingState();

            if (m_ShowTrackableId)
                m_DebugInfoDisplayController.AppendDebugEntry("TrackableId:", m_TrackableId.ToString());

            if (m_ShowClassifications)
                m_DebugInfoDisplayController.AppendDebugEntry("Classifications:", m_Classifications.ToString());

            if (m_ShowAlignment)
                m_DebugInfoDisplayController.AppendDebugEntry("Alignment:", m_Alignment.ToString());

            if (m_ShowTrackingState)
                m_DebugInfoDisplayController.AppendDebugEntry("Tracking State:", m_TrackingState.ToString());

            m_DebugInfoDisplayController.RefreshDisplayInfo();
        }

        void UpdatePlaneNormal()
        {
            if (m_PlaneNormalVisualizer != null && m_ShowPlaneNormal != m_PlaneNormalVisualizer.activeSelf)
                m_PlaneNormalVisualizer.SetActive(m_ShowPlaneNormal);

            if (!m_ShowPlaneNormal || m_PlaneNormalVisualizer == null)
                return;

            m_PlaneNormalVisualizer.transform.position = m_ARPlane.center;
            m_PlaneNormalVisualizer.transform.rotation = m_ARPlane.transform.rotation;
        }

        void UpdateTrackingState()
        {
            var newTrackingState = m_ARPlane.trackingState;
            if (newTrackingState == m_TrackingState)
                return;

            m_TrackingState = newTrackingState;
            var mat = m_MeshRenderer.material;
            switch (m_TrackingState)
            {
                case TrackingState.Tracking:
                    mat.SetTexture("_MainTex", m_TrackingTexture);
                    mat.mainTextureScale = new(20, 20);
                    mat.SetColor("_Color", m_TrackingMeshColor);
                    m_Outline.colorGradient = m_TrackingOutlineGradient;
                    break;
                case TrackingState.Limited:
                    mat.SetTexture("_MainTex", m_TrackingTexture);
                    mat.mainTextureScale = new(20, 20);
                    mat.SetColor("_Color", m_TrackingMeshColor);
                    m_Outline.colorGradient = m_LimitedTrackingOutlineGradient;
                    break;
                case TrackingState.None:
                    mat.SetTexture("_MainTex", m_NoneTrackingTexture);
                    mat.mainTextureScale = new(2, 2);
                    mat.SetColor("_Color", m_NoneTrackingMeshColor);
                    m_Outline.colorGradient = m_NoneTrackingOutlineGradient;
                    break;
            }
        }
    }
}
