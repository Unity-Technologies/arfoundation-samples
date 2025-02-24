#if METAOPENXR_2_1_OR_NEWER && (UNITY_EDITOR || UNITY_ANDROID)
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Meta;
#endif // METAOPENXR_2_1_OR_NEWER && (UNITY_EDITOR || UNITY_ANDROID)

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RequiresBoundaryVisibilityMeta : IBooleanExpression
    {
        /// <inheritdoc />
        public bool Evaluate()
        {
#if !(METAOPENXR_2_1_OR_NEWER && (UNITY_EDITOR || UNITY_ANDROID))
            return false;
#else
            OpenXRFeature feature = null;
            feature = OpenXRSettings.Instance.GetFeature<BoundaryVisibilityFeature>();
            return feature != null && feature.enabled;
#endif // !(METAOPENXR_2_1_OR_NEWER && (UNITY_EDITOR || UNITY_ANDROID))
        }
    }
}
