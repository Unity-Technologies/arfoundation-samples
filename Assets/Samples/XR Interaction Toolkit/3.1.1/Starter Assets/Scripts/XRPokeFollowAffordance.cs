using System;
using Unity.Mathematics;
using Unity.XR.CoreUtils.Bindings;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// Follow animation affordance for <see cref="IPokeStateDataProvider"/>, such as <see cref="XRPokeFilter"/>.
    /// Used to animate a pressed transform, such as a button to follow the poke position.
    /// </summary>
    /// <remarks>
    /// The Affordance System namespace and all associated classes have been deprecated.
    /// The existing affordance system will be moved, replaced and updated with a new interaction
    /// feedback system in a future version of XRI, including this sample script.
    /// </remarks>
    [AddComponentMenu("XR/XR Poke Follow Affordance", 22)]
    public class XRPokeFollowAffordance : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Transform that will move in the poke direction when this or a parent GameObject is poked." +
                 "\nNote: Should be a direct child GameObject.")]
        Transform m_PokeFollowTransform;

        /// <summary>
        /// Transform that will animate along the axis of interaction when this interactable is poked.
        /// Note: Must be a direct child GameObject as it moves in local space relative to the poke target's transform.
        /// </summary>
        public Transform pokeFollowTransform
        {
            get => m_PokeFollowTransform;
            set => m_PokeFollowTransform = value;
        }

        [SerializeField]
        [Range(0f, 20f)]
        [Tooltip("Multiplies transform position interpolation as a factor of Time.deltaTime. If 0, no smoothing will be applied.")]
        float m_SmoothingSpeed = 16f;

        /// <summary>
        /// Multiplies transform position interpolation as a factor of <see cref="Time.deltaTime"/>. If <c>0</c>, no smoothing will be applied.
        /// </summary>
        public float smoothingSpeed
        {
            get => m_SmoothingSpeed;
            set => m_SmoothingSpeed = value;
        }

        [SerializeField]
        [Tooltip("When this component is no longer the target of the poke, the Poke Follow Transform returns to the original position.")]
        bool m_ReturnToInitialPosition = true;

        /// <summary>
        /// When this component is no longer the target of the poke, the <see cref="pokeFollowTransform"/> returns to the original position.
        /// </summary>
        public bool returnToInitialPosition
        {
            get => m_ReturnToInitialPosition;
            set => m_ReturnToInitialPosition = value;
        }

        [SerializeField]
        [Tooltip("Whether to apply the follow animation if the target of the poke is a child of this transform. " +
                 "This is useful for UI objects that may have child graphics.")]
        bool m_ApplyIfChildIsTarget = true;

        /// <summary>
        /// Whether to apply the follow animation if the target of the poke is a child of this transform.
        /// This is useful for UI objects that may have child graphics.
        /// </summary>
        public bool applyIfChildIsTarget
        {
            get => m_ApplyIfChildIsTarget;
            set => m_ApplyIfChildIsTarget = value;
        }

        [SerializeField]
        [Tooltip("Whether to keep the Poke Follow Transform from moving past a maximum distance from the poke target.")]
        bool m_ClampToMaxDistance;

        /// <summary>
        /// Whether to keep the <see cref="pokeFollowTransform"/> from moving past <see cref="maxDistance"/> from the poke target.
        /// </summary>
        public bool clampToMaxDistance
        {
            get => m_ClampToMaxDistance;
            set => m_ClampToMaxDistance = value;
        }

        [SerializeField]
        [Tooltip("The maximum distance from this transform that the Poke Follow Transform can move.")]
        float m_MaxDistance;

        /// <summary>
        /// The maximum distance from this transform that the <see cref="pokeFollowTransform"/> can move when
        /// <see cref="clampToMaxDistance"/> is <see langword="true"/>.
        /// </summary>
        public float maxDistance
        {
            get => m_MaxDistance;
            set => m_MaxDistance = value;
        }

        /// <summary>
        /// The original position of this interactable before any pushes have been applied.
        /// </summary>
        public Vector3 initialPosition
        {
            get => m_InitialPosition;
            set => m_InitialPosition = value;
        }

        IPokeStateDataProvider m_PokeDataProvider;
        IMultiPokeStateDataProvider m_MultiPokeStateDataProvider;

#pragma warning disable CS0618 // Type or member is obsolete
        readonly Vector3TweenableVariable m_TransformTweenableVariable = new Vector3TweenableVariable();
