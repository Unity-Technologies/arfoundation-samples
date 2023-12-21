using System.Text;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    [RequireComponent(typeof(ARPlaneMeshVisualizer))]
    [RequireComponent(typeof(ARPlane))]
    public class ARPlaneDebugVisualizer : MonoBehaviour
    {
        static readonly Vector3 k_TextFlipVec = new(0, 180, 0);
        static readonly float k_ColumnWidthExtent = 0.005f;

        [Header("References")]
        [SerializeField, Tooltip("The prefab to visualize the plane orientation.")]
        GameObject m_PlaneNormalPrefab;

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

        [SerializeField, Tooltip("The size of the font for the debug text.")]
        float m_FontSize = 0.25f;

        [SerializeField, Tooltip("The mesh color used for planes in the Tracking state")]
        Color trackingColor = new Color(253, 184, 19, 84);

        [SerializeField, Tooltip("The mesh color used for planes in the Limited state")]
        Color limitedColor = new Color(75, 75, 75, 84);
        
        [SerializeField, Tooltip("The mesh color used for planes in the None state")]
        Color noneColor = new Color(75, 75, 75, 84);

        [SerializeField, HideInInspector]
        ARPlaneMeshVisualizer m_ARPlaneMeshVisualizer;

        [SerializeField, HideInInspector]
        ARPlane m_ARPlane;

        [SerializeField, HideInInspector]
        MeshRenderer m_MeshRenderer;

        public ARPlaneMeshVisualizer arPlaneMeshVisualizer => m_ARPlaneMeshVisualizer;

        GameObject m_PlaneNormalVisualizer;
        Transform m_MainCameraTransform;
        Transform m_DebugLabelOffset;
        TextMeshPro m_DebugLabelTypes;
        TextMeshPro m_DebugLabelValues;

        StringBuilder m_TypesBuilder = new();
        StringBuilder m_ValuesBuilder = new();
        TrackableId m_TrackableId;
        PlaneClassifications m_Classifications;
        PlaneAlignment m_Alignment;
        TrackingState m_TrackingState;

        void OnDestroy()
        {
            Destroy(m_DebugLabelValues.gameObject);
            
            if (m_PlaneNormalVisualizer != null)
                Destroy(m_PlaneNormalVisualizer);
        }

        void Awake()
        {
            if (m_ARPlaneMeshVisualizer == null)
                m_ARPlaneMeshVisualizer = GetComponent<ARPlaneMeshVisualizer>();

            if (m_ARPlane == null)
                m_ARPlane = GetComponent<ARPlane>();
            
            if (m_MeshRenderer == null)
                m_MeshRenderer = GetComponent<MeshRenderer>();

            m_MainCameraTransform = Camera.main!.transform;

            if (m_ShowPlaneNormal && m_PlaneNormalPrefab == null)
            {
                Debug.LogWarning($"{nameof(m_ShowPlaneNormal)} is enabled but {nameof(m_PlaneNormalPrefab)} is not assigned. To show the plane normal vector visualizer assign a prefab to the {nameof(m_PlaneNormalPrefab)} in the inspector.", this);
            }
            
            if (m_PlaneNormalPrefab != null)
            {
                m_PlaneNormalVisualizer = Instantiate(m_PlaneNormalPrefab, transform);
                m_PlaneNormalVisualizer.SetActive(false);
            }

            m_DebugLabelOffset = new GameObject("Debug Label Offset").transform;
            m_DebugLabelOffset.SetParent(transform);
            m_DebugLabelOffset.Rotate(k_TextFlipVec);

            SetupDebugLabelTypesText();
            SetupDebugLabelValuesText();
        }
        
        void Reset()
        {
            m_ARPlaneMeshVisualizer = GetComponent<ARPlaneMeshVisualizer>();
            m_ARPlane = GetComponent<ARPlane>();
            m_MeshRenderer = GetComponent<MeshRenderer>();
        }

        void Update()
        {
            UpdateDebugInfo();
            UpdatePlaneNormal();
            
            switch (m_TrackingState)
            {
                case TrackingState.Tracking:
                    m_MeshRenderer.material.color = trackingColor;
                    break;
                case TrackingState.Limited:
                    m_MeshRenderer.material.color = limitedColor;
                    break;
                case TrackingState.None:
                    m_MeshRenderer.material.color = noneColor;
                    break;
            }
        }

        void UpdateDebugInfo()
        {
            m_DebugLabelOffset.position = m_ARPlane.center;
            m_DebugLabelOffset.LookAt(m_MainCameraTransform.transform);

            if (m_ARPlane.trackableId == m_TrackableId &&
                m_ARPlane.classifications == m_Classifications &&
                m_ARPlane.alignment == m_Alignment &&
                m_ARPlane.trackingState == m_TrackingState)
                return;

            m_TrackableId = m_ARPlane.trackableId;
            m_Classifications = m_ARPlane.classifications;
            m_Alignment = m_ARPlane.alignment;
            m_TrackingState = m_ARPlane.trackingState;

            m_DebugLabelValues.text = string.Empty;

            if (m_ShowTrackableId)
                FormatDebugText("TrackableId:", m_TrackableId.ToString());

            if (m_ShowClassifications)
                FormatDebugText("Classifications:", m_Classifications.ToString());

            if (m_ShowAlignment)
                FormatDebugText("Alignment:", m_Alignment.ToString());

            if (m_ShowTrackingState)
                FormatDebugText("TrackingState:", m_TrackingState.ToString());

            m_DebugLabelTypes.text = m_TypesBuilder.ToString();
            m_TypesBuilder.Clear();
            m_DebugLabelValues.text = m_ValuesBuilder.ToString();
            m_ValuesBuilder.Clear();

            var textOffset = (m_DebugLabelTypes.preferredWidth / 2f) + k_ColumnWidthExtent;
            m_DebugLabelTypes.transform.localPosition = new Vector3(textOffset, 0, 0);
            
            textOffset = (-m_DebugLabelValues.preferredWidth / 2f) - k_ColumnWidthExtent;
            m_DebugLabelValues.transform.localPosition = new Vector3(textOffset, 0, 0);
        }

        void FormatDebugText(string dataType, string value)
        {
            if (dataType.Length != 0)
                m_TypesBuilder.AppendLine();

            m_TypesBuilder.Append($"<b>{dataType}</b>");
            
            if (m_ValuesBuilder.Length != 0)
                m_ValuesBuilder.AppendLine();

            m_ValuesBuilder.Append(value);
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

        void SetupDebugLabelTypesText()
        {
            var debugLabelTypes = new GameObject("Debug Label Types");
            debugLabelTypes.transform.SetParent(m_DebugLabelOffset);

            m_DebugLabelTypes = debugLabelTypes.AddComponent<TextMeshPro>();
            var contentSizeFitter = debugLabelTypes.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            m_DebugLabelTypes.fontSize = m_FontSize;
            m_DebugLabelTypes.alignment = TextAlignmentOptions.MidlineRight;
        }

        void SetupDebugLabelValuesText()
        {
            var debugLabelValues = new GameObject("Debug Label Values");
            debugLabelValues.transform.SetParent(m_DebugLabelOffset);

            m_DebugLabelValues = debugLabelValues.AddComponent<TextMeshPro>();
            var contentSizeFitter = debugLabelValues.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            m_DebugLabelValues.fontSize = m_FontSize;
            m_DebugLabelValues.alignment = TextAlignmentOptions.MidlineLeft;
        }
    }
}
