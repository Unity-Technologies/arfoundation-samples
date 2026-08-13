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
                results.Add(new RequirementResult(
                    descriptor.supportsCameraImage,
                    "Camera Images",
                    k_SubRequirementRemediationText));

            if (m_RequiresBasicLightEstimation)
            {
                var supportsColor = descriptor.supportsAverageColorTemperature || descriptor.supportsColorCorrection;
                var supportsBrightness =
                    descriptor.supportsAverageBrightness ||
                    descriptor.supportsAverageIntensityInLumens;
                results.Add(new RequirementResult(
                    supportsColor && supportsBrightness,
                    "Basic Light Estimation",
                    k_SubRequirementRemediationText));
            }

            if (m_RequiresHdrLightEstimation)
            {
                var supportsHdr =
                    descriptor.supportsFaceTrackingHDRLightEstimation ||
                    descriptor.supportsWorldTrackingHDRLightEstimation;
                results.Add(new RequirementResult(
                    supportsHdr,
                    "HDR Light Estimation",
                    k_SubRequirementRemediationText));
            }

            if (m_RequiresCameraGrain)
                results.Add(new RequirementResult(
                    descriptor.supportsCameraGrain,
                    "Camera Grain",
                    k_SubRequirementRemediationText));

            if (m_RequiresExifData)
                results.Add(new RequirementResult(
                    descriptor.supportsExifData,
                    "EXIF Data",
                    k_SubRequirementRemediationText));

            if (m_RequiresImageStabilization)
                results.Add(new RequirementResult(
                    descriptor.supportsImageStabilization != Supported.Unsupported,
                    "Image Stabilization",
                    k_SubRequirementRemediationText));

            if (m_RequiresCameraTorchMode)
                results.Add(new RequirementResult(
                    descriptor.supportsCameraTorchMode,
                    "Camera Torch",
                    k_SubRequirementRemediationText));
        }
    }
}
