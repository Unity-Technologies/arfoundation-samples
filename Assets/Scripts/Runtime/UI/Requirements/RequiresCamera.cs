using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
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

        public override bool Evaluate()
        {
            if (!base.Evaluate())
                return false;

            var descriptor = s_LoadedSubsystem.subsystemDescriptor;

            if (m_RequiresCameraImages && !descriptor.supportsCameraImage)
                return false;

            if (m_RequiresBasicLightEstimation)
            {
                bool supportsBasics = descriptor.supportsCameraConfigurations && descriptor.supportsCameraImage;
                bool supportsColor = descriptor.supportsAverageColorTemperature || descriptor.supportsColorCorrection;
                bool supportsBrightness = descriptor.supportsAverageBrightness || descriptor.supportsAverageIntensityInLumens;

                if (!(supportsBasics && supportsColor && supportsBrightness))
                    return false;
            }

            if (m_RequiresHdrLightEstimation)
            {
                if (!(descriptor.supportsFaceTrackingHDRLightEstimation || descriptor.supportsWorldTrackingHDRLightEstimation))
                    return false;
            }

            if (m_RequiresCameraGrain && !descriptor.supportsCameraGrain)
                return false;

            if (m_RequiresExifData && !descriptor.supportsExifData)
                return false;

            if (m_RequiresImageStabilization && descriptor.supportsImageStabilization == Supported.Unsupported)
                return false;

            if (m_RequiresCameraTorchMode && !descriptor.supportsCameraTorchMode)
                return false;

            return true;
        }
    }
}
