using System;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresCamera : RequiresARSubsystem<XRCameraSubsystem, XRCameraSubsystemDescriptor>
    {
        [SerializeField]
        bool m_RequiresCameraImages;

        [SerializeField]
        bool m_RequiresBasicLightEstimation;

        [SerializeField]
        bool m_RequiresHdrLightEstimation;

        [SerializeField]
        bool m_RequiresCameraGrain;

        [SerializeField]
        bool m_RequiresExifData;

        [SerializeField]
        bool m_RequiresImageStabilization;

        [SerializeField]
        bool m_RequiresCameraTorchMode;

        public override void Evaluate(List<RequirementResult> results)
        {
            base.Evaluate(results);
            if (s_LoadedSubsystem == null)
                return;

            var descriptor = s_LoadedSubsystem.subsystemDescriptor;

            if (m_RequiresCameraImages)
                results.Add(new RequirementResult(descriptor.supportsCameraImage, nameof(m_RequiresCameraImages)));

            if (m_RequiresBasicLightEstimation)
            {
                var supportsColor = descriptor.supportsAverageColorTemperature || descriptor.supportsColorCorrection;
                var supportsBrightness = descriptor.supportsAverageBrightness || descriptor.supportsAverageIntensityInLumens;
                results.Add(new RequirementResult(supportsColor && supportsBrightness, nameof(m_RequiresBasicLightEstimation)));
            }

            if (m_RequiresHdrLightEstimation)
            {
                var supportsHdr =
                    descriptor.supportsFaceTrackingHDRLightEstimation || descriptor.supportsWorldTrackingHDRLightEstimation;
                results.Add(new RequirementResult(supportsHdr, nameof(m_RequiresHdrLightEstimation)));
            }

            if (m_RequiresCameraGrain)
                results.Add(new RequirementResult(descriptor.supportsCameraGrain, nameof(m_RequiresCameraGrain)));

            if (m_RequiresExifData)
                results.Add(new RequirementResult(descriptor.supportsExifData, nameof(m_RequiresExifData)));

            if (m_RequiresImageStabilization)
                results.Add(new RequirementResult(
                    descriptor.supportsImageStabilization != Supported.Unsupported,
                    nameof(m_RequiresImageStabilization)));

            if (m_RequiresCameraTorchMode)
                results.Add(new RequirementResult(descriptor.supportsCameraTorchMode, nameof(m_RequiresCameraTorchMode)));
        }
    }
}
