using System;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace UnityEditor.XR.ARFoundation.Samples
{
    class SceneManifestBuildProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            try
            {
                SceneManifestGenerator.RefreshFromActiveBuildProfile();
            }
            catch (InvalidOperationException ex)
            {
                throw new BuildFailedException(ex);
            }
        }
    }
}
