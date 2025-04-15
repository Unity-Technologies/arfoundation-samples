using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace UnityEngine.XR.ARFoundation
{
    public class DebugInfoDisplayController : MonoBehaviour
    {
        const float k_DebugInfoBackgroundPadding = 0.04f;

        [SerializeField, Tooltip("")]
        Canvas m_Canvas;

        [SerializeField, Tooltip("")]
        RectTransform m_BackgroundRT;

        [SerializeField, Tooltip("")]
        RectTransform m_DebugLabelOffsetRT;

        [SerializeField, Tooltip("")]
        TextMeshProUGUI m_DebugLabelTypes;

        [SerializeField, Tooltip("")]
        TextMeshProUGUI m_DebugLabelValues;

        [SerializeField, Tooltip("")]
        RectTransform m_Icon;

        Camera m_MainCamera;
        Transform m_MainCameraTransform;
        RectTransform m_MainCameraRT;

        StringBuilder m_TypesBuilder = new();
        StringBuilder m_ValuesBuilder = new();
        Transform m_CanvasTransform;
        float m_ColumnWidth;
        Vector2 m_CanvasSizeInWorld;
        float m_HalfCanvasHeight;

        /// <summary>
        /// Sets the canvas pivot to the center when centering the canvas at
        /// its position is required.
        /// </summary>
        public void SetCenterPivot()
        {
            m_MainCameraRT.pivot = new(0.5f, m_HalfCanvasHeight);
        }

        /// <summary>
        /// Sets the canvas pivot at the bottom when positioning the canvas
        /// offset above a position is required.
        /// </summary>
        public void SetBottomPivot()
        {
            if (m_MainCameraRT != null)
                m_MainCameraRT.pivot = new(0.5f, 0f);
        }

        /// <summary>
        /// Shows or hides the debug info. Debug info is shown by default.
        /// </summary>
        /// <param name="isOn"></param>
        public void Show(bool isOn)
        {
            m_Canvas.gameObject.SetActive(isOn);
        }

        public void SetPosition(Vector3 position)
        {
            if (m_MainCameraRT != null)
                m_CanvasTransform.position = position;
        }

        public void AppendDebugEntry(string dataType, string dataValue)
        {
            if (m_TypesBuilder.Length != 0)
                m_TypesBuilder.AppendLine();

            m_TypesBuilder.Append($"<b>{dataType}</b>");

            if (m_ValuesBuilder.Length != 0)
                m_ValuesBuilder.AppendLine();

            m_ValuesBuilder.Append(dataValue);
        }

        public void RefreshDisplayInfo()
        {
            m_DebugLabelTypes.text = m_TypesBuilder.ToString();
            m_TypesBuilder.Clear();
            m_DebugLabelValues.text = m_ValuesBuilder.ToString();
            m_ValuesBuilder.Clear();

            // update background size and position
            var debugLabelTypesSize = m_DebugLabelTypes.GetPreferredValues();
            var debugLabelValuesSize = m_DebugLabelValues.GetPreferredValues();

            m_DebugLabelTypes.GetComponent<RectTransform>().sizeDelta = debugLabelTypesSize;
            m_DebugLabelValues.GetComponent<RectTransform>().sizeDelta = debugLabelValuesSize;

            var width = debugLabelTypesSize.x + debugLabelValuesSize.x;
            var widthPlusSpacingAndPadding = width + m_ColumnWidth + k_DebugInfoBackgroundPadding;
            var height = Math.Max(debugLabelTypesSize.y, debugLabelValuesSize.y);

            m_BackgroundRT.sizeDelta = new(widthPlusSpacingAndPadding, height + m_Icon.sizeDelta.y + k_DebugInfoBackgroundPadding);

            // shift background to the left to center
            var halfWidth = width * 0.5f;
            var positionOffset = Vector2.zero;
            positionOffset.x -= debugLabelTypesSize.x - halfWidth;
            positionOffset.y = -k_DebugInfoBackgroundPadding * 0.5f;
            m_BackgroundRT.anchoredPosition = positionOffset;

            // place icon in center
            m_Icon.anchoredPosition = new(positionOffset.x, height);

            // shift the label offset to the right to center
            var anchoredPosition = new Vector2(-positionOffset.x, 0);
            m_DebugLabelOffsetRT.anchoredPosition = anchoredPosition;
        }

        void Awake()
        {
            m_MainCamera = Camera.main!;
            m_MainCameraTransform = m_MainCamera.transform;
            m_Canvas.worldCamera = m_MainCamera;
            m_BackgroundRT.sizeDelta = Vector2.zero;
            m_CanvasTransform = m_Canvas.transform;
            m_MainCameraRT = m_Canvas.GetComponent<RectTransform>();

            var debugLabelValuesRT = m_DebugLabelValues.GetComponent<RectTransform>();
            m_ColumnWidth = debugLabelValuesRT.anchoredPosition.x * 2f;
            m_HalfCanvasHeight = m_MainCameraRT.sizeDelta.y * 0.5f;

            m_DebugLabelTypes.text = string.Empty;
            m_DebugLabelValues.text = string.Empty;
        }

        void Update()
        {
            var cameraToCanvasVector = transform.position - m_MainCameraTransform.position;
            m_CanvasTransform.LookAt(transform.position + cameraToCanvasVector);
        }

        void OnDestroy()
        {
            Destroy(m_DebugLabelValues.gameObject);
        }
    }
}
