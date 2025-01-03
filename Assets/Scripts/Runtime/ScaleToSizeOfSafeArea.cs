using System;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class ScaleToSizeOfSafeArea : MonoBehaviour
    {
        [SerializeField, Tooltip("The canvas that this component is a part of.")]
        Canvas m_Canvas;

        [SerializeField, Tooltip("The header gradient background used to add contrast " +
             "to the top buttons when on non-HMD devices. Will be disabled on HMD.")]
        GameObject m_HeaderGradientBackground;

        [SerializeField, Tooltip("The HMD canvas controller used to determine if the UI " +
             "should consider the safe area. If the canvas is in world space, then " +
             "this RectTransform will not be updated to the size of the safe area.")]
        HMDCanvasController m_HmdCanvasController;

        [SerializeField, HideInInspector]
        RectTransform m_RectTransform;

        public static float sizeRatio { get; private set; }

        void Reset()
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_Canvas = FindAnyObjectByType<Canvas>();
        }

        void Awake()
        {
            if (m_RectTransform == null)
                m_RectTransform = GetComponent<RectTransform>();

            if (m_Canvas == null)
                m_Canvas = FindAnyObjectByType<Canvas>();

            m_HeaderGradientBackground.SetActive(enabled);
        }

        void OnEnable()
        {
            if (m_HmdCanvasController.isWorldSpaceCanvas)
            {
                enabled = false;
                return;
            }

            m_HeaderGradientBackground.SetActive(true);
            m_RectTransform.anchorMin = Vector2.zero;
            m_RectTransform.anchorMax = Vector2.zero;
            m_RectTransform.pivot = new(0.5f, 0.5f);

            UpdateSafeArea();
        }

        void OnDisable() => m_HeaderGradientBackground.SetActive(false);

        void Update() => UpdateSafeArea();

        void UpdateSafeArea()
        {
            // checking for situation where this code is run on a platform with no screen
            // such as CI tests
            if (Screen.width == 0 || Screen.height == 0 || m_Canvas.scaleFactor == 0)
                return;

            sizeRatio = 1 / m_Canvas.scaleFactor;

            var scaledSafeAreaSize = Screen.safeArea.size / m_Canvas.scaleFactor;
            m_RectTransform.sizeDelta = scaledSafeAreaSize;

            var scaledSafeAreaCenter = Screen.safeArea.center / m_Canvas.scaleFactor;
            m_RectTransform.anchoredPosition = scaledSafeAreaCenter;
        }
    }
}
