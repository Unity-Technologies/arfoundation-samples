using System;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;
// The UNITY_EDITOR_WIN is to ensure that the Meta display refresh rate utility works on Windows standalone for Meta Quest Link
#if METAOPENXR_0_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR_WIN)
using UnityEngine.XR.OpenXR.Features.Meta;
#endif // METAOPENXR_0_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR_WIN)

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MetaDisplayRefreshRateUtility : IDisplayRefreshRateUtility
    {
        public string displayUtilitiesFeatureName
        {
            get
            {
#if METAOPENXR_0_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR_WIN)
                return DisplayUtilitiesFeature.displayName;
#else
                return null;
#endif // METAOPENXR_0_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR_WIN)
            }
        }

        public bool IsDisplayUtilitiesFeatureEnabled()
        {
#if METAOPENXR_0_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR_WIN)
            return OpenXRUtility.IsOpenXRFeatureEnabled<DisplayUtilitiesFeature>();
#else
            throw new NotSupportedException();
#endif // METAOPENXR_0_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR_WIN)
        }

        public XRResultStatus TrySetDisplayRefreshRate(float refreshRate, XRDisplaySubsystem displaySubsystem)
        {
#if METAOPENXR_0_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR_WIN)
            if (displaySubsystem.TryRequestDisplayRefreshRate(refreshRate))
            {
                return new XRResultStatus(XRResultStatus.StatusCode.UnqualifiedSuccess);
            } else {
                return new XRResultStatus(XRResultStatus.StatusCode.UnknownError);
            }
#endif // METAOPENXR_0_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR_WIN)
            throw new NotSupportedException();
        }

        public XRResultStatus TryGetSupportedDisplayRefreshRates(Allocator allocator, out NativeArray<float> refreshRates, XRDisplaySubsystem displaySubsystem)
        {
#if METAOPENXR_0_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR_WIN)
            if (displaySubsystem.TryGetSupportedDisplayRefreshRates(allocator, out refreshRates))
            {
                return new XRResultStatus(XRResultStatus.StatusCode.UnqualifiedSuccess);
            } else {
                return new XRResultStatus(XRResultStatus.StatusCode.UnknownError);
            }
#endif // METAOPENXR_0_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR_WIN)
            throw new NotSupportedException();
        }
    }
}
