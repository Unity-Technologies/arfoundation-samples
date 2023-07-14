using System;
using TMPro;
using UnityEngine.UI;
#if UNITY_IOS
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARKit;
#endif // UNITY_IOS

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(ARCameraManager))]
#if UNITY_IOS
    public abstract class ARKitAdvancedCameraConfigurationController<TMode, TConfigValue> : MonoBehaviour
        where TMode : Enum
        where TConfigValue : struct
#else
    public abstract class ARKitAdvancedCameraConfigurationController : MonoBehaviour
#endif
    {
        [SerializeField]
        TMP_Dropdown m_ModeDropdown;

        [SerializeField]
        Button m_UpdateButton;

        [SerializeField]
        GameObject m_UnsupportedMessage;

#if UNITY_IOS
        SupportStatus status;

        ARKitCameraSubsystem m_Subsystem;
        ARKitLockedCamera m_LockedCamera;

        protected List<TMode> m_SupportedModes;
#endif

        public bool cameraLocked
        {
            get
            {
#if UNITY_IOS
                return m_LockedCamera != null;
#else
                return false;
#endif
            }
        }

        public void UpdateConfigValues()
        {
#if UNITY_IOS
            if (!m_Subsystem.TryGetLockedCamera(out var lockedCamera))
            {
                Debug.LogError("Couldn't acquire lock on the camera.");
                return;
            }

            try
            {
                UpdateConfigValues(lockedCamera);
            }
            finally
            {
                lockedCamera.Dispose();
            }
#endif
        }

        public void ToggleLock()
        {
#if UNITY_IOS
            if (cameraLocked)
            {
                m_LockedCamera.Dispose();
                m_LockedCamera = default;
            }
            else
            {
                m_Subsystem.TryGetLockedCamera(out m_LockedCamera);
            }
#endif
        }

#if UNITY_IOS
        public TMode currentMode { get; protected set; }
        public TConfigValue currentValue { get; protected set; }

        void Awake()
        {
            status = SupportStatus.Pending;

            var cameraManager = GetComponent<ARCameraManager>();
            m_Subsystem = cameraManager.subsystem as ARKitCameraSubsystem;

            if (m_Subsystem == null)
            {
                Debug.LogWarning(
                    $"No active instance of {nameof(ARKitCameraSubsystem)} found. {GetType().Name} will be disabled.");
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

            // Populate UI controls
            StartCoroutine(PopulateControls());
        }

        void OnDisable()
        {
            if (m_LockedCamera != null)
            {
                m_LockedCamera.Dispose();
            }
        }

        void Update()
        {
            UpdateCurrentState();

            // Update the UI controls with current values
            RefreshCurrentMode();

            PostUpdate();
        }

        IEnumerator CheckSupport()
        {
            // wait for one frame to allow the subsystems and plug-in to initialize before checking support
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
                $"Advance camera configuration is not supported on this device. {GetType().Name} will be disabled.");
            enabled = false;
        }

        void UpdateCurrentState()
        {
            if (!m_Subsystem.TryGetLockedCamera(out var lockedCamera))
            {
                Debug.LogError("Couldn't acquire lock on the camera.");
                return;
            }

            using (lockedCamera)
            {
                UpdateCachedValues(lockedCamera);
            }
        }

        IEnumerator PopulateControls()
        {
            // wait to check support and the platform plug-in to initialize
            yield return new WaitWhile(() => status == SupportStatus.Pending || status == SupportStatus.Checking);

            if (status == SupportStatus.Unsupported)
            {
                yield break;
            }

            if (!m_Subsystem.TryGetLockedCamera(out var lockedCamera))
            {
                Debug.LogError("Couldn't acquire lock on the camera.");
                yield break;
            }

            using (lockedCamera)
            {
                PopulateDropdownModes(lockedCamera);
                PopulateRanges(lockedCamera);
            }
        }

        void PopulateDropdownModes(ARKitLockedCamera lockedCamera)
        {
            if (m_ModeDropdown == null)
                return;

            var options = m_ModeDropdown.options;
            options.Clear();

            PopulateSupportedModes(lockedCamera);

            foreach (var mode in m_SupportedModes)
            {
                options.Add(new TMP_Dropdown.OptionData(mode.ToString()));
            }

            m_ModeDropdown.RefreshShownValue();
            m_ModeDropdown.onValueChanged.AddListener(UpdateMode);
        }

        void RefreshCurrentMode()
        {
            var mode = currentMode;
            if (m_SupportedModes[m_ModeDropdown.value].Equals(mode))
                return;

            var index = m_SupportedModes.IndexOf(mode);

            if (index < 0)
            {
                Debug.LogError($"The {nameof(TMode)} - {mode} is not supported on this device.");
                return;
            }

            m_ModeDropdown.SetValueWithoutNotify(index);
            m_UpdateButton.interactable = InteractableUpdateButton(mode);
        }

        void UpdateMode(int mode)
        {
            var modeEnum = m_SupportedModes[mode];

            if (!m_Subsystem.TryGetLockedCamera(out var lockedCamera))
            {
                Debug.LogError("Couldn't acquire lock on the camera.");
                return;
            }

            try
            {
                UpdateMode(lockedCamera, modeEnum);
            }
            finally
            {
                lockedCamera.Dispose();
                m_UpdateButton.interactable = InteractableUpdateButton(modeEnum);
            }
        }

        protected virtual bool InteractableUpdateButton(TMode mode) => true;
        protected virtual void PostUpdate() { }

        protected abstract void PopulateSupportedModes(ARKitLockedCamera lockedCamera);
        protected abstract void UpdateCachedValues(ARKitLockedCamera lockedCamera);

        protected abstract void PopulateRanges(ARKitLockedCamera lockedCamera);
        protected abstract void UpdateConfigValues(ARKitLockedCamera lockedCamera);
        protected abstract void UpdateMode(ARKitLockedCamera lockedCamera, TMode mode);

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
