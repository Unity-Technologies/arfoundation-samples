using UnityEngine.SceneManagement;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Loads the correct Menu scene based on the active device platform.
    /// </summary>
    [DefaultExecutionOrder(-9999)]
    public class MenuLoader : MonoBehaviour
    {
        const string k_DefaultMenuScene = "Menu";
        const string k_MetaMenuScene = "MetaMenu";
        const string k_AndroidXRMenuScene = "AndroidXRMenu";

        void Awake()
        {
            LoadMenuScene();
        }

        public static string GetMenuSceneName()
        {
            var platform = DevicePlatformUtility.GetActiveDevicePlatform();
            return platform switch
            {
                DevicePlatform.MetaQuest => k_MetaMenuScene,
                DevicePlatform.AndroidXR => k_AndroidXRMenuScene,
                _ => k_DefaultMenuScene,
            };
        }

        public static bool IsHmdDevice()
        {
            var sceneName = GetMenuSceneName();
            return sceneName is k_MetaMenuScene or k_AndroidXRMenuScene;
        }

        public static void LoadMenuScene()
        {
            var sceneName = GetMenuSceneName();
            if (Application.CanStreamedLevelBeLoaded(sceneName))
                SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}
