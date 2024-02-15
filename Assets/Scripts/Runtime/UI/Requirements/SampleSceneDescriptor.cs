using System.Collections.Generic;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [CreateAssetMenu(menuName = "XR/AR Foundation/Sample Scene Descriptor")]
    public class SampleSceneDescriptor : ScriptableObject
    {
        [SerializeField]
        string m_SceneName;
        public string sceneName => m_SceneName;

        [SerializeReference, SelectImplementation(typeof(IBooleanExpression))]
        List<IBooleanExpression> m_Requirements;
        public IList<IBooleanExpression> requirements => m_Requirements;

        public bool EvaluateRequirements()
        {
            bool areRequirementsMet = true;
            foreach (var r in m_Requirements)
            {
                if (!r.Evaluate())
                {
                    areRequirementsMet = false;
                    break;
                }
            }

            return areRequirementsMet;
        }
    }
}
