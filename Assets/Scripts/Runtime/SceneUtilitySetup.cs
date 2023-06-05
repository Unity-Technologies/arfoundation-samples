using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples;

public class SceneUtilitySetup
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Setup()
    {
        var gameObject = new GameObject("SceneUtility");
        gameObject.AddComponent<SceneUtility>();
    }
}
