using System;
using Unity.Collections;
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

        /// <summary>
        /// A custom implementation of a <see cref="ConfigurationChooser"/>.
        /// </summary>
        class PreferCameraConfigurationChooser : ConfigurationChooser
        {
            /// <summary>
            /// Selects a configuration from the given <paramref name="descriptors"/> and <paramref name="requestedFeatures"/>.
            /// </summary>
            public override Configuration ChooseConfiguration(NativeSlice<ConfigurationDescriptor> descriptors, Feature requestedFeatures)
            {
                if (descriptors.Length == 0)
                {
                    throw new ArgumentException("No configuration descriptors to choose from.", nameof(descriptors));
                }

                if (requestedFeatures.Intersection(Feature.AnyTrackingMode).Count() > 1)
                {
                    throw new ArgumentException($"Zero or one tracking mode must be requested. Requested tracking modes => {requestedFeatures.Intersection(Feature.AnyTrackingMode).ToStringList()}", nameof(requestedFeatures));
                }

                if (requestedFeatures.Intersection(Feature.AnyCamera).Count() > 1)
                {
                    throw new ArgumentException($"Zero or one camera mode must be requested. Requested camera modes => {requestedFeatures.Intersection(Feature.AnyCamera).ToStringList()}", nameof(requestedFeatures));
                }

                // Get the requested camera features out of the set of requested features.
                Feature requestedCameraFeatures = requestedFeatures.Intersection(Feature.AnyCamera);

                int highestFeatureWeight = -1;
                int highestRank = int.MinValue;
                ConfigurationDescriptor bestDescriptor = default;
                foreach (var descriptor in descriptors)
                {
                    // Initialize the feature weight with each feature being an equal weight.
                    int featureWeight = requestedFeatures.Intersection(descriptor.capabilities).Count();

                    // Increase the weight if there are matching camera features.
                    if (requestedCameraFeatures.Any(descriptor.capabilities))
                    {
                        featureWeight += 100;
                    }

                    // Store the descriptor with the highest feature weight.
                    if ((featureWeight > highestFeatureWeight) ||
                        (featureWeight == highestFeatureWeight && descriptor.rank > highestRank))
                    {
                        highestFeatureWeight = featureWeight;
                        highestRank = descriptor.rank;
                        bestDescriptor = descriptor;
                    }
                }

                // Return the configuration with the best matching descriptor.
                return new Configuration(bestDescriptor, requestedFeatures.Intersection(bestDescriptor.capabilities));
            }
        }
    }
}
