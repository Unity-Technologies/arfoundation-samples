using System.Runtime.CompilerServices;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;

namespace UnityEngine.XR.ARFoundation.Samples
{
    static class OpenXRUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsOpenXRFeatureEnabled<T>() where T : OpenXRFeature
        {
            var feature = OpenXRSettings.Instance.GetFeature<T>() ;
            return feature != null && feature.enabled;
        }
    }
}
