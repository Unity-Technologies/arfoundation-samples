using System;
using Unity.Mathematics;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.Hands
{
    /// <summary>
    /// Affordance receiver applying a Vector3 (Float3) affordance theme to a Transform local position.
    /// Broadcasts new affordance value with Unity Event.
    /// </summary>
    [AddComponentMenu("Affordance System/Receiver/Transformation/Local Position Offset Affordance Receiver", 12)]
    [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
    public class LocalPositionOffsetAffordanceReceiver : Vector3AffordanceReceiver
    {
        [SerializeField]
        [Tooltip("Transform on which to apply a local translation value.")]
        Transform m_TransformToTranslate;

        /// <summary>
        /// Transform on which to apply a local translation value.
        /// </summary>
        public Transform transformToTranslate
        {
            get => m_TransformToTranslate;
            set
            {
                m_TransformToTranslate = value;
                m_HasTransformToTranslate = m_TransformToTranslate != null;
            }
        }

        bool m_HasTransformToTranslate;
        float3 m_InitialOffset = float3.zero;

        /// <inheritdoc/>
        protected override void OnEnable()
        {
            base.OnEnable();
            m_HasTransformToTranslate = m_TransformToTranslate != null;
        }

        /// <inheritdoc/>
        protected override float3 GetCurrentValueForCapture()
        {
            if (m_HasTransformToTranslate)
            {
                m_InitialOffset = m_TransformToTranslate.localPosition;
            }

            return float3.zero;
        }

        /// <inheritdoc/>
        protected override void OnAffordanceValueUpdated(float3 newValue)
        {
            if (m_HasTransformToTranslate)
            {
                m_TransformToTranslate.localPosition = m_InitialOffset + newValue;
            }

            base.OnAffordanceValueUpdated(newValue);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnValidate()
        {
            if (m_TransformToTranslate == null)
                m_TransformToTranslate = transform;
        }
    }
}
