using System;
using System.Collections.Generic;
#if OPENXR_1_13_OR_NEWER
using UnityEngine.XR.OpenXR;
#endif // OPENXR_1_13_OR_NEWER

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresOpenXRExtension : ISceneRequirement
    {
        [SerializeField, Tooltip("The OpenXR extension that must be enabled.")]
        string m_RequiredExtension;

        public virtual void Evaluate(List<RequirementResult> results)
        {
#if !OPENXR_1_13_OR_NEWER
            results.Add(new RequirementResult(false, $"{GetType().Name} ({m_RequiredExtension})"));
#else
            var name = $"{GetType().Name} ({m_RequiredExtension})";

            if (!XRManagerUtility.IsLoaderActive<OpenXRLoader>())
            {
                results.Add(new RequirementResult(false, name));
                return;
            }

            var isExtensionEnabled = OpenXRRuntime.IsExtensionEnabled(m_RequiredExtension);
            results.Add(new RequirementResult(isExtensionEnabled, name));
#endif
        }
    }
}
