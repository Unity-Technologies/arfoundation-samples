using System;
using UnityEditor.Build;
using UnityEditor.Build.Profile;
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
                var profile = AssetDatabase.LoadAssetAtPath<BuildProfile>(report.summary.buildProfilePath);
                SceneManifestGenerator.Refresh(profile);
            }
            catch (InvalidOperationException ex)
            {
                throw new BuildFailedException(ex);
            }
        }
    }
}
