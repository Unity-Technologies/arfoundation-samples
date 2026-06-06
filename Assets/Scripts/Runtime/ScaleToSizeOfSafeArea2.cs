using System;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class ScaleToSizeOfSafeArea2 : MonoBehaviour
    {
        [SerializeField, Tooltip("The canvas that this component is a part of.")]
        Canvas m_Canvas;

        [SerializeField, HideInInspector]
        RectTransform m_RectTransform;

        public float sizeRatio { get; private set; }

        Rect m_LastSafeArea;
        float m_LastScaleFactor;

        void Reset()
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_Canvas = GetComponentInParent<Canvas>();
        }

        void Awake()
        {
            if (m_RectTransform == null)
                m_RectTransform = GetComponent<RectTransform>();

            if (m_Canvas == null)
                m_Canvas = GetComponentInParent<Canvas>();
        }

        void OnEnable()
        {
            m_RectTransform.anchorMin = Vector2.zero;
            m_RectTransform.anchorMax = Vector2.zero;
            m_RectTransform.pivot = new(0.5f, 0.5f);

            ForceUpdateSafeArea();
        }

        void Update()
        {
            if (m_LastSafeArea != Screen.safeArea || !Mathf.Approximately(m_LastScaleFactor, m_Canvas.scaleFactor))
                ForceUpdateSafeArea();
        }

        void ForceUpdateSafeArea()
        {
            // Checking for situation where this code is run on a platform with no screen such as CI tests
            if (Screen.width == 0 || Screen.height == 0 || m_Canvas.scaleFactor == 0)
                return;

            m_LastSafeArea = Screen.safeArea;
            m_LastScaleFactor = m_Canvas.scaleFactor;

            sizeRatio = 1f / m_Canvas.scaleFactor;

            var scaledSafeAreaSize = m_LastSafeArea.size * sizeRatio;
            m_RectTransform.sizeDelta = scaledSafeAreaSize;

            var scaledSafeAreaCenter = m_LastSafeArea.center * sizeRatio;
            m_RectTransform.anchoredPosition = scaledSafeAreaCenter;
        }
    }
}
