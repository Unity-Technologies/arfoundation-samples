#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class DisplayCameraWhiteBalanceInformation
#if UNITY_IOS
        : ARKitDisplayAdvancedCameraConfigurationInformation<CameraWhiteBalanceController, ARKitWhiteBalanceMode, ARKitWhiteBalanceGains>
#else
        : ARKitDisplayAdvancedCameraConfigurationInformation<CameraWhiteBalanceController>
#endif
    {
    }
}
