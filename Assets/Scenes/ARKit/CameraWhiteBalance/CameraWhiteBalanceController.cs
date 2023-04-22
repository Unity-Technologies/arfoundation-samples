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

#if UNITY_IOS
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

        protected override void UpdateCachedValues(ARKitLockedCamera lockedCamera)
        {
            currentMode = lockedCamera.currentWhiteBalanceMode;
            currentValue = lockedCamera.whiteBalance;
        }

        protected override void PopulateSupportedModes(ARKitLockedCamera lockedCamera)
        {
            var supportedModes = lockedCamera.supportedWhiteBalanceModes;
            m_SupportedModes = new();

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
            {
                return;
            }

            var range = lockedCamera.whiteBalanceRange;
            Debug.Log($"White Balance Range: {range.minimumGain}, {range.maximumGain}");

            // update range of blue gain slider
            if (m_BlueGainSlider)
            {
                m_BlueGainSlider.minValue = range.minimumGain;
                m_BlueGainSlider.maxValue = range.maximumGain;
            }

            // update range of green gain slider
            if (m_GreenGainSlider)
            {
                m_GreenGainSlider.minValue = range.minimumGain;
                m_GreenGainSlider.maxValue = range.maximumGain;
            }

            // update range of red gain slider
            if (m_RedGainSlider)
            {
                m_RedGainSlider.minValue = range.minimumGain;
                m_RedGainSlider.maxValue = range.maximumGain;
            }

            RefreshUI(lockedCamera.whiteBalance);
        }

        void RefreshUI(ARKitWhiteBalanceGains whiteBalance)
        {
            if (m_BlueGainSlider != null && Math.Abs(m_BlueGainSlider.value - whiteBalance.blue) > float.Epsilon)
            {
                blueGain = whiteBalance.blue;
                m_BlueGainSlider.SetValueWithoutNotify(whiteBalance.blue);
            }

            if (m_GreenGainSlider != null && Math.Abs(m_GreenGainSlider.value - whiteBalance.green) > float.Epsilon)
            {
                greenGain = whiteBalance.green;
                m_GreenGainSlider.SetValueWithoutNotify(whiteBalance.green);
            }

            if (m_RedGainSlider != null && Math.Abs(m_RedGainSlider.value - whiteBalance.red) > float.Epsilon)
            {
                redGain = whiteBalance.red;
                m_RedGainSlider.SetValueWithoutNotify(whiteBalance.red);
            }
        }

        protected override void UpdateMode(ARKitLockedCamera lockedCamera, ARKitWhiteBalanceMode mode)
        {
            Debug.Log($"Updating white balance mode to {mode.ToString()}.");
            lockedCamera.requestedWhiteBalanceMode = mode;
        }

        protected override void UpdateConfigValues(ARKitLockedCamera lockedCamera)
        {
            var whiteBalance = new ARKitWhiteBalanceGains(m_BlueGain, m_GreenGain, m_RedGain);
            Debug.Log($"Updating white balance: {whiteBalance.ToString()}");

            lockedCamera.whiteBalance = whiteBalance;
        }
#endif // UNITY_IOS
    }
}
