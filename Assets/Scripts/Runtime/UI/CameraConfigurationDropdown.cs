using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class CameraConfigurationDropdown : MonoBehaviour
    {
        [SerializeField]
        ARCameraManager m_CameraManager;

        [SerializeField]
        TMP_Dropdown m_CameraConfigurationDropdown;

        XRCameraConfiguration m_CurrentCameraConfiguration;

        void Reset()
        {
            TryInitializeSerializedFields();
        }

        void Awake()
        {
            if (m_CameraManager == null)
                TryInitializeSerializedFields();

            m_CameraConfigurationDropdown.onValueChanged.AddListener(delegate { OnCameraDropdownValueChanged(m_CameraConfigurationDropdown); });
            StartCoroutine(PopulateCameraDropdown());
        }

        void LateUpdate()
        {
            var cameraConfigOption = m_CameraManager.currentConfiguration;
            if(cameraConfigOption.HasValue && cameraConfigOption.Value != m_CurrentCameraConfiguration)
                m_CurrentCameraConfiguration = cameraConfigOption.Value;
        }

        void OnCameraDropdownValueChanged(TMP_Dropdown dropdown)
        {
            var configurationIndex = dropdown.value;
            using var configurations = m_CameraManager.GetConfigurations(Allocator.Temp);
            var configuration = configurations[configurationIndex];
            m_CameraManager.currentConfiguration = configuration;
        }

        IEnumerator PopulateCameraDropdown()
        {
            yield return null;

            if (m_CameraManager == null || m_CameraManager.subsystem is not {running: true})
                yield break;

            using var configurations = m_CameraManager.GetConfigurations(Allocator.Temp);

            var configurationNames = new List<string>();
            foreach (var config in configurations)
            {
                configurationNames.Add($"{config.width}x{config.height}{(config.framerate.HasValue ? $" at {config.framerate.Value} Hz" : "")}");
            }
            m_CameraConfigurationDropdown.AddOptions(configurationNames);

            var currentConfig = m_CameraManager.currentConfiguration;
            for (int i = 0; i < configurations.Length; ++i)
            {
                if (currentConfig == configurations[i])
                {
                    m_CameraConfigurationDropdown.value = i;
                }
            }
        }

        [ContextMenu("Try Initialize Serialized Fields")]
        void TryInitializeSerializedFields()
        {
            m_CameraManager = FindAnyObjectByType<ARCameraManager>();
            m_CameraConfigurationDropdown = GetComponentInChildren<TMP_Dropdown>();
        }
    }
}
