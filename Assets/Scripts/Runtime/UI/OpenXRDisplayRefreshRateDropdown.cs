using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class OpenXRDisplayRefreshRateDropdown : MonoBehaviour
    {
        [SerializeField]
        TMP_Dropdown m_Dropdown;

        [SerializeField]
        TextMeshProUGUI m_CurrentRefreshRateLabel;

        [SerializeField]
        LoadingVisualizer m_LoadingVisualizer;

        IDisplayRefreshRateUtility m_DisplayRefreshRateUtility;
        XRDisplaySubsystem m_DisplaySubsystem;
        NativeArray<float> m_DisplayRefreshRates;
        float m_ChosenDisplayRefreshRate;

        void Reset()
        {
            TryInitializeSerializedFields();
        }

        void Awake()
        {
            if (m_Dropdown == null)
                TryInitializeSerializedFields();
        }

        void Start()
        {
            m_LoadingVisualizer.StartAnimating();

            if (!SubsystemsUtility.TryGetLoadedSubsystem<XRDisplaySubsystem, XRDisplaySubsystem>(out m_DisplaySubsystem))
            {
                Debug.LogError(
                    $"No {nameof(XRDisplaySubsystem)} is loaded. {nameof(OpenXRDisplayRefreshRateDropdown)} will have no effect.",
                    this);
                enabled = false;
                return;
            }

            if (!DisplayRefreshRateUtilityFactory.TryCreate(out m_DisplayRefreshRateUtility))
            {
                Debug.LogError("Failed to construct appropriate refresh rate utility. Display refresh rate dropdown will not function. Confirm that you have the correct subsystem provider package installed for your target platform.");
                enabled = false;
                return;
            }

            if (!m_DisplayRefreshRateUtility.IsDisplayUtilitiesFeatureEnabled())
            {
                LogFeatureNotEnabledMessage();
                return;
            }

            m_Dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
            PopulateDropdown();
        }

        void Update()
        {
            if (m_CurrentRefreshRateLabel != null && m_DisplaySubsystem.TryGetDisplayRefreshRate(out var currentRefreshRate))
            {
                m_CurrentRefreshRateLabel.text = $"Current display refresh rate: {currentRefreshRate} Hz";
            }
        }

        void LogFeatureNotEnabledMessage()
        {
            string featureName = string.Empty;

            featureName = m_DisplayRefreshRateUtility.displayUtilitiesFeatureName ?? "Display refresh utility";

            Debug.LogError(
                $"{featureName} is not enabled. {nameof(OpenXRDisplayRefreshRateDropdown)} will have no effect.",
                this);

            enabled = false;
        }

        void OnDropdownValueChanged(int newIndex)
        {
            if (!enabled)
                return;

            m_ChosenDisplayRefreshRate = m_DisplayRefreshRates[newIndex];
            if (!m_DisplaySubsystem.TryGetDisplayRefreshRate(out var currentRefreshRate))
            {
                Debug.LogError("Failed to get the selected display refresh rate from the native provider.", this);
                ChangeToChosenDisplayRefreshRate();
                return;
            }

            if (!ApproximatelyEqual(currentRefreshRate, m_ChosenDisplayRefreshRate))
            {
                ChangeToChosenDisplayRefreshRate();
            }
        }

        void ChangeToChosenDisplayRefreshRate()
        {
            if (m_DisplayRefreshRateUtility == null)
            {
                Debug.LogError($"Failed to set refresh rate to {m_ChosenDisplayRefreshRate}");
                return;
            }

            var status = m_DisplayRefreshRateUtility.TrySetDisplayRefreshRate(m_ChosenDisplayRefreshRate, m_DisplaySubsystem);
            if (status.IsError())
            {
                Debug.LogError($"Failed to set refresh rate to {m_ChosenDisplayRefreshRate}");
            }
        }

        void PopulateDropdown()
        {
            if (m_DisplayRefreshRateUtility == null)
                return;

            var status = m_DisplayRefreshRateUtility.TryGetSupportedDisplayRefreshRates(Allocator.Persistent, out m_DisplayRefreshRates, m_DisplaySubsystem);
            if (status.IsError())
            {
                return;
            }

            if (!m_DisplaySubsystem.TryGetDisplayRefreshRate(out var currentRefreshRate))
            {
                Debug.LogWarning("Failed to get display refresh rates. Setting refresh rate dropdown index to 0");
            }

            if (m_DisplayRefreshRates.Length == 0)
            {
                Debug.LogWarning("Found 0 available display refresh rates from the provider. Disabling display refresh rate selection dropdown.");
                m_Dropdown.interactable = false;
                return;
            }

            var dropdownItems = new List<string>();
            var dropdownIndex = 0;
            for (var i = 0; i < m_DisplayRefreshRates.Length; i++)
            {
                dropdownItems.Add(m_DisplayRefreshRates[i].ToString());
                if (ApproximatelyEqual(currentRefreshRate, m_DisplayRefreshRates[i]))
                {
                    dropdownIndex = i;
                }
            }
            m_Dropdown.AddOptions(dropdownItems);
            m_Dropdown.value = dropdownIndex;

        }

        void OnDestroy()
        {
            m_LoadingVisualizer.StopAnimating();

            if (m_DisplayRefreshRates.IsCreated)
                m_DisplayRefreshRates.Dispose();
        }

        /// <summary>
        /// This method is used to check for display refresh rate equality because display refresh rates can be off by a factor of tenths.
        /// </summary>
        static bool ApproximatelyEqual(float a, float b)
        {
            return Mathf.Abs(a - b) < 1;
        }

        [ContextMenu("Try Initialize Serialized Fields")]
        void TryInitializeSerializedFields()
        {
            m_Dropdown = GetComponentInChildren<TMP_Dropdown>();
        }
    }
}
