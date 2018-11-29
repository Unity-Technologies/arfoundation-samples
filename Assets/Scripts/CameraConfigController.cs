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

        // No configurations available probably means this feature
        // isn't supported by the current device.
        if (cameraSubsystem.GetConfigurationCount() == 0)
            return;

        // Here we demonstrate the two ways to enumerate the camera configurations.
        // Typically, you would do this once, perhaps at the start of the AR app
        // rather than every frame.

        // Here, we use a foreach to iterate over all the available configurations
        m_ConfigurationNames.Clear();
        foreach (var config in cameraSubsystem.Configurations())
            m_ConfigurationNames.Add(config.ToString());
        m_Dropdown.AddOptions(m_ConfigurationNames);

        // We can also use a normal for...loop
        var currentConfig = cameraSubsystem.GetCurrentConfiguration();
        for (int i = 0; i < cameraSubsystem.GetConfigurationCount(); i++)
        {
            // Find the current configuration and update the drop down value
            if (currentConfig == cameraSubsystem.GetConfiguration(i))
                m_Dropdown.value = i;
        }
    }
}
