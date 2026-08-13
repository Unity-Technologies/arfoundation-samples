using System.Collections.Generic;
using Unity.XR.CoreUtils.Collections;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RuntimeSceneManifest : ScriptableObject
    {
        public const string runtimeSceneManifestPath = "RuntimeSceneManifest";

        [SerializeField]
        List<SampleSceneDescriptor> m_SceneDescriptors = new();

        public ReadOnlyListSpan<SampleSceneDescriptor> sceneDescriptors => new(m_SceneDescriptors);


#if UNITY_EDITOR
        public void SetScenes(List<SampleSceneDescriptor> descriptors)
        {
            m_SceneDescriptors = descriptors;

            if (!UnityEditor.EditorApplication.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
            }
        }
#endif
    }
}
