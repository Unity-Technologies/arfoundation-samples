using System.Collections.Generic;
using UnityEngine.Events;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RequirementsTrigger : MonoBehaviour
    {
        [SerializeReference, SelectImplementation(typeof(ISceneRequirement))]
        List<ISceneRequirement> m_Requirements = new();

        [SerializeField, Tooltip("Invoked on Start if given requirements are met.")]
        UnityEvent requirementsMet;

        [SerializeField, Tooltip("Invoked on Start if given requirements are not met.")]
        UnityEvent requirementsNotMet;

        readonly List<RequirementResult> m_RequirementResults = new();

        public IList<ISceneRequirement> requirements => m_Requirements;

        void Start()
        {
            m_RequirementResults.Clear();
            foreach (var r in m_Requirements)
                r.Evaluate(m_RequirementResults);

            foreach (var result in m_RequirementResults)
            {
                if (!result.isSupported)
                {
                    requirementsNotMet?.Invoke();
                    return;
                }
            }

            requirementsMet?.Invoke();
        }
    }
}
