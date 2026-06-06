namespace UnityEngine.XR.ARFoundation.Samples
{
    public static class FramerateBootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitializeFramerate()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 120;
        }
    }
}
