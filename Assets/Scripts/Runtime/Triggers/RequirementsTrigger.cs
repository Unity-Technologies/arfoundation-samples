using System.Collections.Generic;
using UnityEngine.Events;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RequirementsTrigger : MonoBehaviour
    {
        [SerializeReference, SelectImplementation(typeof(IBooleanExpression))]
        List<IBooleanExpression> m_Requirements = new();
        public IList<IBooleanExpression> requirements => m_Requirements;

        [SerializeField, Tooltip("Invoked on Start if given requirements are met.")]
        UnityEvent requirementsMet;

        [SerializeField, Tooltip("Invoked on Start if given requirements are not met.")]
        UnityEvent requirementsNotMet;

        void Start()
        {
            foreach (var r in m_Requirements)
            {
                if (!r.Evaluate())
                {
                    requirementsNotMet?.Invoke();
                    return;
                }
            }

            requirementsMet?.Invoke();
        }
    }
}
