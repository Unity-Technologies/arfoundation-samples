using System.Collections;
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

        protected override IEnumerator Start()
        {
            yield return base.Start();

            if (m_Button.interactable == false)
                yield break;

            var descriptor = s_LoadedSubsystem.subsystemDescriptor;

            if (m_RequiresCameraImages && !descriptor.supportsCameraImage)
            {
                ARSceneSelectUI.DisableButton(m_Button);
                yield break;
            }

            if (m_RequiresBasicLightEstimation)
            {
                bool supportsBasics = descriptor.supportsCameraConfigurations && descriptor.supportsCameraImage;
                bool supportsColor = descriptor.supportsAverageColorTemperature || descriptor.supportsColorCorrection;
                bool supportsBrightness = descriptor.supportsAverageBrightness || descriptor.supportsAverageIntensityInLumens;

                if (!(supportsBasics && supportsColor && supportsBrightness))
                {
                    ARSceneSelectUI.DisableButton(m_Button);
                    yield break;
                }
            }

            if (m_RequiresHdrLightEstimation)
            {
                if (!(descriptor.supportsFaceTrackingHDRLightEstimation || descriptor.supportsWorldTrackingHDRLightEstimation))
                {
                    ARSceneSelectUI.DisableButton(m_Button);
                    yield break;
                }
            }

            if (m_RequiresCameraGrain && !descriptor.supportsCameraGrain)
                ARSceneSelectUI.DisableButton(m_Button);
        }
    }
}
