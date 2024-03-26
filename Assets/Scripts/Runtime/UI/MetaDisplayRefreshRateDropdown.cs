using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
#if METAOPENXR_0_2_OR_NEWER && UNITY_ANDROID
using UnityEngine.XR.OpenXR.Features.Meta;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MetaDisplayRefreshRateDropdown : MonoBehaviour
    {
        [SerializeField]
        TMP_Dropdown m_Dropdown;

        [SerializeField]
        TextMeshProUGUI m_CurrentRefreshRateLabel;

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
            if (!SubsystemsUtility.TryGetLoadedSubsystem<XRDisplaySubsystem, XRDisplaySubsystem>(out m_DisplaySubsystem))
            {
                Debug.LogError(
                    $"No {nameof(XRDisplaySubsystem)} is loaded. {nameof(MetaDisplayRefreshRateDropdown)} will have no effect.",
                    this);

                enabled = false;
                return;
            }

#if UNITY_EDITOR || !UNITY_ANDROID
            Debug.LogError($"{nameof(MetaDisplayRefreshRateDropdown)} is only supported on the Android platform.", this);
            enabled = false;
            return;
#elif METAOPENXR_0_2_OR_NEWER
            if (!OpenXRUtility.IsOpenXRFeatureEnabled<DisplayUtilitiesFeature>())
            {
                Debug.LogError(
                    $"{DisplayUtilitiesFeature.displayName} is not enabled. {nameof(MetaDisplayRefreshRateDropdown)} will have no effect.",
                    this);

                enabled = false;
                return;
            }

            m_Dropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(m_Dropdown); });
            StartCoroutine(PopulateDropdown());

#else
            Debug.LogError($"{nameof(MetaDisplayRefreshRateDropdown)} requires the package com.unity.xr.meta-openxr", this);
            enabled = false;
            return;
#endif
        }

        void Update()
        {
            if (m_CurrentRefreshRateLabel != null && m_DisplaySubsystem.TryGetDisplayRefreshRate(out var currentRefreshRate))
            {
                m_CurrentRefreshRateLabel.text = $"Current display refresh rate: {currentRefreshRate} Hz";
            }
        }

        void OnDropdownValueChanged(TMP_Dropdown dropdown)
        {
#if METAOPENXR_0_2_OR_NEWER && UNITY_ANDROID
            if (!enabled)
                return;

            int index = dropdown.value;
            m_ChosenDisplayRefreshRate = m_DisplayRefreshRates[index];
            if (m_DisplaySubsystem.TryGetDisplayRefreshRate(out var currentRefreshRate))
            {
                if (!ApproximatelyEqual(currentRefreshRate, m_ChosenDisplayRefreshRate))
                    m_DisplaySubsystem.TryRequestDisplayRefreshRate(m_ChosenDisplayRefreshRate);
            }
            else
            {
                Debug.LogWarning("Failed to get current display refresh rate.", this);
                m_DisplaySubsystem.TryRequestDisplayRefreshRate(m_ChosenDisplayRefreshRate);
            }
#endif
        }

        IEnumerator PopulateDropdown()
        {
            yield return null;

#if METAOPENXR_0_2_OR_NEWER && UNITY_ANDROID
            if (!m_DisplaySubsystem.TryGetSupportedDisplayRefreshRates(Allocator.Persistent, out m_DisplayRefreshRates))
                yield break;

            var dropdownItems = new List<string>();
            foreach (var rate in m_DisplayRefreshRates)
            {
                dropdownItems.Add($"{rate} Hz");
            }
            m_Dropdown.AddOptions(dropdownItems);

            if (m_DisplaySubsystem.TryGetDisplayRefreshRate(out var currentRefreshRate))
            {
                for (int i = 0; i < m_DisplayRefreshRates.Length; ++i)
                {
                    if (ApproximatelyEqual(currentRefreshRate, m_DisplayRefreshRates[i]))
                    {
                        m_Dropdown.value = i;
                    }
                }
            }
#endif
        }

        void OnDestroy()
        {
            if (m_DisplayRefreshRates.IsCreated)
                m_DisplayRefreshRates.Dispose();
        }

        /// <summary>
        /// Actual display refresh rates can be off by a factor of tenths
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
