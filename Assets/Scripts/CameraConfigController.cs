using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARExtensions;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(Dropdown))]
public class CameraConfigController : MonoBehaviour
{
    List<string> m_ConfigurationNames;

    Dropdown m_Dropdown;

    public void OnValueChanged(Dropdown dropdown)
    {
        var cameraSubsystem = ARSubsystemManager.cameraSubsystem;
        if (cameraSubsystem == null)
            return;

        var configurationIndex = dropdown.value;

        // Check that the value makes sense
        if (configurationIndex >= cameraSubsystem.GetConfigurationCount())
            return;

        // Get that configuration by index
        var configuration = cameraSubsystem.GetConfiguration(configurationIndex);

        // Make it the active one
        cameraSubsystem.SetConfiguration(configuration);
    }

    void Awake()
    {
        m_Dropdown = GetComponent<Dropdown>();
        m_ConfigurationNames = new List<string>();
    }

    void Update()
    {
        m_Dropdown.ClearOptions();

        var cameraSubsystem = ARSubsystemManager.cameraSubsystem;
        if (cameraSubsystem == null)
            return;

        // Typically, you would only need to do this once.
        m_ConfigurationNames.Clear();
        foreach (var config in cameraSubsystem.Configurations())
            m_ConfigurationNames.Add(config.ToString());

        m_Dropdown.AddOptions(m_ConfigurationNames);
    }
}
