using System;
using Unity.Mathematics;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.Hands
{
    /// <summary>
    /// Affordance receiver applying a Vector3 (Float3) affordance theme to a Transform local scale.
    /// Broadcasts new affordance value with Unity Event.
    /// </summary>
    [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
    public class Vector3ScaleAffordanceReceiver : Vector3AffordanceReceiver
    {
        [SerializeField]
        [Tooltip("The transform to apply the scale value to.")]
        Transform m_TargetTransform;

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            if (m_TargetTransform == null)
                m_TargetTransform = transform;
        }

        /// <inheritdoc />
        protected override void OnAffordanceValueUpdated(float3 newValue)
        {
            base.OnAffordanceValueUpdated(newValue);
            m_TargetTransform.localScale = newValue;
        }
    }
}
