using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(ScrollRect))]
    public class UIScrollViewVerticalScrollController : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        ScrollRect m_ScrollView;
        
        [SerializeField]
        RectTransform m_Viewport;

        [SerializeField]
        RectTransform m_Content;

        void Reset() => m_ScrollView = GetComponent<ScrollRect>();

        void Start()
        {
            if (m_ScrollView == null)
                m_ScrollView = GetComponent<ScrollRect>();
        }

        void Update()
        {
            var viewportHeight = m_Viewport.rect.size.y;
            var contentHeight = m_Content.rect.size.y;

            m_ScrollView.vertical = contentHeight > viewportHeight;
        }
    }
}
