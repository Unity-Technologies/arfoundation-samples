#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class DisplayCameraExposureInformation
#if UNITY_IOS
        : ARKitDisplayAdvancedCameraConfigurationInformation<CameraExposureController, ARKitExposureMode, ARKitExposure>
#else
        : ARKitDisplayAdvancedCameraConfigurationInformation<CameraExposureController>
#endif
    {
    }
}
