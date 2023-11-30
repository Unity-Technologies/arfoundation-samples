using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class CustomConfigurationChooser : MonoBehaviour
    {
        /// <summary>
        /// The configuration chooser mode. Values must match the UI dropdown.
        /// </summary>
        enum ConfigurationChooserMode
        {
            Default = 0,
            PreferCamera = 1,
        }

        /// <summary>
        /// The AR session on which to set the configuration chooser.
        /// </summary>
        public ARSession session
        {
            get => m_Session;
            set => m_Session = value;
        }

        [SerializeField]
        ARSession m_Session;

        /// <summary>
        /// The instantiated instance of the default configuration chooser.
        /// </summary>
        static readonly ConfigurationChooser m_DefaultConfigurationChooser = new DefaultConfigurationChooser();

        /// <summary>
        /// The instantiated instance of the prefer camera configuration chooser.
        /// </summary>
        static readonly ConfigurationChooser m_PreferCameraConfigurationChooser = new PreferCameraConfigurationChooser();

        /// <summary>
        /// The current configuration chooser mode.
        /// </summary>
        ConfigurationChooserMode m_ConfigurationChooserMode = ConfigurationChooserMode.Default;

        void Start()
        {
            UpdateConfigurationChooser();
        }

        /// <summary>
        /// Callback when the depth mode dropdown UI has a value change.
        /// </summary>
        /// <param name="dropdown">The dropdown UI that changed.</param>
        public void OnDepthModeDropdownValueChanged(Dropdown dropdown)
        {
            // Update the display mode from the dropdown value.
            m_ConfigurationChooserMode = (ConfigurationChooserMode)dropdown.value;

            // Update the configuration chooser.
            UpdateConfigurationChooser();
        }

        /// <summary>
        /// Update the configuration chooser based on the current mode.
        /// </summary>
        void UpdateConfigurationChooser()
        {
            Debug.Assert(m_Session != null, "ARSession must be nonnull");
            Debug.Assert(m_Session.subsystem != null, "ARSession must have a subsystem");
            switch (m_ConfigurationChooserMode)
            {
                case ConfigurationChooserMode.PreferCamera:
                    m_Session.subsystem.configurationChooser = m_PreferCameraConfigurationChooser;
                    break;
                case ConfigurationChooserMode.Default:
                default:
                    m_Session.subsystem.configurationChooser = m_DefaultConfigurationChooser;
                    break;
            }
        }
    }
}
