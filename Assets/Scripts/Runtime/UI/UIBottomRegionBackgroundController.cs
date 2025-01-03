using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    [DisallowMultipleComponent]
    public class UIBottomRegionBackgroundController : MonoBehaviour
    {
        [SerializeField]
        HMDCanvasController m_HMDCanvasController;

        [SerializeField]
        RectTransform m_BottomRegionSafeAreaRT;

        [SerializeField, HideInInspector]
        RectTransform m_RectTransform;

        [SerializeField, HideInInspector]
        Image m_Image;

        void Reset()
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_Image = GetComponent<Image>();
        }

        void Awake()
        {
            if (m_RectTransform == null)
                m_RectTransform = GetComponent<RectTransform>();

            if (m_Image == null)
                m_Image = GetComponent<Image>();
        }

        void Update()
        {
            if (Application.isPlaying)
                m_Image.enabled = !m_HMDCanvasController.isWorldSpaceCanvas;

            if (m_RectTransform == null || m_BottomRegionSafeAreaRT == null)
                return;

            var safeAreaBottomHeight = Screen.safeArea.yMin * ScaleToSizeOfSafeArea.sizeRatio;
            var backgroundHeight = m_BottomRegionSafeAreaRT.sizeDelta.y + safeAreaBottomHeight;
            if (Mathf.Approximately(m_RectTransform.sizeDelta.y, backgroundHeight))
                return;

            var bottomRegionBackgroundTargetSize = m_RectTransform.sizeDelta;
            bottomRegionBackgroundTargetSize.y = backgroundHeight;
            m_RectTransform.sizeDelta = bottomRegionBackgroundTargetSize;
        }
    }
}
