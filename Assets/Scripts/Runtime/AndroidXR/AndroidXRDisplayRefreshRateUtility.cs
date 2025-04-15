using System;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;
#if ANDROIDOPENXR_0_0_1_OR_NEWER && UNITY_ANDROID
using UnityEngine.XR.OpenXR.Features.Android;
#endif // ANDROIDOPENXR_0_0_1_OR_NEWER && UNITY_ANDROID

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class AndroidXRDisplayRefreshRateUtility : IDisplayRefreshRateUtility
    {
        public string displayUtilitiesFeatureName
        {
            get
            {
#if ANDROIDOPENXR_0_0_1_OR_NEWER && UNITY_ANDROID
                return "Android XR: Display Utilities";
#else
                return null;
#endif // ANDROIDOPENXR_0_0_1_OR_NEWER && UNITY_ANDROID
            }
        }

        public bool IsDisplayUtilitiesFeatureEnabled()
        {
#if ANDROIDOPENXR_0_0_1_OR_NEWER && UNITY_ANDROID
            return OpenXRUtility.IsOpenXRFeatureEnabled<DisplayUtilitiesFeature>();
#else
            throw new NotSupportedException();
#endif // ANDROIDOPENXR_0_0_1_OR_NEWER && UNITY_ANDROID
        }

        public XRResultStatus TrySetDisplayRefreshRate(float refreshRate, XRDisplaySubsystem displaySubsystem)
        {
#if ANDROIDOPENXR_0_0_1_OR_NEWER && UNITY_ANDROID
            return displaySubsystem.TrySetDisplayRefreshRate(refreshRate);
#else
            throw new NotSupportedException();
#endif // ANDROIDOPENXR_0_0_1_OR_NEWER && UNITY_ANDROID
        }

        public XRResultStatus TryGetSupportedDisplayRefreshRates(Allocator allocator, out NativeArray<float> refreshRates, XRDisplaySubsystem displaySubsystem)
        {
#if ANDROIDOPENXR_0_0_1_OR_NEWER && UNITY_ANDROID
            return displaySubsystem.TryGetSupportedDisplayRefreshRates(allocator, out refreshRates);
#else
            throw new NotSupportedException();
#endif // ANDROIDOPENXR_0_0_1_OR_NEWER && UNITY_ANDROID
        }
    }
}
