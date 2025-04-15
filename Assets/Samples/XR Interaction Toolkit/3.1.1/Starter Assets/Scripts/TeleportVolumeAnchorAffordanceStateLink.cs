using System;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// Helper component that binds an <see cref="XRInteractableAffordanceStateProvider"/> to a
    /// <see cref="TeleportationMultiAnchorVolume"/> when the teleport volume sets its destination anchor to a child transform
    /// of the state provider's originally bound interactable.
    /// </summary>
    [RequireComponent(typeof(XRInteractableAffordanceStateProvider))]
    [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
    public class TeleportVolumeAnchorAffordanceStateLink : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The teleport volume that will drive affordance states when its destination anchor belongs to this interactable.")]
        TeleportationMultiAnchorVolume m_ContainingTeleportVolume;

        /// <summary>
        /// The teleport volume that will drive affordance states when its destination anchor belongs to the
        /// state provider's originally bound interactable.
        /// </summary>
        public TeleportationMultiAnchorVolume containingTeleportVolume
        {
            get => m_ContainingTeleportVolume;
            set => m_ContainingTeleportVolume = value;
        }

        XRInteractableAffordanceStateProvider m_AffordanceStateProvider;
        IXRInteractable m_Interactable;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            m_AffordanceStateProvider = GetComponent<XRInteractableAffordanceStateProvider>();
            if (m_AffordanceStateProvider == null)
            {
                Debug.LogError($"Missing {nameof(XRInteractableAffordanceStateProvider)} on {gameObject.name}.", this);
                enabled = false;
                return;
            }

            if (m_ContainingTeleportVolume == null)
            {
                Debug.LogError($"Missing {nameof(TeleportationMultiAnchorVolume)} reference on {gameObject.name}.", this);
                enabled = false;
                return;
            }

            var interactableSource = m_AffordanceStateProvider.interactableSource;
            m_Interactable = interactableSource != null && interactableSource is IXRInteractable interactable
                    ? interactable
                    : m_AffordanceStateProvider.GetComponentInParent<IXRInteractable>();

            if (m_Interactable == null)
            {
                Debug.LogError($"Interactable source must be an {nameof(IXRInteractable)}.", this);
                enabled = false;
                return;
            }

            m_ContainingTeleportVolume.destinationAnchorChanged += OnDestinationAnchorChanged;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            if (m_ContainingTeleportVolume != null)
                m_ContainingTeleportVolume.destinationAnchorChanged -= OnDestinationAnchorChanged;

            if (m_AffordanceStateProvider != null)
                m_AffordanceStateProvider.SetBoundInteractionReceiver(m_Interactable);
        }

        void OnDestinationAnchorChanged(TeleportationMultiAnchorVolume anchorVolume)
        {
            var anchor = anchorVolume.destinationAnchor;
            if (anchor == null)
            {
                m_AffordanceStateProvider.SetBoundInteractionReceiver(m_Interactable);
                return;
            }

            // Use teleport volume to drive affordance states if its current anchor belongs to this interactable
            m_AffordanceStateProvider.SetBoundInteractionReceiver(
                anchor.IsChildOf(m_Interactable.transform)
                    ? m_ContainingTeleportVolume
                    : m_Interactable);
        }
    }
}
