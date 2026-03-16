using Unity.XR.CoreUtils.Bindings;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.Hands
{
    /// <summary>
    /// Animates a blend shape on a SkinnedMeshRenderer based on the interaction strength of a poke.
    /// </summary>
    public class PokeBlendShapeAnimator : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The PokeFilter to use to determine the interaction strength.")]
        XRPokeFilter m_PokeFilter;

        [SerializeField]
        [Tooltip("The SkinnedMeshRenderer to animate.")]
        SkinnedMeshRenderer m_SkinnedMeshRenderer;

        [SerializeField]
        [Tooltip("The index of the blend shape to animate.")]
        int m_BlendShapeIndex;

        [SerializeField]
        [Tooltip("The minimum blend shape value.")]
        float m_BlendShapeMin;

        [SerializeField]
        [Tooltip("The maximum blend shape value.")]
        float m_BlendShapeMax = 100f;

        readonly BindingsGroup m_BindingsGroup = new BindingsGroup();

        IXRHoverInteractable m_HoverInteractable;
        IXRInteractionStrengthInteractable m_InteractionStrengthInteractable;
#pragma warning disable CS0618 // Type or member is obsolete
        readonly FloatTweenableVariable m_TweenableVariable = new FloatTweenableVariable();
#pragma warning restore CS0618 // Type or member is obsolete

        float m_TweenTarget;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnEnable()
        {
            if (m_PokeFilter == null || m_SkinnedMeshRenderer == null)
            {
                enabled = false;
                return;
            }

            m_HoverInteractable = m_PokeFilter.GetComponent<IXRHoverInteractable>();
            m_InteractionStrengthInteractable = m_PokeFilter.GetComponent<IXRInteractionStrengthInteractable>();

            m_BindingsGroup.AddBinding(m_PokeFilter.pokeStateData.Subscribe(data =>
            {
                var blendShapeValue = Mathf.Lerp(m_BlendShapeMin, m_BlendShapeMax, data.interactionStrength);
                m_TweenTarget = blendShapeValue;
            }));

            m_BindingsGroup.AddBinding(m_TweenableVariable.SubscribeAndUpdate(newValue =>
            {
                m_SkinnedMeshRenderer.SetBlendShapeWeight(m_BlendShapeIndex, newValue);
            }));
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnDisable()
        {
            m_BindingsGroup.Clear();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Update()
        {
            m_TweenableVariable.HandleTween(Time.deltaTime * 16f);
            if (m_HoverInteractable.interactorsHovering.Count == 0)
                return;

            var pokeInteractorStrength = 0f;
            var largestNonPokeInteractorStrength = 0f;
            for (var index = 0; index < m_HoverInteractable.interactorsHovering.Count; ++index)
            {
                var interactor = m_HoverInteractable.interactorsHovering[index];
                var interactionStrength = m_InteractionStrengthInteractable.GetInteractionStrength(interactor);
                var isPokeProvider = interactor is IPokeStateDataProvider;
                if (isPokeProvider)
                {
                    pokeInteractorStrength = interactionStrength;
                }
                else
                {
                    largestNonPokeInteractorStrength = Mathf.Max(largestNonPokeInteractorStrength, interactionStrength);
                }
            }

            m_TweenableVariable.target = pokeInteractorStrength > largestNonPokeInteractorStrength ? m_TweenTarget : 0f;
        }
    }
}
