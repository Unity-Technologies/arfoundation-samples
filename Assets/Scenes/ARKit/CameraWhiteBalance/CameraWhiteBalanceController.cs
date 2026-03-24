using System;
using UnityEngine.UI;
#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif // UNITY_IOS

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(ARCameraManager))]
    public class CameraWhiteBalanceController
#if UNITY_IOS
        : ARKitAdvancedCameraConfigurationController<ARKitWhiteBalanceMode, ARKitWhiteBalanceGains>
#else
        : ARKitAdvancedCameraConfigurationController
#endif
    {
        [SerializeField]
        Slider m_BlueGainSlider;

        [SerializeField]
        Slider m_GreenGainSlider;

        [SerializeField]
        Slider m_RedGainSlider;

        bool m_SlidersInitialized;
        float m_BlueGain;
        float m_GreenGain;
        float m_RedGain;

        public float blueGain
        {
            get => m_BlueGain;
            set => m_BlueGain = value;
        }

        public float greenGain
        {
            get => m_GreenGain;
            set => m_GreenGain = value;
        }

        public float redGain
        {
            get => m_RedGain;
            set => m_RedGain = value;
        }

#if UNITY_IOS
        protected override void UpdateCachedValues(ARKitLockedCamera lockedCamera)
        {
            currentMode = lockedCamera.currentWhiteBalanceMode;
            currentValue = lockedCamera.whiteBalance;
        }

        protected override void PopulateSupportedModes(ARKitLockedCamera lockedCamera)
        {
            var supportedModes = lockedCamera.supportedWhiteBalanceModes;
            if ((supportedModes & ARKitWhiteBalanceMode.Locked) != ARKitWhiteBalanceMode.None)
            {
                m_SupportedModes.Add(ARKitWhiteBalanceMode.Locked);
            }
            if ((supportedModes & ARKitWhiteBalanceMode.Auto) != ARKitWhiteBalanceMode.None)
            {
                m_SupportedModes.Add(ARKitWhiteBalanceMode.Auto);
            }
            if ((supportedModes & ARKitWhiteBalanceMode.ContinuousAuto) != ARKitWhiteBalanceMode.None)
            {
                m_SupportedModes.Add(ARKitWhiteBalanceMode.ContinuousAuto);
            }
        }

        protected override void PopulateRanges(ARKitLockedCamera lockedCamera)
        {
            if (!m_BlueGainSlider && !m_GreenGainSlider && !m_RedGainSlider)
                return;

            var range = lockedCamera.whiteBalanceRange;
            Debug.Log($"White Balance Range: {range.minimumGain.ToString()}, {range.maximumGain.ToString()}");

            if (m_BlueGainSlider)
            {
                m_BlueGainSlider.minValue = range.minimumGain;
                m_BlueGainSlider.maxValue = range.maximumGain;

                // It is possible for ARKit to report gain values that are higher than its own reported maximum gain.
                // Therefore when we set our control values, we need to clamp ARKit's data to fit within its own constraints.
                // If our controls try to set a gain value that's higher than the max, even if it is the current gain value, ARKit will throw an exception.
                var blue = Mathf.Clamp(lockedCamera.whiteBalance.blue, 1, lockedCamera.whiteBalanceRange.maximumGain);
                m_BlueGainSlider.SetValueWithoutNotify(blue);
                m_BlueGain = blue;
                m_BlueGainSlider.interactable = true;
            }

            if (m_GreenGainSlider)
            {
                m_GreenGainSlider.minValue = range.minimumGain;
                m_GreenGainSlider.maxValue = range.maximumGain;
                var green = Mathf.Clamp(lockedCamera.whiteBalance.green, 1, lockedCamera.whiteBalanceRange.maximumGain);
                m_GreenGainSlider.SetValueWithoutNotify(green);
                m_GreenGain = green;
                m_GreenGainSlider.interactable = true;
            }

            if (m_RedGainSlider)
            {
                m_RedGainSlider.minValue = range.minimumGain;
                m_RedGainSlider.maxValue = range.maximumGain;
                var red = Mathf.Clamp(lockedCamera.whiteBalance.red, 1, lockedCamera.whiteBalanceRange.maximumGain);
                m_RedGainSlider.SetValueWithoutNotify(red);
                m_RedGain = red;
                m_RedGainSlider.interactable = true;
            }

            m_SlidersInitialized = true;
        }

        protected override void UpdateMode(ARKitLockedCamera lockedCamera, ARKitWhiteBalanceMode mode)
        {
            Debug.Log($"Updating white balance mode to {mode.ToString()}.");
            lockedCamera.requestedWhiteBalanceMode = mode;
        }

        protected override void UpdateConfigValues(ARKitLockedCamera lockedCamera)
        {
            if (!m_SlidersInitialized)
                return;

            var whiteBalance = new ARKitWhiteBalanceGains(
                blue: m_BlueGain,
                green: m_GreenGain,
                red: m_RedGain);

            Debug.Log($"Updating white balance: {whiteBalance.ToString()}");

            lockedCamera.whiteBalance = whiteBalance;
        }
#endif // UNITY_IOS
    }
}
