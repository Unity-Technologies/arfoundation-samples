using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// Affordance component used in conjunction with a <see cref="ClimbTeleportInteractor"/> to display an object
    /// pointing at the target teleport destination while climbing.
    /// </summary>
    public class ClimbTeleportDestinationIndicator : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The interactor that drives the display and placement of the pointer object.")]
        ClimbTeleportInteractor m_ClimbTeleportInteractor;

        /// <summary>
        /// The interactor that drives the display and placement of the pointer object.
        /// </summary>
        public ClimbTeleportInteractor climbTeleportInteractor
        {
            get => m_ClimbTeleportInteractor;
            set => m_ClimbTeleportInteractor = value;
        }

        [SerializeField]
        [Tooltip("The prefab to spawn when a teleport destination is chosen. The instance will spawn next to the " +
            "destination and point its forward vector at the destination and its up vector at the camera.")]
        GameObject m_PointerPrefab;

        /// <summary>
        /// The prefab to spawn when a teleport destination is chosen. The instance will spawn next to the destination
        /// and point its forward vector at the destination and its up vector at the camera.
        /// </summary>
        public GameObject pointerPrefab
        {
            get => m_PointerPrefab;
            set => m_PointerPrefab = value;
        }

        [SerializeField]
        [Tooltip("The distance from the destination at which the pointer object spawns.")]
        float m_PointerDistance = 0.3f;

        /// <summary>
        /// The distance from the destination at which the pointer object spawns.
        /// </summary>
        public float pointerDistance
        {
            get => m_PointerDistance;
            set => m_PointerDistance = value;
        }

        TeleportationMultiAnchorVolume m_ActiveTeleportVolume;
        Transform m_PointerInstance;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            if (m_ClimbTeleportInteractor == null)
            {
                if (!ComponentLocatorUtility<ClimbTeleportInteractor>.TryFindComponent(out m_ClimbTeleportInteractor))
                {
                    Debug.LogError($"Could not find {nameof(ClimbTeleportInteractor)} in scene.");
                    enabled = false;
                    return;
                }
            }

            m_ClimbTeleportInteractor.hoverEntered.AddListener(OnInteractorHoverEntered);
            m_ClimbTeleportInteractor.hoverExited.AddListener(OnInteractorHoverExited);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            HideIndicator();

            if (m_ActiveTeleportVolume != null)
            {
                m_ActiveTeleportVolume.destinationAnchorChanged -= OnClimbTeleportDestinationAnchorChanged;
                m_ActiveTeleportVolume = null;
            }

            if (m_ClimbTeleportInteractor != null)
            {
                m_ClimbTeleportInteractor.hoverEntered.RemoveListener(OnInteractorHoverEntered);
                m_ClimbTeleportInteractor.hoverExited.RemoveListener(OnInteractorHoverExited);
            }
        }

        void OnInteractorHoverEntered(HoverEnterEventArgs args)
        {
            if (m_ActiveTeleportVolume != null || !(args.interactableObject is TeleportationMultiAnchorVolume teleportVolume))
                return;

            m_ActiveTeleportVolume = teleportVolume;
            if (m_ActiveTeleportVolume.destinationAnchor != null)
                OnClimbTeleportDestinationAnchorChanged(m_ActiveTeleportVolume);

            m_ActiveTeleportVolume.destinationAnchorChanged += OnClimbTeleportDestinationAnchorChanged;
        }

        void OnInteractorHoverExited(HoverExitEventArgs args)
        {
            if (!(args.interactableObject is TeleportationMultiAnchorVolume teleportVolume) || teleportVolume != m_ActiveTeleportVolume)
                return;

            HideIndicator();
            m_ActiveTeleportVolume.destinationAnchorChanged -= OnClimbTeleportDestinationAnchorChanged;
            m_ActiveTeleportVolume = null;
        }

        void OnClimbTeleportDestinationAnchorChanged(TeleportationMultiAnchorVolume teleportVolume)
        {
            HideIndicator();

            var destinationAnchor = teleportVolume.destinationAnchor;
            if (destinationAnchor == null)
                return;

            m_PointerInstance = Instantiate(m_PointerPrefab).transform;
            var cameraTrans = teleportVolume.teleportationProvider.mediator.xrOrigin.Camera.transform;
            var cameraPosition = cameraTrans.position;
            var destinationPosition = destinationAnchor.position;
            var destinationDirectionInScreenSpace = cameraTrans.InverseTransformDirection(destinationPosition - cameraPosition);
            destinationDirectionInScreenSpace.z = 0f;
            var pointerDirection = cameraTrans.TransformDirection(destinationDirectionInScreenSpace).normalized;
            m_PointerInstance.position = destinationPosition - pointerDirection * m_PointerDistance;
            m_PointerInstance.rotation = Quaternion.LookRotation(pointerDirection, -cameraTrans.forward);
        }

        void HideIndicator()
        {
            if (m_PointerInstance != null)
                Destroy(m_PointerInstance.gameObject);
        }
    }
}