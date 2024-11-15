using System.Runtime.CompilerServices;
#if OPENXR_1_13_OR_NEWER
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
#endif // OPENXR_1_13_OR_NEWER

namespace UnityEngine.XR.ARFoundation.Samples
{
    static class OpenXRUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if OPENXR_1_13_OR_NEWER
        internal static bool IsOpenXRFeatureEnabled<T>() where T : OpenXRFeature
#else
        internal static bool IsOpenXRFeatureEnabled<T>()
#endif // OPENXR_1_13_OR_NEWER
        {
#if !OPENXR_1_13_OR_NEWER
            return false;
#else
            var feature = OpenXRSettings.Instance.GetFeature<T>() ;
            return feature != null && feature.enabled;
#endif // !OPENXR_1_13_OR_NEWER
        }
    }
}
