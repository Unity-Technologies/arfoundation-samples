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
            if (string.IsNullOrEmpty(m_RequiredFeature))
            {
                results.Add(new RequirementResult(false, "OpenXR Feature (Unknown) No OpenXR feature type specified."));
                return;
            }

            var featureType = Type.GetType(m_RequiredFeature);
            var featureName = featureType?.Name ?? ParseTypeName(m_RequiredFeature);
            var requirementName = $"OpenXR Feature ({featureName})";
            var remediationText = $"Enable {featureName} in <b>Project Settings</b> > <b>XR Plug-in Management</b> > <b>OpenXR</b>.";
#if !OPENXR_1_13_OR_NEWER
            results.Add(new RequirementResult(false, requirementName, remediationText));
#else
            if (!XRManagerUtility.IsLoaderActive<OpenXRLoader>())
            {
                results.Add(new RequirementResult(false, requirementName, remediationText));
                return;
            }

            if (featureType == null)
            {
                results.Add(new RequirementResult(false, requirementName, remediationText));
                return;
            }

            var feature = OpenXRSettings.Instance.GetFeature(featureType);
            results.Add(new RequirementResult(feature != null && feature.enabled, requirementName, remediationText));
#endif
        }

        static string ParseTypeName(string assemblyQualifiedName)
        {
            var fullName = assemblyQualifiedName;
            var commaIndex = fullName.IndexOf(',');
            if (commaIndex >= 0)
                fullName = fullName.Substring(0, commaIndex);

            var dotIndex = fullName.LastIndexOf('.');
            return dotIndex >= 0
                ? fullName.Substring(dotIndex + 1)
                : fullName;
        }
    }
}
