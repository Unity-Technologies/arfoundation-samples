#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class DisplayCameraFocusInformation
#if UNITY_IOS
        : ARKitDisplayAdvancedCameraConfigurationInformation<CameraFocusController, ARKitFocusMode, ARKitFocus>
#else
        : ARKitDisplayAdvancedCameraConfigurationInformation<CameraFocusController>
#endif
    {
    }
}