#pragma warning restore CS0618 // Type or member is obsolete
        readonly BindingsGroup m_BindingsGroup = new BindingsGroup();
        Vector3 m_InitialPosition;
        bool m_IsFirstFrame;

        [HideInInspector]
        [SerializeField]
        XRPokeFilter m_PokeFilter = null;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Awake()
        {
            m_MultiPokeStateDataProvider = GetComponentInParent<IMultiPokeStateDataProvider>();
            if (m_MultiPokeStateDataProvider == null)
                m_PokeDataProvider = GetComponentInParent<IPokeStateDataProvider>();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Start()
        {
            if (m_PokeFollowTransform != null)
            {
                m_InitialPosition = m_PokeFollowTransform.localPosition;
                m_BindingsGroup.AddBinding(m_TransformTweenableVariable.Subscribe(OnTransformTweenableVariableUpdated));

                if (m_MultiPokeStateDataProvider != null)
                    m_BindingsGroup.AddBinding(m_MultiPokeStateDataProvider.GetPokeStateDataForTarget(transform).Subscribe(OnPokeStateDataUpdated));
                else if (m_PokeDataProvider != null)
                    m_BindingsGroup.AddBinding(m_PokeDataProvider.pokeStateData.SubscribeAndUpdate(OnPokeStateDataUpdated));
            }
            else
            {
                enabled = false;
                Debug.LogWarning($"Missing Poke Follow Transform assignment on {this}. Disabling component.", this);
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDestroy()
        {
            m_BindingsGroup.Clear();
            m_TransformTweenableVariable?.Dispose();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void LateUpdate()
        {
            if (m_IsFirstFrame)
            {
                m_TransformTweenableVariable.HandleTween(1f);
                m_IsFirstFrame = false;
                return;
            }

            m_TransformTweenableVariable.HandleTween(m_SmoothingSpeed > 0f ? Time.deltaTime * m_SmoothingSpeed : 1f);
        }

        protected virtual void OnTransformTweenableVariableUpdated(float3 position)
        {
            // UI Anchors can cause this to not work correctly, so we check if it's a RectTransform and set the localPosition Z only
            if (m_PokeFollowTransform is RectTransform)
            {
                var targetPosition = m_PokeFollowTransform.localPosition;
                targetPosition.z = position.z;
                m_PokeFollowTransform.localPosition = targetPosition;
            }
            else
            {
                m_PokeFollowTransform.localPosition = position;
            }
        }

        void OnPokeStateDataUpdated(PokeStateData data)
        {
            var pokeTarget = data.target;
            var applyFollow = m_ApplyIfChildIsTarget
                ? pokeTarget != null && pokeTarget.IsChildOf(transform)
                : pokeTarget == transform;

            if (applyFollow)
            {
                var targetPosition = pokeTarget.InverseTransformPoint(data.axisAlignedPokeInteractionPoint);
                if (m_ClampToMaxDistance && targetPosition.sqrMagnitude > m_MaxDistance * m_MaxDistance)
                    targetPosition = Vector3.ClampMagnitude(targetPosition, m_MaxDistance);

                m_TransformTweenableVariable.target = targetPosition;
            }
            else if (m_ReturnToInitialPosition)
            {
                m_TransformTweenableVariable.target = m_InitialPosition;
            }
        }

        public void ResetFollowTransform()
        {
            if (!m_ClampToMaxDistance || m_PokeFollowTransform == null)
                return;

            m_PokeFollowTransform.localPosition = m_InitialPosition;
        }

        void OnDrawGizmos()
        {
            if (!TryGetTargetEndPoint(out var endPoint))
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, endPoint);
        }

        bool TryGetTargetEndPoint(out Vector3 endPoint)
        {
            if (!m_ClampToMaxDistance || m_PokeFilter == null)
            {
                endPoint = Vector3.zero;
                return false;
            }

            Vector3 origin = transform.position;
            Vector3 direction = ComputeRotatedDepthEvaluationAxis(m_PokeFilter.pokeConfiguration);
            endPoint = origin + direction.normalized * m_MaxDistance;
            return true;
        }

        Vector3 ComputeRotatedDepthEvaluationAxis(PokeThresholdData pokeThresholdData)
        {
            if (pokeThresholdData == null)
                return Vector3.zero;

            Vector3 rotatedDepthEvaluationAxis = Vector3.zero;
            switch (pokeThresholdData.pokeDirection)
            {
                case PokeAxis.X:
                case PokeAxis.NegativeX:
                    rotatedDepthEvaluationAxis = transform.right;
                    break;
                case PokeAxis.Y:
                case PokeAxis.NegativeY:
                    rotatedDepthEvaluationAxis = transform.up;
                    break;
                case PokeAxis.Z:
                case PokeAxis.NegativeZ:
                    rotatedDepthEvaluationAxis = transform.forward;
                    break;
            }

            switch (pokeThresholdData.pokeDirection)
            {
                case PokeAxis.X:
                case PokeAxis.Y:
                case PokeAxis.Z:
                    rotatedDepthEvaluationAxis = -rotatedDepthEvaluationAxis;
                    break;
            }

            return rotatedDepthEvaluationAxis;
        }

        void OnValidate()
        {
            if (m_PokeFilter == null)
            {
                m_PokeFilter = GetComponentInParent<XRPokeFilter>();
            }

            // Visually update the end point to match the target clamped position
            if (m_PokeFollowTransform != null && TryGetTargetEndPoint(out var endPoint))
                m_PokeFollowTransform.position = endPoint;
        }
    }
}
