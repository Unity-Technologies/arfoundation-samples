using System;
using TMPro;
using UnityEngine.UI;
#if UNITY_IOS
using System.Collections;
using UnityEngine.XR.ARKit;
#endif // UNITY_IOS

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(ARCameraManager))]
    public class CameraExposureController : MonoBehaviour
    {
        [SerializeField]
        TMP_Dropdown m_ExposureModeDropdown;

        [SerializeField]
        Slider m_DurationSlider;

        [SerializeField]
        Slider m_IsoSlider;

        [SerializeField]
        Button m_UpdateButton;

        [SerializeField]
        GameObject m_UnsupportedMessage;

#if UNITY_IOS
        double m_Duration;
        float m_Iso;

        SupportStatus status;

        ARKitCameraSubsystem m_Subsystem;
        ARKitLockedCamera m_LockedCamera;

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

        public bool cameraLocked => m_LockedCamera != null;

        public ARKitExposureMode currentExposureMode { get; private set; }
        public ARKitExposure currentExposure { get; private set; }

        void Awake()
        {
            status = SupportStatus.Pending;

            var cameraManager = GetComponent<ARCameraManager>();
            m_Subsystem = cameraManager.subsystem as ARKitCameraSubsystem;

            if (m_Subsystem == null)
            {
                Debug.LogWarning(
                    $"No active instance of {nameof(ARKitCameraSubsystem)} found. {nameof(CameraExposureController)} will be disabled.");
                enabled = false;
            }
        }

        void OnEnable()
        {
            if (m_UnsupportedMessage)
            {
                m_UnsupportedMessage.SetActive(false);
            }

            status = SupportStatus.Checking;
            StartCoroutine(CheckSupport());

            // Update UI controls
            PopulateExposureMode();
            StartCoroutine(PopulateExposureRange());
        }

        private void OnDisable()
        {
            if (m_LockedCamera != null)
            {
                m_LockedCamera.Dispose();
            }
        }

        void Update()
        {
            UpdateCurrentExposureState();

            // Update the UI controls with current values
            RefreshCurrentExposureMode();

            if (currentExposureMode != ARKitExposureMode.Custom)
            {
                RefreshCurrentExposure(currentExposure);
            }
        }

        void UpdateCurrentExposureState()
        {
            if (!m_Subsystem.TryGetLockedCamera(out var lockedCamera))
                return;

            using (lockedCamera)
            {
                currentExposureMode = lockedCamera.currentExposureMode;
                currentExposure = lockedCamera.exposure;
            }
        }

        void PopulateExposureMode()
        {
            if (m_ExposureModeDropdown == null)
                return;

            var options = m_ExposureModeDropdown.options;
            options.Clear();

            options.Add(new TMP_Dropdown.OptionData(ARKitExposureMode.Locked.ToString()));
            options.Add(new TMP_Dropdown.OptionData(ARKitExposureMode.Auto.ToString()));
            options.Add(new TMP_Dropdown.OptionData(ARKitExposureMode.ContinuousAuto.ToString()));
            options.Add(new TMP_Dropdown.OptionData(ARKitExposureMode.Custom.ToString()));

            m_ExposureModeDropdown.onValueChanged.AddListener(UpdateExposureMode);
        }

        IEnumerator CheckSupport()
        {
            yield return null;

            if (m_Subsystem.advancedCameraConfigurationSupported)
            {
                status = SupportStatus.Supported;
                yield break;
            }

            status = SupportStatus.Unsupported;

            if (m_UnsupportedMessage)
            {
                m_UnsupportedMessage.SetActive(true);
            }

            Debug.LogWarning(
                $"Advance camera configuration is not supported on this device. {nameof(CameraExposureController)} will be disabled.");
            enabled = false;
        }

        IEnumerator PopulateExposureRange()
        {
            if (!m_DurationSlider && !m_IsoSlider)
            {
                yield break;
            }

            // wait to check support and the platform plug-in to initialize
            yield return new WaitWhile(() => status == SupportStatus.Pending || status == SupportStatus.Checking);

            if (status == SupportStatus.Unsupported)
            {
                yield break;
            }

            if (!m_Subsystem.TryGetLockedCamera(out var lockedCamera))
            {
                Debug.LogError("Couldn't acquire lock on the camera to query exposure range.");
                yield break;
            }

            using (lockedCamera)
            {
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
        }

        void RefreshCurrentExposureMode()
        {
            var exposureMode = (int) currentExposureMode - 1;
            if (m_ExposureModeDropdown.value == exposureMode)
                return;

            m_ExposureModeDropdown.SetValueWithoutNotify(exposureMode);
            m_UpdateButton.interactable = ((ARKitExposureMode)exposureMode + 1) == ARKitExposureMode.Custom;
        }

        void RefreshCurrentExposure(ARKitExposure exposure)
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

        void UpdateExposureMode(int mode)
        {
            var exposureMode = (ARKitExposureMode)mode + 1;

            if (!m_Subsystem.TryGetLockedCamera(out var lockedCamera))
                return;

            try
            {
                Debug.Log($"Updating exposure mode to {exposureMode.ToString()}.");
                lockedCamera.requestedExposureMode = exposureMode;
            }
            finally
            {
                lockedCamera.Dispose();
                m_UpdateButton.interactable = exposureMode == ARKitExposureMode.Custom;
            }
        }

        public void UpdateExposure()
        {
            if (!m_Subsystem.TryGetLockedCamera(out var lockedCamera))
                return;

            try
            {
                lockedCamera.exposure = new ARKitExposure(m_Duration, m_Iso);
            }
            finally
            {
                lockedCamera.Dispose();
            }
        }

        public void ToggleLock()
        {
            if (cameraLocked)
            {
                m_LockedCamera.Dispose();
                m_LockedCamera = default;
            }
            else
            {
                m_Subsystem.TryGetLockedCamera(out m_LockedCamera);
            }
        }

        enum SupportStatus
        {
            Pending,
            Checking,
            Supported,
            Unsupported
        }
#endif // UNITY_IOS
    }
}
