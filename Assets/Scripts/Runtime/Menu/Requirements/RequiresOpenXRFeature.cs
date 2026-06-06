using System;
using System.Collections.Generic;
#if OPENXR_1_13_OR_NEWER
using UnityEngine.XR.OpenXR;
#endif // OPENXR_1_13_OR_NEWER

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresOpenXRFeature : ISceneRequirement
    {
        [SerializeField, SelectOpenXRFeatureTypename, Tooltip("The assembly qualified type name of the OpenXR feature.")]
        string m_RequiredFeature;

        public virtual void Evaluate(List<RequirementResult> results)
        {
            var featureType = Type.GetType(m_RequiredFeature);
            var featureName = featureType?.Name ?? m_RequiredFeature;
            var requirementName = $"{GetType().Name} ({featureName})";
#if !OPENXR_1_13_OR_NEWER
            results.Add(new RequirementResult(false, requirementName));
#else
            var feature = OpenXRSettings.Instance.GetFeature(featureType);
            results.Add(new RequirementResult(feature != null && feature.enabled, requirementName));
#endif
        }
    }
}
