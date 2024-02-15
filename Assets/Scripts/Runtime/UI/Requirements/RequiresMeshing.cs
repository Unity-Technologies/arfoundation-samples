using System;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresMeshing : IBooleanExpression
    {
        static bool s_MeshingSupported;
        static bool s_MeshingChecked;

        public bool Evaluate()
        {
            if (!s_MeshingChecked)
            {
                s_MeshingChecked = true;
                var activeLoader = LoaderUtility.GetActiveLoader();
                if(activeLoader && activeLoader.GetLoadedSubsystem<XRMeshSubsystem>() != null)
                    s_MeshingSupported = true;
            }

            return s_MeshingSupported;
        }
    }
}
