using System;
using UnityEngine.UI;
#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif // UNITY_IOS

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(ARCameraManager))]
    public class CameraExposureController
#if UNITY_IOS
        : ARKitAdvancedCameraConfigurationController<ARKitExposureMode, ARKitExposure>
#else
        : ARKitAdvancedCameraConfigurationController
#endif
    {
        [SerializeField]
        Slider m_DurationSlider;

        [SerializeField]
        Slider m_IsoSlider;

#if UNITY_IOS
        double m_Duration;
        float m_Iso;

        public float duration
        {
            get => (float)m_Duration;
            set => m_Duration = Math.Round(value, 9);
        }

        public float iso
        {
            get => m_Iso;
            set => m_Iso = value;
        }

        protected override void PostUpdate()
        {
            if (currentMode != ARKitExposureMode.Custom)
            {
                RefreshUI(currentValue);
            }
        }

        protected override void UpdateCachedValues(ARKitLockedCamera lockedCamera)
        {
            currentMode = lockedCamera.currentExposureMode;
            currentValue = lockedCamera.exposure;
        }

        protected override void PopulateSupportedModes(ARKitLockedCamera lockedCamera)
        {
            var supportedModes = lockedCamera.supportedExposureModes;
            m_SupportedModes = new();

            if ((supportedModes & ARKitExposureMode.Locked) != ARKitExposureMode.None)
            {
                m_SupportedModes.Add(ARKitExposureMode.Locked);
            }

            if ((supportedModes & ARKitExposureMode.Auto) != ARKitExposureMode.None)
            {
                m_SupportedModes.Add(ARKitExposureMode.Auto);
            }

            if ((supportedModes & ARKitExposureMode.ContinuousAuto) != ARKitExposureMode.None)
            {
                m_SupportedModes.Add(ARKitExposureMode.ContinuousAuto);
            }

            if ((supportedModes & ARKitExposureMode.Custom) != ARKitExposureMode.None)
            {
                m_SupportedModes.Add(ARKitExposureMode.Custom);
            }
        }

        protected override void PopulateRanges(ARKitLockedCamera lockedCamera)
        {
            if (!m_DurationSlider && !m_IsoSlider)
            {
                return;
            }

            var range = lockedCamera.exposureRange;
            Debug.Log($"Exposure Range: {range.minimumDuration}, {range.maximumDuration}");
            Debug.Log($"ISO Range: {range.minimumIso}, {range.maximumIso}");

            // update range of duration slider
            if (m_DurationSlider)
            {
                m_DurationSlider.minValue = (float)range.minimumDuration;
                m_DurationSlider.maxValue = (float)range.maximumDuration;
            }

            // update range of iso slider
            if (m_IsoSlider)
            {
                m_IsoSlider.minValue = range.minimumIso;
                m_IsoSlider.maxValue = range.maximumIso;
            }
        }

        protected override bool InteractableUpdateButton(ARKitExposureMode mode)
            => mode == ARKitExposureMode.Custom;

        void RefreshUI(ARKitExposure exposure)
        {
            if (m_DurationSlider != null && Math.Abs(m_DurationSlider.value - exposure.duration) > float.Epsilon)
            {
                m_Duration = exposure.duration;
                m_DurationSlider.SetValueWithoutNotify((float)exposure.duration);
            }

            if (m_IsoSlider != null && Math.Abs(m_IsoSlider.value - exposure.iso) > float.Epsilon)
            {
                m_Iso = exposure.iso;
                m_IsoSlider.SetValueWithoutNotify(exposure.iso);
            }
        }

        protected override void UpdateMode(ARKitLockedCamera lockedCamera, ARKitExposureMode mode)
        {
            Debug.Log($"Updating exposure mode to {mode.ToString()}.");
            lockedCamera.requestedExposureMode = mode;
        }

        protected override void UpdateConfigValues(ARKitLockedCamera lockedCamera)
        {
            var exposure = new ARKitExposure(m_Duration, m_Iso);
            Debug.Log($"Updating exposure: {exposure.ToString()}");

            lockedCamera.exposure = exposure;
        }
#endif // UNITY_IOS
    }
}
