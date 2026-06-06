using System;
using TMPro;
using UnityEngine.EventSystems;

namespace UnityEngine.XR.ARFoundation.PackageSamples.DebugVisualizers
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class MarkerDataVisualizer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        RectTransform m_CanvasRT;

        [SerializeField, ReadOnlyField]
        TextMeshProUGUI m_Label;

        RectTransform m_RectTransform;

        readonly Color m_NonLinkColor = new(196f / 255f, 196f / 255f, 196f / 255, 1f);
        readonly Color m_LinkColor = new(32f / 255f, 150f / 255f, 243f / 255, 1f);
        readonly Color m_LinkHoverColor = new(24f / 255f, 122f / 255f, 182f / 255f, 1f);
        readonly Color m_LinkClickColor = new(88f / 255f, 176f / 255f, 246f / 255f, 1f);

        string m_LabelText;
        bool m_IsWebLink;
        bool m_WasHoveringLink;

        const float k_MaxWidth = 0.52f;

        void Reset()
        {
            m_Label = GetComponent<TextMeshProUGUI>();
        }

        void Awake()
        {
            m_RectTransform = m_Label.GetComponent<RectTransform>();
        }

        public Vector2 GetPreferredValues()
        {
            var wrapWidth =  m_RectTransform.rect.width;
            return m_Label.GetPreferredValues(m_Label.text, wrapWidth, Mathf.Infinity);
        }

        public void SetText(string text)
        {
            m_LabelText = text;
            m_IsWebLink = IsWebLink(text);
            m_Label.color = m_IsWebLink ? m_LinkColor : m_NonLinkColor;

            var size = m_RectTransform.sizeDelta;
            size.x = k_MaxWidth;
            m_RectTransform.sizeDelta = size;

            m_Label.text = text;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!m_IsWebLink)
                return;

            m_WasHoveringLink = true;
            m_Label.color = m_LinkHoverColor;
            m_Label.text = $"<u>{m_LabelText}</u>";
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!m_IsWebLink)
                return;

            if (!m_WasHoveringLink)
                return;

            m_Label.color = m_LinkColor;
            m_Label.text = $"{m_LabelText}";
            m_WasHoveringLink = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!m_IsWebLink)
                return;

            m_Label.color = m_LinkClickColor;
            Application.OpenURL(m_LabelText);
        }

        static bool IsWebLink(string text)
        {
            if (!Uri.TryCreate(text, UriKind.Absolute, out var uriResult))
                return false;

            return uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps;
        }
    }
}
