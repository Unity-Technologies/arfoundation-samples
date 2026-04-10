using System;
using System.Text;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

#if XRI_3_4_0_PRE_2_OR_NEWER
using UnityEngine.XR.Interaction.Toolkit.UI;
#endif

namespace UnityEngine.XR.ARFoundation.PackageSamples.DebugVisualizers
{
    public class MarkerDebugInfo : MonoBehaviour
    {
        const float k_CanvasVerticalOffset = 0.025f;
        const float k_CanvasPadding = 0.05f;

        [Header("Containers")]
        [SerializeField]
        RectTransform m_CanvasRT;

        [SerializeField]
        RectTransform m_LabelContainerRT;

        [Header("Labels")]
        [SerializeField]
        RectTransform m_TrackableIdLabelRT;

        [SerializeField]
        RectTransform m_TrackingStateLabelRT;

        [SerializeField]
        RectTransform m_MarkerTypeLabelRT;

        [SerializeField]
        RectTransform m_MarkerIdLabelRT;

        [SerializeField]
        RectTransform m_EncodedDataLabelRT;

        [SerializeField]
        RectTransform m_DataLabelRT;

        [Header("Values")]
        [SerializeField]
        RectTransform m_TrackableIdValueRT;

        [SerializeField]
        RectTransform m_TrackingStateValueRT;

        [SerializeField]
        RectTransform m_MarkerTypeValueRT;

        [SerializeField]
        RectTransform m_MarkerIdValueRT;

        [FormerlySerializedAs("m_EncodedDataValuelRT")]
        [SerializeField]
        RectTransform m_EncodedDataValueRT;

        [SerializeField]
        RectTransform m_DataValueRT;

        [SerializeField]
        MarkerDataVisualizer m_MarkerDataVisualizer;

        public ARMarker marker { get; set; }

        Transform m_MainCameraTransform;
        TextMeshProUGUI m_TrackableIdValue;
        TextMeshProUGUI m_TrackingStateValue;
        TextMeshProUGUI m_MarkerTypeValue;
        TextMeshProUGUI m_MarkerIdValue;
        TextMeshProUGUI m_EncodedDataValue;

        void Awake()
        {
            m_TrackableIdValue = m_TrackableIdValueRT.GetComponent<TextMeshProUGUI>();
            m_TrackingStateValue = m_TrackingStateValueRT.GetComponent<TextMeshProUGUI>();
            m_MarkerTypeValue = m_MarkerTypeValueRT.GetComponent<TextMeshProUGUI>();
            m_MarkerIdValue = m_MarkerIdValueRT.GetComponent<TextMeshProUGUI>();
            m_EncodedDataValue = m_EncodedDataValueRT.GetComponent<TextMeshProUGUI>();

            m_MainCameraTransform = Camera.main.transform;

#if XRI_3_4_0_PRE_2_OR_NEWER
            if (m_CanvasRT.GetComponent<Canvas>() != null)
                m_CanvasRT.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
#endif
        }

        void Update()
        {
            var offset = m_CanvasRT.sizeDelta.y * m_CanvasRT.localScale.x * 0.5f;
            offset += marker.size.y * 0.5f;
            offset += k_CanvasVerticalOffset;
            m_CanvasRT.transform.position = marker.pose.position + new Vector3(0, offset, 0);

            var cameraToCanvasVector = m_CanvasRT.transform.position - m_MainCameraTransform.position;
            m_CanvasRT.LookAt(m_CanvasRT.transform.position + cameraToCanvasVector);
        }

        public void ShowTrackableId(bool show, TrackableId trackableId)
        {
            m_TrackableIdLabelRT.gameObject.SetActive(show);
            m_TrackableIdValueRT.gameObject.SetActive(show);
            m_TrackableIdValue.text = trackableId.ToString();
        }

        public void ShowTrackingState(bool show, TrackingState trackingState)
        {
            m_TrackingStateLabelRT.gameObject.SetActive(show);
            m_TrackingStateValueRT.gameObject.SetActive(show);
            m_TrackingStateValue.text = trackingState.ToString();
        }

        public void ShowMarkerType(bool show, XRMarkerType markerType)
        {
            m_MarkerTypeLabelRT.gameObject.SetActive(show);
            m_MarkerTypeValueRT.gameObject.SetActive(show);
            m_MarkerTypeValue.text = markerType.ToString();
        }

        public void ShowMarkerId(bool show, uint markerId)
        {
            m_MarkerIdLabelRT.gameObject.SetActive(show);
            m_MarkerIdValueRT.gameObject.SetActive(show);
            m_MarkerIdValue.text = markerId.ToString();
        }

        public void ShowEncodedDataType(bool show, XRSpatialBufferType dataType)
        {
            m_EncodedDataLabelRT.gameObject.SetActive(show);
            m_EncodedDataValueRT.gameObject.SetActive(show);
            m_EncodedDataValue.text = dataType.ToString();
        }

        public void ShowEncodedStringData(bool show, string stringData)
        {
            m_DataLabelRT.gameObject.SetActive(show);
            m_DataValueRT.gameObject.SetActive(show);
            m_MarkerDataVisualizer.SetText(stringData);
        }

        public void ShowEncodedBytesData(bool show, byte[] bytes)
        {
            m_DataLabelRT.gameObject.SetActive(show);
            m_DataValueRT.gameObject.SetActive(show);
            var bytesAsString = BitConverter.ToString(bytes);
            m_MarkerDataVisualizer.SetText(bytesAsString);
        }

        public void Refresh()
        {
            // Resize from the most nested component up
            ResizeDataLabel();
            ResizeLabelContainer();
            ResizeCanvas();
        }

        void ResizeDataLabel()
        {
            var size = m_MarkerDataVisualizer.GetPreferredValues();
            m_DataValueRT.sizeDelta = size;
        }

        void ResizeLabelContainer()
        {
            var labelContainerHeight = 0f;

            if (m_TrackableIdValue.gameObject.activeSelf)
                labelContainerHeight += m_TrackableIdValueRT.rect.height;

            if (m_TrackingStateValueRT.gameObject.activeSelf)
                labelContainerHeight += m_TrackingStateValueRT.rect.height;

            if (m_MarkerTypeValueRT.gameObject.activeSelf)
                labelContainerHeight += m_MarkerTypeValueRT.rect.height;

            if (m_MarkerIdValueRT.gameObject.activeSelf)
                labelContainerHeight += m_MarkerIdValueRT.rect.height;

            if (m_EncodedDataValueRT.gameObject.activeSelf)
                labelContainerHeight += m_EncodedDataValueRT.rect.height;

            if (m_DataValueRT.gameObject.activeSelf)
                labelContainerHeight += m_DataValueRT.rect.height;

            var labelContainerSize = m_LabelContainerRT.sizeDelta;
            labelContainerSize.y = labelContainerHeight;
            m_LabelContainerRT.sizeDelta = labelContainerSize;
        }

        void ResizeCanvas()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_CanvasRT);
            var canvasSize = m_CanvasRT.rect.size;
            canvasSize.x = m_LabelContainerRT.rect.width + k_CanvasPadding;
            m_CanvasRT.sizeDelta = canvasSize;
        }
    }
}
