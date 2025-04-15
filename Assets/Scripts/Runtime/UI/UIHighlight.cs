using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class UIHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField, ReadOnlyField]
        Image m_ImageToHighlight;

        [SerializeField, ReadOnlyField]
        Color m_DefaultColor;

        [SerializeField]
        Color m_HighlightColor = Color.blue;

        [SerializeField]
        UnityEvent m_HighlightEnabled = new();

        [SerializeField]
        UnityEvent m_HighlightDisabled = new();

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_ImageToHighlight.color = m_HighlightColor;
            m_HighlightEnabled?.Invoke();
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            m_ImageToHighlight.color = m_DefaultColor;
            m_HighlightDisabled?.Invoke();
        }

        void Reset()
        {
            m_ImageToHighlight = GetComponent<Image>();

            if (m_ImageToHighlight != null)
                m_DefaultColor = m_ImageToHighlight.color;
        }
    }
}
