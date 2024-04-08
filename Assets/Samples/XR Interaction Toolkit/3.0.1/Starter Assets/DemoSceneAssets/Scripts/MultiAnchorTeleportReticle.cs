using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactables.Visuals;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// A custom reticle for a <see cref="TeleportationMultiAnchorVolume"/> that displays its progress towards evaluating
    /// a destination anchor and an indicator pointing in the direction of the destination anchor.
    /// </summary>
    public class MultiAnchorTeleportReticle : MonoBehaviour, IXRInteractableCustomReticle
    {
        [SerializeField]
        [Tooltip("Filled image that displays the progress towards evaluating a destination anchor.")]
        Image m_TimerProgressFilledImage;

        /// <summary>
        /// <see cref="Image.Type.Filled"/> image that displays the progress towards evaluating a destination anchor.
        /// </summary>
        public Image timerProgressFilledImage
        {
            get => m_TimerProgressFilledImage;
            set => m_TimerProgressFilledImage = value;
        }

        [SerializeField]
        [Tooltip("Object that is rotated about its Z axis to point at the destination anchor.")]
        GameObject m_DestinationIndicator;

        /// <summary>
        /// Object that is rotated about its Z axis to point at the destination anchor.
        /// </summary>
        public GameObject destinationIndicator
        {
            get => m_DestinationIndicator;
            set => m_DestinationIndicator = value;
        }

        [SerializeField]
        [Tooltip("Object that is rotated about its Z axis to point at the potential destination while still evaluating.")]
        GameObject m_PotentialDestinationIndicator;

        /// <summary>
        /// Object that is rotated about its Z axis to point at the potential destination while still evaluating.
        /// </summary>
        public GameObject potentialDestinationIndicator
        {
            get => m_PotentialDestinationIndicator;
            set => m_PotentialDestinationIndicator = value;
        }

        [SerializeField]
        [Tooltip("The amount of time, in seconds, between updates to the indicator pointing at the potential destination.")]
        float m_PotentialIndicatorUpdateFrequency = 0.1f;

        /// <summary>
        /// The amount of time, in seconds, between updates to the indicator pointing at the potential destination.
        /// </summary>
        public float potentialIndicatorUpdateFrequency
        {
            get => m_PotentialIndicatorUpdateFrequency;
            set => m_PotentialIndicatorUpdateFrequency = value;
        }

        TeleportationMultiAnchorVolume m_AnchorVolume;
        float m_LastPotentialIndicatorUpdateTime;

        /// <inheritdoc/>
        public void OnReticleAttached(XRBaseInteractable interactable, IXRCustomReticleProvider reticleProvider)
        {
            m_AnchorVolume = interactable as TeleportationMultiAnchorVolume;
            m_PotentialDestinationIndicator.SetActive(false);
            m_DestinationIndicator.SetActive(false);
            m_TimerProgressFilledImage.type = Image.Type.Filled;
            m_TimerProgressFilledImage.fillAmount = 0f;
            if (m_AnchorVolume == null)
                return;

            m_AnchorVolume.destinationAnchorChanged += OnDestinationAnchorChanged;
        }

        /// <inheritdoc/>
        public void OnReticleDetaching()
        {
            if (m_AnchorVolume == null)
                return;

            m_AnchorVolume.destinationAnchorChanged -= OnDestinationAnchorChanged;
            m_AnchorVolume = null;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Update()
        {
            if (m_AnchorVolume == null)
                return;

            var destinationAnchor = m_AnchorVolume.destinationAnchor;
            if (destinationAnchor != null)
            {
                PointAtTarget(m_DestinationIndicator.transform, destinationAnchor.position);
                return;
            }

            m_TimerProgressFilledImage.fillAmount = m_AnchorVolume.destinationEvaluationProgress;
            if (Time.time - m_LastPotentialIndicatorUpdateTime >= m_PotentialIndicatorUpdateFrequency)
                UpdatePotentialDestinationIndicator();
        }

        void UpdatePotentialDestinationIndicator()
        {
            m_LastPotentialIndicatorUpdateTime = Time.time;
            if (!m_AnchorVolume.destinationEvaluationSettings.Value.pollForDestinationChange)
            {
                m_PotentialDestinationIndicator.SetActive(false);
                return;
            }

            var potentialDestinationIndex = m_AnchorVolume.destinationEvaluationFilter.GetDestinationAnchorIndex(m_AnchorVolume);
            var anchors = m_AnchorVolume.anchorTransforms;
            if (potentialDestinationIndex < 0 || potentialDestinationIndex >= anchors.Count)
            {
                m_PotentialDestinationIndicator.SetActive(false);
                return;
            }

            var potentialDestination = anchors[potentialDestinationIndex];
            if (potentialDestination == null)
            {
                m_PotentialDestinationIndicator.SetActive(false);
                return;
            }

            m_PotentialDestinationIndicator.SetActive(true);
            PointAtTarget(m_PotentialDestinationIndicator.transform, potentialDestination.position);
        }

        void OnDestinationAnchorChanged(TeleportationMultiAnchorVolume anchorVolume)
        {
            var destinationAnchor = anchorVolume.destinationAnchor;
            if (destinationAnchor != null)
            {
                m_TimerProgressFilledImage.fillAmount = 1f;
                m_PotentialDestinationIndicator.SetActive(false);
                m_DestinationIndicator.SetActive(true);
                PointAtTarget(m_DestinationIndicator.transform, destinationAnchor.position);
            }
            else
            {
                m_TimerProgressFilledImage.fillAmount = 0f;
                m_DestinationIndicator.SetActive(false);
            }
        }

        static void PointAtTarget(Transform indicatorTransform, Vector3 targetPosition)
        {
            indicatorTransform.rotation = Quaternion.LookRotation(indicatorTransform.forward, targetPosition - indicatorTransform.position);
        }
    }
}