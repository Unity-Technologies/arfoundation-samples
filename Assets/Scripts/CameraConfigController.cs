using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARExtensions;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Populates a drop down UI element with all the supported
/// camera configurations and changes the active camera
/// configuration when the user changes the selection in the dropdown.
/// 
/// The camera configuration affects the resolution (and possibly framerate)
/// of the hardware camera during an AR session.
/// </summary>
[RequireComponent(typeof(Dropdown))]
public class CameraConfigController : MonoBehaviour
{
    List<string> m_ConfigurationNames;

    Dropdown m_Dropdown;

    /// <summary>
    /// Callback invoked when <see cref="m_Dropdown"/> changes. This
    /// lets us change the camera configuration when the user changes
    /// the selection in the UI.
    /// </summary>
    /// <param name="dropdown">The <c>Dropdown</c> which changed.</param>
    public void OnValueChanged(Dropdown dropdown)
    {
        var cameraSubsystem = ARSubsystemManager.cameraSubsystem;
        if (cameraSubsystem == null)
            return;

        var configurationIndex = dropdown.value;

        // Check that the value makes sense
        var configurations = cameraSubsystem.Configurations();
        if (configurationIndex >= configurations.count)
            return;

        // Get that configuration by index
        var configuration = configurations[configurationIndex];

        // Make it the active one
        cameraSubsystem.SetCurrentConfiguration(configuration);
    }

    void Awake()
    {
        m_Dropdown = GetComponent<Dropdown>();
        m_Dropdown.ClearOptions();
        m_ConfigurationNames = new List<string>();
    }

    void PopulateDropdown()
    {
        var cameraSubsystem = ARSubsystemManager.cameraSubsystem;
        if (cameraSubsystem == null)
            return;

        // No configurations available probably means this feature
        // isn't supported by the current device.
        var configurations = cameraSubsystem.Configurations();
        if (configurations.count == 0)
            return;

        // There are two ways to enumerate the camera configurations.

        // 1. Use a foreach to iterate over all the available configurations
        foreach (var config in configurations)
            m_ConfigurationNames.Add(config.ToString());
        m_Dropdown.AddOptions(m_ConfigurationNames);

        // 2. Use a normal for...loop
        var currentConfig = cameraSubsystem.GetCurrentConfiguration();
        for (int i = 0; i < configurations.count; i++)
        {
            // Find the current configuration and update the drop down value
            if (currentConfig == configurations[i])
                m_Dropdown.value = i;
        }
    }

    void Update()
    {
        if (m_ConfigurationNames.Count == 0)
            PopulateDropdown();
    }
}
