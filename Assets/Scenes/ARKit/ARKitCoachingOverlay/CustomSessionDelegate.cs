#if UNITY_IOS
using UnityEngine.XR.ARKit;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class CustomSessionDelegate : DefaultARKitSessionDelegate
    {
        protected override void OnCoachingOverlayViewWillActivate(ARKitSessionSubsystem sessionSubsystem)
        {
            Logger.Log(nameof(OnCoachingOverlayViewWillActivate));
        }

        protected override void OnCoachingOverlayViewDidDeactivate(ARKitSessionSubsystem sessionSubsystem)
        {
            Logger.Log(nameof(OnCoachingOverlayViewDidDeactivate));
        }
    }
}
#endif
