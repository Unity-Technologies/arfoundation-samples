using System.Collections.Generic;
using Unity.XR.CoreUtils.Collections;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RuntimeSceneManifest : ScriptableObject
    {
        [SerializeField]
        List<SampleSceneDescriptor> m_SceneDescriptors = new();

        public ReadOnlyListSpan<SampleSceneDescriptor> sceneDescriptors => new(m_SceneDescriptors);

        public const string k_ResourcesPath = "RuntimeSceneManifest";

#if UNITY_EDITOR
        public void SetScenes(List<SampleSceneDescriptor> descriptors)
        {
            m_SceneDescriptors = descriptors;
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
        }
#endif
    }
}
