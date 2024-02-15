using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class UIBottomRegionSafeAreaController : MonoBehaviour
    {
        [SerializeField]
        HMDCanvasController m_HMDCanvasController;

        [SerializeField]
        RectTransform m_NavigationBarRT;

        [SerializeField, HideInInspector]
        RectTransform m_RectTransform;
        
        void Reset()
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_HMDCanvasController = FindAnyObjectByType<HMDCanvasController>();

            if (m_HMDCanvasController == null)
                Debug.LogError("m_HMDCanvasController is null.");
            
            if (m_HMDCanvasController == null)
                Debug.LogError("m_HMDCanvasController is null.", this);
        }

        void Awake()
        {
            if (m_RectTransform == null)
                m_RectTransform = GetComponent<RectTransform>();

            if (m_HMDCanvasController == null)
                m_HMDCanvasController = FindAnyObjectByType<HMDCanvasController>();
            
            if (m_HMDCanvasController == null)
                Debug.LogError("m_HMDCanvasController is null.", this);
        }

        void OnEnable()
        {
            if (m_HMDCanvasController.isWorldSpaceCanvas)
            {
                m_RectTransform.anchorMin = Vector2.zero;
                m_RectTransform.anchorMax = Vector2.zero;
                m_RectTransform.pivot = Vector2.zero;
            }
        }

        void Update()
        {
            if (m_HMDCanvasController == null)
                return;

            // have to update every frame because world space canvas gets set
            // the frame after Start is called and using async await causes 
            // the canvas to not get placed in front of the user for some reason
            if (m_HMDCanvasController.isWorldSpaceCanvas)
            {
                var bottomRegionSize = m_HMDCanvasController.canvasDimensions;
                bottomRegionSize.y -= m_NavigationBarRT.sizeDelta.y;
                m_RectTransform.sizeDelta = bottomRegionSize;
            }
        }
    }
}
