using TMPro;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// This example demonstrates how to change the plane detection mode.
    /// </summary>
    [RequireComponent(typeof(ARPlaneManager))]
    public class PlaneDetectionModeController : MonoBehaviour
    {
        [Tooltip("The UI Text element used to display the state of the Plane Detection Mode flags.")]
        [SerializeField]
        TextMeshProUGUI m_FlagsText;

        [Tooltip("The SliderToggle element used to control the on/off state of Horizontal plane detection.")]
        [SerializeField]
        SliderToggle m_HorizontalSliderToggle;
        
        [Tooltip("The SliderToggle element used to control the on/off state of Vertical plane detection.")]
        [SerializeField]
        SliderToggle m_VerticalSliderToggle;
        
        [Tooltip("The SliderToggle element used to control the on/off state of NotAxisAligned plane detection.")]
        [SerializeField]
        SliderToggle m_NotAxisAlignedSliderToggle;

        [SerializeField, HideInInspector]
        ARPlaneManager m_ARPlaneManager;

        PlaneDetectionMode m_DetectionMode;

        void Reset()
        {
            m_ARPlaneManager = GetComponent<ARPlaneManager>();
        }
        
        void Start()
        {
            var detectionMode = m_ARPlaneManager.requestedDetectionMode;
            m_FlagsText.text = "Flags: " + detectionMode;
            m_DetectionMode = detectionMode;

            if (m_ARPlaneManager.descriptor.supportsHorizontalPlaneDetection)
                m_HorizontalSliderToggle.SetSliderValue((detectionMode & PlaneDetectionMode.Horizontal) == PlaneDetectionMode.Horizontal);
            else
                m_HorizontalSliderToggle.SetEnabled(false);
            
            if (m_ARPlaneManager.descriptor.supportsVerticalPlaneDetection)
                m_VerticalSliderToggle.SetSliderValue((detectionMode & PlaneDetectionMode.Vertical) == PlaneDetectionMode.Vertical);
            else
                m_VerticalSliderToggle.SetEnabled(false);
            
            if (m_ARPlaneManager.descriptor.supportsArbitraryPlaneDetection)
                m_NotAxisAlignedSliderToggle.SetSliderValue((detectionMode & PlaneDetectionMode.NotAxisAligned) == PlaneDetectionMode.NotAxisAligned);
            else
                m_NotAxisAlignedSliderToggle.SetEnabled(false);
        }

        void Update()
        {
            var detectionMode = m_ARPlaneManager.currentDetectionMode;

            if (m_DetectionMode != detectionMode)
            {
                m_FlagsText.text = "Flags: " + detectionMode;
                m_DetectionMode = detectionMode;
            }
        }

        public void ToggleHorizontalPlanes()
        {
            m_ARPlaneManager.requestedDetectionMode ^= PlaneDetectionMode.Horizontal;
            m_HorizontalSliderToggle.SetSliderValue((m_ARPlaneManager.requestedDetectionMode & PlaneDetectionMode.Horizontal) == PlaneDetectionMode.Horizontal);
        }

        public void ToggleVerticalPlanes()
        {
            m_ARPlaneManager.requestedDetectionMode ^= PlaneDetectionMode.Vertical;
            m_VerticalSliderToggle.SetSliderValue((m_ARPlaneManager.requestedDetectionMode & PlaneDetectionMode.Vertical) == PlaneDetectionMode.Vertical);
        }

        public void ToggleNotAxisAlignedPlanes()
        {
            m_ARPlaneManager.requestedDetectionMode ^= PlaneDetectionMode.NotAxisAligned;
            m_NotAxisAlignedSliderToggle.SetSliderValue((m_ARPlaneManager.requestedDetectionMode & PlaneDetectionMode.NotAxisAligned) == PlaneDetectionMode.NotAxisAligned);
        }
    }
}
