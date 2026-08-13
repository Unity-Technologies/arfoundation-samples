using System;
using System.Collections.Generic;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresVisualScripting : ISceneRequirement
    {
        public virtual void Evaluate(List<RequirementResult> results)
        {
            const string remediationText = "Install the Visual Scripting package (1.8+) from the Package Manager.";
#if VISUALSCRIPTING_1_8_OR_NEWER
            results.Add(new RequirementResult(true, "Visual Scripting", remediationText));
            return;
#endif
#pragma warning disable CS0162
            results.Add(new RequirementResult(false, "Visual Scripting", remediationText));
#pragma warning restore CS0162
        }
    }
}
