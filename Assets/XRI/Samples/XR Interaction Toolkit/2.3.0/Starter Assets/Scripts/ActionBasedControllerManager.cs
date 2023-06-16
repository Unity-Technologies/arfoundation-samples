using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// Use this class to mediate the controllers and their associated interactors and input actions under different interaction states.
    /// </summary>
    [AddComponentMenu("XR/Action Based Controller Manager")]
    [DefaultExecutionOrder(k_UpdateOrder)]
    public class ActionBasedControllerManager : MonoBehaviour
    {
        /// <summary>
        /// Order when instances of type <see cref="ActionBasedControllerManager"/> are updated.
        /// </summary>
        /// <remarks>
        /// Executes before controller components to ensure input processors can be attached
        /// to input actions and/or bindings before the controller component reads the current
        /// values of the input actions.
        /// </remarks>
        public const int k_UpdateOrder = XRInteractionUpdateOrder.k_Controllers - 1;

        [Space]
        [Header("Interactors")]

        [SerializeField]
        [Tooltip("The GameObject containing the interaction group used for direct and distant manipulation.")]
        XRInteractionGroup m_ManipulationInteractionGroup;

        [SerializeField]
        [Tooltip("The GameObject containing the interactor used for direct manipulation.")]
        XRDirectInteractor m_DirectInteractor;

        [SerializeField]
        [Tooltip("The GameObject containing the interactor used for distant/ray manipulation.")]
        XRRayInteractor m_RayInteractor;

        [SerializeField]
        [Tooltip("The GameObject containing the interactor used for teleportation.")]
        XRRayInteractor m_TeleportInteractor;

        [Space]
        [Header("Controller Actions")]

        [SerializeField]
        [Tooltip("The reference to the action to start the teleport aiming mode for this controller.")]
        InputActionReference m_TeleportModeActivate;

        [SerializeField]
        [Tooltip("The reference to the action to cancel the teleport aiming mode for this controller.")]
        InputActionReference m_TeleportModeCancel;

        [SerializeField]
        [Tooltip("The reference to the action of continuous turning the XR Origin with this controller.")]
        InputActionReference m_Turn;

        [SerializeField]
        [Tooltip("The reference to the action of snap turning the XR Origin with this controller.")]
        InputActionReference m_SnapTurn;

        [SerializeField]
        [Tooltip("The reference to the action of moving the XR Origin with this controller.")]
        InputActionReference m_Move;

        [Space]
        [Header("Locomotion Settings")]

        [SerializeField]
        [Tooltip("If true, continuous movement will be enabled. If false, teleport will enabled.")]
        bool m_SmoothMotionEnabled;
        
        [SerializeField]
        [Tooltip("If true, continuous turn will be enabled. If false, snap turn will be enabled. Note: If smooth motion is enabled and enable strafe is enabled on the continuous move provider, turn will be overriden in favor of strafe.")]
        bool m_SmoothTurnEnabled;

        public bool smoothMotionEnabled
        {
            get => m_SmoothMotionEnabled;
            set
            {
                m_SmoothMotionEnabled = value;
                UpdateLocomotionActions();
            }
        }

        public bool smoothTurnEnabled
        {
            get => m_SmoothTurnEnabled;
            set
            {
                m_SmoothTurnEnabled = value;
                UpdateLocomotionActions();
            }
        }

        bool m_Teleporting;

        /// <summary>
        /// Temporary scratch list to populate with the group members of the interaction group.
        /// </summary>
        static readonly List<IXRGroupMember> s_GroupMembers = new List<IXRGroupMember>();

        // For our input mediation, we are enforcing a few rules between direct, ray, and teleportation interaction:
        // 1. If the Teleportation Ray is engaged, the Ray interactor is disabled
        // 2. The interaction group ensures that the Direct and Ray interactors cannot interact at the same time, with the Direct interactor taking priority
        // 3. If the Ray interactor is selecting, all locomotion controls are disabled (teleport ray, move, and turn controls) to prevent input collision
        void SetupInteractorEvents()
        {
            if (m_RayInteractor != null)
            {
                m_RayInteractor.selectEntered.AddListener(OnRaySelectEntered);
                m_RayInteractor.selectExited.AddListener(OnRaySelectExited);
            }

            var teleportModeActivateAction = GetInputAction(m_TeleportModeActivate);
            if (teleportModeActivateAction != null)
            {
                teleportModeActivateAction.performed += OnStartTeleport;
                teleportModeActivateAction.canceled += OnCancelTeleport;
            }

            var teleportModeCancelAction = GetInputAction(m_TeleportModeCancel);
            if (teleportModeCancelAction != null)
            {
                teleportModeCancelAction.performed += OnCancelTeleport;
            }
        }

        void TeardownInteractorEvents()
        {
            if (m_RayInteractor != null)
            {
                m_RayInteractor.selectEntered.RemoveListener(OnRaySelectEntered);
                m_RayInteractor.selectExited.RemoveListener(OnRaySelectExited);
            }

            var teleportModeActivateAction = GetInputAction(m_TeleportModeActivate);
            if (teleportModeActivateAction != null)
            {
                teleportModeActivateAction.performed -= OnStartTeleport;
                teleportModeActivateAction.canceled -= OnCancelTeleport;
            }

            var teleportModeCancelAction = GetInputAction(m_TeleportModeCancel);
            if (teleportModeCancelAction != null)
            {
                teleportModeCancelAction.performed -= OnCancelTeleport;
            }
        }

        void OnStartTeleport(InputAction.CallbackContext context)
        {
            m_Teleporting = true;

            if (m_TeleportInteractor != null)
                m_TeleportInteractor.gameObject.SetActive(true);

            RayInteractorUpdate();
        }

        void OnCancelTeleport(InputAction.CallbackContext context)
        {
            m_Teleporting = false;

            // Do not deactivate the teleport interactor in this callback.
            // We delay turning off the teleport interactor in this callback so that
            // the teleport interactor has a chance to complete the teleport if needed.
            // OnAfterInteractionEvents will handle deactivating its GameObject.

            RayInteractorUpdate();
        }

        void RayInteractorUpdate()
        {
            if (m_RayInteractor != null)
                m_RayInteractor.gameObject.SetActive(!m_Teleporting);
        }

        void OnRaySelectEntered(SelectEnterEventArgs args)
        {
            // Disable locomotion and turn actions
            DisableLocomotionActions();
        }

        void OnRaySelectExited(SelectExitEventArgs args)
        {
            // Re-enable the locomotion and turn actions
            UpdateLocomotionActions();
        }

        protected void Awake()
        {
            // Start the coroutine that executes code after the Update phase (during yield null).
            // This routine is started during Awake to ensure the code after
            // the first yield will execute after Update but still on the first frame.
            // If started in Start, Unity would not resume execution until the second frame.
            // See https://docs.unity3d.com/Manual/ExecutionOrder.html
            StartCoroutine(OnAfterInteractionEvents());
        }

        protected void OnEnable()
        {
            if (m_TeleportInteractor != null)
                m_TeleportInteractor.gameObject.SetActive(false);

            SetupInteractorEvents();
        }

        protected void OnDisable()
        {
            TeardownInteractorEvents();
        }

        protected void Start()
        {
            // Ensure the enabled state of locomotion and turn actions are properly set up.
            // Called in Start so it is done after the InputActionManager enables all input actions earlier in OnEnable.
            UpdateLocomotionActions();

            if (m_ManipulationInteractionGroup == null)
            {
                Debug.LogError("Missing required Manipulation Interaction Group reference. Use the Inspector window to assign the XR Interaction Group component reference.", this);
                return;
            }

            // Ensure interactors are properly set up in the interaction group by adding
            // them if necessary and ordering Direct before Ray interactor.
            var directInteractorIndex = -1;
            var rayInteractorIndex = -1;
            m_ManipulationInteractionGroup.GetGroupMembers(s_GroupMembers);
            for (var i = 0; i < s_GroupMembers.Count; ++i)
            {
                var groupMember = s_GroupMembers[i];
                if (ReferenceEquals(groupMember, m_DirectInteractor))
                    directInteractorIndex = i;
                else if (ReferenceEquals(groupMember, m_RayInteractor))
                    rayInteractorIndex = i;
            }

            if (directInteractorIndex < 0)
            {
                // Must add Direct interactor to group, and make sure it is ordered before the Ray interactor
                if (rayInteractorIndex < 0)
                {
                    // Must add Ray interactor to group
                    m_ManipulationInteractionGroup.AddGroupMember(m_DirectInteractor);
                    m_ManipulationInteractionGroup.AddGroupMember(m_RayInteractor);
                }
                else
                {
                    m_ManipulationInteractionGroup.MoveGroupMemberTo(m_DirectInteractor, rayInteractorIndex);
                }
            }
            else
            {
                if (rayInteractorIndex < 0)
                {
                    // Must add Ray interactor to group
                    m_ManipulationInteractionGroup.AddGroupMember(m_RayInteractor);
                }
                else
                {
                    // Must make sure Direct interactor is ordered before the Ray interactor
                    if (rayInteractorIndex < directInteractorIndex)
                    {
                        m_ManipulationInteractionGroup.MoveGroupMemberTo(m_DirectInteractor, rayInteractorIndex);
                    }
                }
            }
        }

        IEnumerator OnAfterInteractionEvents()
        {
            // Avoid comparison to null each frame since that operation is somewhat expensive
            if (m_TeleportInteractor == null)
                yield break;

            while (true)
            {
                // Yield so this coroutine is resumed after the teleport interactor
                // has a chance to process its select interaction event.
                yield return null;

                if (!m_Teleporting && m_TeleportInteractor.gameObject.activeSelf)
                    m_TeleportInteractor.gameObject.SetActive(false);
            }
        }

        void UpdateLocomotionActions()
        {
            // Disable/enable Teleport and Turn when Move is enabled/disabled.
            SetEnabled(m_Move, m_SmoothMotionEnabled);
            SetEnabled(m_TeleportModeActivate, !m_SmoothMotionEnabled);
            SetEnabled(m_TeleportModeCancel, !m_SmoothMotionEnabled);

            // Disable ability to turn when using continuous movement
            SetEnabled(m_Turn, !m_SmoothMotionEnabled && m_SmoothTurnEnabled);
            SetEnabled(m_SnapTurn, !m_SmoothMotionEnabled && !m_SmoothTurnEnabled);
        }

        void DisableLocomotionActions()
        {
            DisableAction(m_Move);
            DisableAction(m_TeleportModeActivate);
            DisableAction(m_TeleportModeCancel);
            DisableAction(m_Turn);
            DisableAction(m_SnapTurn);
        }

        static void SetEnabled(InputActionReference actionReference, bool enabled)
        {
            if (enabled)
                EnableAction(actionReference);
            else
                DisableAction(actionReference);
        }

        static void EnableAction(InputActionReference actionReference)
        {
            var action = GetInputAction(actionReference);
            if (action != null && !action.enabled)
                action.Enable();
        }

        static void DisableAction(InputActionReference actionReference)
        {
            var action = GetInputAction(actionReference);
            if (action != null && action.enabled)
                action.Disable();
        }

        static InputAction GetInputAction(InputActionReference actionReference)
        {
#pragma warning disable IDE0031 // Use null propagation -- Do not use for UnityEngine.Object types
            return actionReference != null ? actionReference.action : null;
#pragma warning restore IDE0031
        }
    }
}
