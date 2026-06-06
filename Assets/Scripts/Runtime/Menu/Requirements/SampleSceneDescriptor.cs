using System.Collections.Generic;
using Unity.XR.CoreUtils.Collections;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [CreateAssetMenu(menuName = "XR/AR Foundation/Sample Scene Descriptor")]
    public class SampleSceneDescriptor : ScriptableObject
    {
        [SerializeField]
        string m_DisplayName;

        [SerializeField, TextArea(2, 5)]
        string m_Description;

        [SerializeField]
        SceneCategory m_Category;

        [SerializeField]
        Sprite m_PreviewImage;

        [SerializeReference, SelectImplementation(typeof(ISceneRequirement))]
        List<ISceneRequirement> m_Requirements = new();

        public string displayName => m_DisplayName;
        public string description => m_Description;
        public SceneCategory category => m_Category;
        public Sprite previewImage => m_PreviewImage;
        public ReadOnlyListSpan<ISceneRequirement> requirements => new(m_Requirements);

        public bool EvaluateRequirements(List<RequirementResult> results)
        {
            results.Clear();
            foreach (var r in m_Requirements)
                r.Evaluate(results);

            foreach (var result in results)
            {
                if (!result.isSupported)
                    return false;
            }

            return true;
        }
    }
}
