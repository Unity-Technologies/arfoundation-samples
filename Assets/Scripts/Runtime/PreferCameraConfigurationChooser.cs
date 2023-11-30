using System;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// A custom implementation of a <see cref="ConfigurationChooser"/>.
    /// </summary>
    public class PreferCameraConfigurationChooser : ConfigurationChooser
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
            var requestedCameraFeatures = requestedFeatures.Intersection(Feature.AnyCamera);

            var highestFeatureWeight = -1;
            var highestRank = int.MinValue;
            ConfigurationDescriptor bestDescriptor = default;
            foreach (var descriptor in descriptors)
            {
                // Initialize the feature weight with each feature being an equal weight.
                var featureWeight = requestedFeatures.Intersection(descriptor.capabilities).Count();

                // Increase the weight if there are matching camera features.
                if (requestedCameraFeatures.Any(descriptor.capabilities))
                {
                    featureWeight += 100;
                }

                // Store the descriptor with the highest feature weight.
                if ((featureWeight <= highestFeatureWeight)
                    && (featureWeight != highestFeatureWeight || descriptor.rank <= highestRank))
                    continue;

                highestFeatureWeight = featureWeight;
                highestRank = descriptor.rank;
                bestDescriptor = descriptor;
            }

            // Return the configuration with the best matching descriptor.
            return new Configuration(bestDescriptor, requestedFeatures.Intersection(bestDescriptor.capabilities));
        }
    }
}
