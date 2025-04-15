using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    interface IDisplayRefreshRateUtility
    {
        string displayUtilitiesFeatureName { get; }
        bool IsDisplayUtilitiesFeatureEnabled();
        XRResultStatus TrySetDisplayRefreshRate(float refreshRate, XRDisplaySubsystem displaySubsystem);
        XRResultStatus TryGetSupportedDisplayRefreshRates(Allocator allocator, out NativeArray<float> refreshRates, XRDisplaySubsystem displaySubsystem);
    }
}
