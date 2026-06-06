using System;
using System.Collections.Generic;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresVisualScripting : ISceneRequirement
    {
        public virtual void Evaluate(List<RequirementResult> results)
        {
#if VISUALSCRIPTING_1_8_OR_NEWER
            results.Add(new RequirementResult(true, GetType().Name));
            return;
#endif
#pragma warning disable CS0162
            results.Add(new RequirementResult(false, GetType().Name));
#pragma warning restore CS0162
        }
    }
}
