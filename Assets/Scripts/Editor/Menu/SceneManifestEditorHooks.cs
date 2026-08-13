using Unity.Scripting.LifecycleManagement;
using UnityEditor.Scripting.LifecycleManagement;
using UnityEngine.XR.ARFoundation.Samples;

namespace UnityEditor.XR.ARFoundation.Samples
{
    static partial class SceneManifestEditorHooks
    {
        [OnCodeLoaded]
        static void SetupBridge()
        {
            BuildProfileSelector.refreshManifest = SceneManifestGenerator.Refresh;
        }

        [OnExitingEditMode]
        static void RefreshManifestBeforePlay()
        {
            SceneManifestGenerator.RefreshFromActiveBuildProfile();
        }
    }
}
