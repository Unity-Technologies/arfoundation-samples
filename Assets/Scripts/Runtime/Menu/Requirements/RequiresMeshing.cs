using System;
using System.Collections.Generic;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresMeshing : ISceneRequirement
    {
        static bool s_MeshingSupported;
        static bool s_MeshingChecked;

        public virtual void Evaluate(List<RequirementResult> results)
        {
            if (!s_MeshingChecked)
            {
                s_MeshingChecked = true;
                var activeLoader = LoaderUtility.GetActiveLoader();
                if (activeLoader && activeLoader.GetLoadedSubsystem<XRMeshSubsystem>() != null)
                    s_MeshingSupported = true;
            }

            results.Add(new RequirementResult(s_MeshingSupported, GetType().Name));
        }
    }
}
