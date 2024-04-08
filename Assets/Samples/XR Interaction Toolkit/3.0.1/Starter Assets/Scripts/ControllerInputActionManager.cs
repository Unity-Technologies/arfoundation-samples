using System.Collections.Generic;
using Unity.XR.CoreUtils.Bindings;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// Use this class to mediate the interactors for a controller under different interaction states
    /// and the input actions used by them.
    /// </summary>
    /// <remarks>
    /// If the teleport ray input is engaged, the Ray Interactor used for distant manipulation is disabled
    /// and the Ray Interactor used for teleportation is enabled. If the Ray Interactor is selecting and it
    /// is configured to allow for attach transform manipulation, all locomotion input actions are disabled
    /// (teleport ray, move, and turn controls) to prevent input collision with the manipulation inputs used
    /// by the ray interactor.
    /// <br />
    /// A typical hierarchy also includes an XR Interaction Group component to mediate between interactors.
    /// The interaction group ensures that the Direct and Ray Interactors cannot interact at the same time,
    /// with the Direct Interactor taking priority over the Ray Interactor.
    /// </remarks>
    [AddComponentMenu("XR/Controller Input Action Manager")]
    public class ControllerInputActionManager : MonoBehaviour
    {
        [Space]
        [Header("Interactors")]

        [SerializeField]
        [Tooltip("The interactor used for distant/ray manipulation. Use this or Near-Far Interactor, not both.")]
        XRRayInteractor m_RayInteractor;

        [SerializeField]
        [Tooltip("Near-Far Interactor used for distant/ray manipulation. Use this or Ray Interactor, not both.")]
        NearFarInteractor m_NearFarInteractor;
        
        [SerializeField]
        [Tooltip("The interactor used for teleportation.")]
        XRRayInteractor m_TeleportInteractor;

        [Space]
        [Header("Controller Actions")]

        [SerializeField]
        [Tooltip("The reference to the action to start the teleport aiming mode for this controller.")]
        [FormerlySerializedAs("m_TeleportModeActivate")]
        InputActionReference m_TeleportMode;

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

        [SerializeField]
        [Tooltip("The reference to the action of scrolling UI with this controller.")]
        InputActionReference m_UIScroll;

        [Space]
        [Header("Locomotion Settings")]

        [SerializeField]
        [Tooltip("If true, continuous movement will be enabled. If false, teleport will be enabled.")]
        bool m_SmoothMotionEnabled;
        
        [SerializeField]
        [Tooltip("If true, continuous turn will be enabled. If false, snap turn will be enabled. Note: If smooth motion is enabled and enable strafe is enabled on the continuous move provider, turn will be overriden in favor of strafe.")]
        bool m_SmoothTurnEnabled;
        
        [SerializeField]
        [Tooltip("With the Near-Far Interactor, if true, teleport will be enabled during near interaction. If false, teleport will be disabled during near interaction.")]
        bool m_NearFarEnableTeleportDuringNearInteraction = true;

        [Space]
        [Header("UI Settings")]

        [SerializeField]
        [Tooltip("If true, UI scrolling will be enabled. Locomotion will be disabled when pointing at UI to allow it to be scrolled.")]
        bool m_UIScrollingEnabled = true;

        [Space]
        [Header("Mediation Events")]

        [SerializeField]
        [Tooltip("Event fired when the active ray interactor changes between interaction and teleport.")]
        UnityEvent<IXRRayProvider> m_RayInteractorChanged;

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

        public bool uiScrollingEnabled
        {
            get => m_UIScrollingEnabled;
            set
            {
                m_UIScrollingEnabled = value;
                UpdateUIActions();
            }
        }

        bool m_StartCalled;
        bool m_PostponedDeactivateTeleport;
        bool m_HoveringScrollableUI;

        readonly HashSet<InputAction> m_LocomotionUsers = new HashSet<InputAction>();
        readonly BindingsGroup m_BindingsGroup = new BindingsGroup(); 

        void SetupInteractorEvents()
        {
            if (m_NearFarInteractor != null)
            {
                m_NearFarInteractor.uiHoverEntered.AddListener(OnUIHoverEntered);
                m_NearFarInteractor.uiHoverExited.AddListener(OnUIHoverExited);
                m_BindingsGroup.AddBinding(m_NearFarInteractor.selectionRegion.Subscribe(OnNearFarSelectionRegionChanged));
            }
            
            if (m_RayInteractor != null)
            {
                m_RayInteractor.selectEntered.AddListener(OnRaySelectEntered);
                m_RayInteractor.selectExited.AddListener(OnRaySelectExited);
                m_RayInteractor.uiHoverEntered.AddListener(OnUIHoverEntered);
                m_RayInteractor.uiHoverExited.AddListener(OnUIHoverExited);
            }

            var teleportModeAction = GetInputAction(m_TeleportMode);
            if (teleportModeAction != null)
            {
                teleportModeAction.performed += OnStartTeleport;
                teleportModeAction.performed += OnStartLocomotion;
                teleportModeAction.canceled += OnCancelTeleport;
                teleportModeAction.canceled += OnStopLocomotion;
            }

            var teleportModeCancelAction = GetInputAction(m_TeleportModeCancel);
            if (teleportModeCancelAction != null)
            {
                teleportModeCancelAction.performed += OnCancelTeleport;
            }

            var moveAction = GetInputAction(m_Move);
            if (moveAction != null)
            {
                moveAction.started += OnStartLocomotion;
                moveAction.canceled += OnStopLocomotion;
            }

            var turnAction = GetInputAction(m_Turn);
            if (turnAction != null)
            {
                turnAction.started += OnStartLocomotion;
                turnAction.canceled += OnStopLocomotion;
            }

            var snapTurnAction = GetInputAction(m_SnapTurn);
            if (snapTurnAction != null)
            {
                snapTurnAction.started += OnStartLocomotion;
                snapTurnAction.canceled += OnStopLocomotion;
            }
        }

        void TeardownInteractorEvents()
        {
            m_BindingsGroup.Clear();
            
            if (m_NearFarInteractor != null)
            {
                m_NearFarInteractor.uiHoverEntered.RemoveListener(OnUIHoverEntered);
                m_NearFarInteractor.uiHoverExited.RemoveListener(OnUIHoverExited);
            }
            
            if (m_RayInteractor != null)
            {
                m_RayInteractor.selectEntered.RemoveListener(OnRaySelectEntered);
                m_RayInteractor.selectExited.RemoveListener(OnRaySelectExited);
                m_RayInteractor.uiHoverEntered.RemoveListener(OnUIHoverEntered);
                m_RayInteractor.uiHoverExited.RemoveListener(OnUIHoverExited);
            }

            var teleportModeAction = GetInputAction(m_TeleportMode);
            if (teleportModeAction != null)
            {
                teleportModeAction.performed -= OnStartTeleport;
                teleportModeAction.performed -= OnStartLocomotion;
                teleportModeAction.canceled -= OnCancelTeleport;
                teleportModeAction.canceled -= OnStopLocomotion;
            }

            var teleportModeCancelAction = GetInputAction(m_TeleportModeCancel);
            if (teleportModeCancelAction != null)
            {
                teleportModeCancelAction.performed -= OnCancelTeleport;
            }

            var moveAction = GetInputAction(m_Move);
            if (moveAction != null)
            {
                moveAction.started -= OnStartLocomotion;
                moveAction.canceled -= OnStopLocomotion;
            }

            var turnAction = GetInputAction(m_Turn);
            if (turnAction != null)
            {
                turnAction.started -= OnStartLocomotion;
                turnAction.canceled -= OnStopLocomotion;
            }

            var snapTurnAction = GetInputAction(m_SnapTurn);
            if (snapTurnAction != null)
            {
                snapTurnAction.started -= OnStartLocomotion;
                snapTurnAction.canceled -= OnStopLocomotion;
            }
        }

        void OnStartTeleport(InputAction.CallbackContext context)
        {
            m_PostponedDeactivateTeleport = false;

            if (m_TeleportInteractor != null)
                m_TeleportInteractor.gameObject.SetActive(true);

            if (m_RayInteractor != null)
                m_RayInteractor.gameObject.SetActive(false);

            if (m_NearFarInteractor != null && m_NearFarInteractor.selectionRegion.Value != NearFarInteractor.Region.Near)
                m_NearFarInteractor.gameObject.SetActive(false);
    
            m_RayInteractorChanged?.Invoke(m_TeleportInteractor);
        }

        void OnCancelTeleport(InputAction.CallbackContext context)
        {
            // Do not deactivate the teleport interactor in this callback.
            // We delay turning off the teleport interactor in this callback so that
            // the teleport interactor has a chance to complete the teleport if needed.
            // OnAfterInteractionEvents will handle deactivating its GameObject.
            m_PostponedDeactivateTeleport = true;

            if (m_RayInteractor != null)
                m_RayInteractor.gameObject.SetActive(true);
            
            if (m_NearFarInteractor != null)
                m_NearFarInteractor.gameObject.SetActive(true);

            m_RayInteractorChanged?.Invoke(m_RayInteractor);
        }
        
        void OnNearFarSelectionRegionChanged(NearFarInteractor.Region selectionRegion)
        {
            if (selectionRegion == NearFarInteractor.Region.Far ||
                (selectionRegion == NearFarInteractor.Region.Near && !m_NearFarEnableTeleportDuringNearInteraction))
                DisableTeleportActions();
            else
                UpdateLocomotionActions();
        }

        void OnStartLocomotion(InputAction.CallbackContext context)
        {
            m_LocomotionUsers.Add(context.action);
        }

        void OnStopLocomotion(InputAction.CallbackContext context)
        {
            m_LocomotionUsers.Remove(context.action);

            if (m_LocomotionUsers.Count == 0 && m_HoveringScrollableUI)
            {
                DisableAllLocomotionActions();
                UpdateUIActions();
            }
        }

        void OnRaySelectEntered(SelectEnterEventArgs args)
        {
            if (m_RayInteractor.manipulateAttachTransform)
            {
                // Disable locomotion and turn actions
                DisableAllLocomotionActions();
            }
        }

        void OnRaySelectExited(SelectExitEventArgs args)
        {
            if (m_RayInteractor.manipulateAttachTransform)
            {
                // Re-enable the locomotion and turn actions
                UpdateLocomotionActions();
            }
        }

        void OnUIHoverEntered(UIHoverEventArgs args)
        {
            m_HoveringScrollableUI = m_UIScrollingEnabled && args.deviceModel.isScrollable;
            UpdateUIActions();

            // If locomotion is occurring, wait
            if (m_HoveringScrollableUI && m_LocomotionUsers.Count == 0)
            {
                // Disable locomotion and turn actions
                DisableAllLocomotionActions();
            }
        }

        void OnUIHoverExited(UIHoverEventArgs args)
        {
            m_HoveringScrollableUI = false;
            UpdateUIActions();

            // Re-enable the locomotion and turn actions
            UpdateLocomotionActions();
        }

        protected void OnEnable()
        {
            if (m_RayInteractor != null && m_NearFarInteractor != null)
            {
                Debug.LogWarning("Both Ray Interactor and Near-Far Interactor are assigned. Only one should be assigned, not both. Clearing Ray Interactor.", this);
                m_RayInteractor = null;
            }

            if (m_TeleportInteractor != null)
                m_TeleportInteractor.gameObject.SetActive(false);

            // Allow the actions to be refreshed when this component is re-enabled.
            // See comments in Start for why we wait until Start to enable/disable actions.
            if (m_StartCalled)
            {
                UpdateLocomotionActions();
                UpdateUIActions();
            }

            SetupInteractorEvents();
        }

        protected void OnDisable()
        {
            TeardownInteractorEvents();
        }

        protected void Start()
        {
            m_StartCalled = true;

            // Ensure the enabled state of locomotion and turn actions are properly set up.
            // Called in Start so it is done after the InputActionManager enables all input actions earlier in OnEnable.
            UpdateLocomotionActions();
            UpdateUIActions();
        }

        protected void Update()
        {
            // Start the coroutine that executes code after the Update phase (during yield null).
            // Since this behavior has the default execution order, it runs after the XRInteractionManager,
            // so selection events have been finished by now this frame. This means that the teleport interactor
            // has had a chance to process its select interaction event and teleport if needed.
            if (m_PostponedDeactivateTeleport)
            {
                if (m_TeleportInteractor != null)
                    m_TeleportInteractor.gameObject.SetActive(false);

                m_PostponedDeactivateTeleport = false;
            }
        }

        void UpdateLocomotionActions()
        {
            // Disable/enable Teleport and Turn when Move is enabled/disabled.
            SetEnabled(m_Move, m_SmoothMotionEnabled);
            SetEnabled(m_TeleportMode, !m_SmoothMotionEnabled);
            SetEnabled(m_TeleportModeCancel, !m_SmoothMotionEnabled);

            // Disable ability to turn when using continuous movement
            SetEnabled(m_Turn, !m_SmoothMotionEnabled && m_SmoothTurnEnabled);
            SetEnabled(m_SnapTurn, !m_SmoothMotionEnabled && !m_SmoothTurnEnabled);
        }

        void DisableTeleportActions()
        {
            DisableAction(m_TeleportMode);
            DisableAction(m_TeleportModeCancel);
        }

        void DisableMoveAndTurnActions()
        {
            DisableAction(m_Move);
            DisableAction(m_Turn);
            DisableAction(m_SnapTurn);
        }

        void DisableAllLocomotionActions()
        {
            DisableTeleportActions();
            DisableMoveAndTurnActions();
        }

        void UpdateUIActions()
        {
            SetEnabled(m_UIScroll, m_UIScrollingEnabled && m_HoveringScrollableUI && m_LocomotionUsers.Count == 0);
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
