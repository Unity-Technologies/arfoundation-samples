using System;
using UnityEngine.UI;
#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif // UNITY_IOS

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(ARCameraManager))]
    public class CameraFocusController
#if UNITY_IOS
        : ARKitAdvancedCameraConfigurationController<ARKitFocusMode, ARKitFocus>
#else
        : ARKitAdvancedCameraConfigurationController
#endif
    {
        [SerializeField]
        Slider m_LensPositionSlider;

        float m_LensPosition;

        public float lensPosition
        {
            get => m_LensPosition;
            set => m_LensPosition = value;
        }

#if UNITY_IOS
        protected override void UpdateCachedValues(ARKitLockedCamera lockedCamera)
        {
            currentMode = lockedCamera.currentFocusMode;
            currentValue = lockedCamera.focus;
        }

        protected override void PopulateSupportedModes(ARKitLockedCamera lockedCamera)
        {
            var supportedModes = lockedCamera.supportedFocusModes;
            m_SupportedModes = new();

            if ((supportedModes & ARKitFocusMode.Locked) != ARKitFocusMode.None)
            {
                m_SupportedModes.Add(ARKitFocusMode.Locked);
            }

            if ((supportedModes & ARKitFocusMode.Auto) != ARKitFocusMode.None)
            {
                m_SupportedModes.Add(ARKitFocusMode.Auto);
            }

            if ((supportedModes & ARKitFocusMode.ContinuousAuto) != ARKitFocusMode.None)
            {
                m_SupportedModes.Add(ARKitFocusMode.ContinuousAuto);
            }
        }

        protected override void PopulateRanges(ARKitLockedCamera lockedCamera)
        {
            if (!m_LensPositionSlider)
            {
                return;
            }

            var range = lockedCamera.focusRange;
            Debug.Log($"Lens Position Range: {range.minimumLensPosition}, {range.maximumLensPosition}");

            // update range of lens position slider
            if (m_LensPositionSlider)
            {
                m_LensPositionSlider.minValue = range.minimumLensPosition;
                m_LensPositionSlider.maxValue = range.maximumLensPosition;
            }

            RefreshUI(lockedCamera.focus);
        }

        void RefreshUI(ARKitFocus focus)
        {
            if (m_LensPositionSlider != null && Math.Abs(m_LensPositionSlider.value - focus.lensPosition) > float.Epsilon)
            {
                m_LensPosition = focus.lensPosition;
                m_LensPositionSlider.SetValueWithoutNotify(focus.lensPosition);
            }
        }

        protected override void UpdateMode(ARKitLockedCamera lockedCamera, ARKitFocusMode mode)
        {
            Debug.Log($"Updating focus mode to {mode.ToString()}.");
            lockedCamera.requestedFocusMode = mode;
        }

        protected override void UpdateConfigValues(ARKitLockedCamera lockedCamera)
        {
            var focus = new ARKitFocus(m_LensPosition);
            Debug.Log($"Updating focus: {focus.ToString()}");

            lockedCamera.focus = focus;
        }
#endif // UNITY_IOS
    }
}
