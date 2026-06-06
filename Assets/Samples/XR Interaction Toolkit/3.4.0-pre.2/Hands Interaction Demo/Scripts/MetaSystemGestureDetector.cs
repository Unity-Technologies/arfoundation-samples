using System;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
#if XR_HANDS_1_1_OR_NEWER
using UnityEngine.XR.Hands;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Samples.Hands
{
    /// <summary>
    /// Behavior that provides events for when the system gesture starts and ends and when the
    /// menu palm pinch gesture occurs while hand tracking is in use.
    /// </summary>
    /// <remarks>
    /// See <see href="https://docs.unity3d.com/Packages/com.unity.xr.hands@1.1/manual/features/metahandtrackingaim.html">Meta Hand Tracking Aim</see>.
    /// </remarks>
    /// <seealso cref="MetaAimHand"/>
    public class MetaSystemGestureDetector : MonoBehaviour
    {
        /// <summary>
        /// The state of the system gesture.
        /// </summary>
        /// <seealso cref="systemGestureState"/>
        public enum SystemGestureState
        {
            /// <summary>
            /// The system gesture has fully ended.
            /// </summary>
            Ended,

            /// <summary>
            /// The system gesture has started or is ongoing. Typically, this means the user is looking at
            /// their palm at eye level or has not yet released the palm pinch gesture or turned their hand around.
            /// </summary>
            Started,
        }

        [SerializeField]
        InputActionProperty m_AimFlagsAction = new InputActionProperty(new InputAction(expectedControlType: "Integer"));

        /// <summary>
        /// The Input System action to read the Aim Flags.
        /// </summary>
        /// <remarks>
        /// Typically a <b>Value</b> action type with an <b>Integer</b> control type with a binding to either:
        /// <list type="bullet">
        /// <item>
        /// <description><c>&lt;MetaAimHand&gt;{LeftHand}/aimFlags</c></description>
        /// </item>
        /// <item>
        /// <description><c>&lt;MetaAimHand&gt;{RightHand}/aimFlags</c></description>
        /// </item>
        /// </list>
        /// </remarks>
        public InputActionProperty aimFlagsAction
        {
            get => m_AimFlagsAction;
            set
            {
                if (Application.isPlaying)
                    UnbindAimFlags();

                m_AimFlagsAction = value;

                if (Application.isPlaying && isActiveAndEnabled)
                    BindAimFlags();
            }
        }

        [SerializeField]
        UnityEvent m_SystemGestureStarted;

        /// <summary>
        /// Calls the methods in its invocation list when the system gesture starts, which typically occurs when
        /// the user looks at their palm at eye level.
        /// </summary>
        /// <seealso cref="systemGestureEnded"/>
        /// <seealso cref="MetaAimFlags.SystemGesture"/>
        public UnityEvent systemGestureStarted
        {
            get => m_SystemGestureStarted;
            set => m_SystemGestureStarted = value;
        }

        [SerializeField]
        UnityEvent m_SystemGestureEnded;

        /// <summary>
        /// Calls the methods in its invocation list when the system gesture ends.
        /// </summary>
        /// <remarks>
        /// This behavior postpones ending the system gesture until the user has turned their hand around.
        /// In other words, it isn't purely based on the <see cref="MetaAimFlags.SystemGesture"/>
        /// being cleared from the aim flags in order to better replicate the native visual feedback in the Meta Home menu.
        /// </remarks>
        /// <seealso cref="systemGestureStarted"/>
        /// <seealso cref="MetaAimFlags.SystemGesture"/>
        public UnityEvent systemGestureEnded
        {
            get => m_SystemGestureEnded;
            set => m_SystemGestureEnded = value;
        }

        [SerializeField]
        UnityEvent m_MenuPressed;

        /// <summary>
        /// Calls the methods in its invocation list when the menu button is triggered by a palm pinch gesture.
        /// </summary>
        /// <remarks>
        /// This is triggered by the non-dominant hand, which is the one with the menu icon (&#x2630;).
        /// The universal menu (Oculus icon) on the dominant hand does not trigger this event.
        /// </remarks>
        /// <seealso cref="MetaAimFlags.MenuPressed"/>
        public UnityEvent menuPressed
        {
            get => m_MenuPressed;
            set => m_MenuPressed = value;
        }

        /// <summary>
        /// The state of the system gesture.
        /// </summary>
        /// <seealso cref="SystemGestureState"/>
        /// <seealso cref="systemGestureStarted"/>
        /// <seealso cref="systemGestureEnded"/>
        public IReadOnlyBindableVariable<SystemGestureState> systemGestureState => m_SystemGestureState;

        readonly BindableEnum<SystemGestureState> m_SystemGestureState = new BindableEnum<SystemGestureState>(checkEquality: false);

#if XR_HANDS_1_1_OR_NEWER && (ENABLE_VR || UNITY_GAMECORE)
        [NonSerialized] // NonSerialized is required to avoid an "Unsupported enum base type" error about the Flags enum being ulong
        MetaAimFlags m_AimFlags;
#endif

        bool m_AimFlagsBound;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            BindAimFlags();

#if XR_HANDS_1_1_OR_NEWER
#if ENABLE_VR || UNITY_GAMECORE
            var action = m_AimFlagsAction.action;
            if (action != null)
                // Force invoking the events upon initialization to simplify making sure the callback's desired results are synced
                UpdateAimFlags((MetaAimFlags)action.ReadValue<int>(), true);
#endif
#else
            Debug.LogWarning("Script requires XR Hands (com.unity.xr.hands) package to monitor Meta Aim Flags. Install using Window > Package Manager or click Fix on the related issue in Edit > Project Settings > XR Plug-in Management > Project Validation.", this);
            SetGestureState(SystemGestureState.Ended, true);
#endif
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            UnbindAimFlags();
        }

        void BindAimFlags()
        {
            if (m_AimFlagsBound)
                return;

            var action = m_AimFlagsAction.action;
            if (action == null)
                return;

            action.performed += OnAimFlagsActionPerformedOrCanceled;
            action.canceled += OnAimFlagsActionPerformedOrCanceled;
            m_AimFlagsBound = true;

            m_AimFlagsAction.EnableDirectAction();
        }

        void UnbindAimFlags()
        {
            if (!m_AimFlagsBound)
                return;

            var action = m_AimFlagsAction.action;
            if (action == null)
                return;

            m_AimFlagsAction.DisableDirectAction();

            action.performed -= OnAimFlagsActionPerformedOrCanceled;
            action.canceled -= OnAimFlagsActionPerformedOrCanceled;
            m_AimFlagsBound = false;
        }

        void SetGestureState(SystemGestureState state, bool forceInvoke)
        {
            if (!forceInvoke && m_SystemGestureState.Value == state)
                return;

            m_SystemGestureState.Value = state;
            switch (state)
            {
                case SystemGestureState.Ended:
                    m_SystemGestureEnded?.Invoke();
                    break;
                case SystemGestureState.Started:
                    m_SystemGestureStarted?.Invoke();
                    break;
            }
        }

#if XR_HANDS_1_1_OR_NEWER && (ENABLE_VR || UNITY_GAMECORE)
        void UpdateAimFlags(MetaAimFlags value, bool forceInvoke = false)
        {
            var hadMenuPressed = (m_AimFlags & MetaAimFlags.MenuPressed) != 0;
            m_AimFlags = value;
            var hasSystemGesture = (m_AimFlags & MetaAimFlags.SystemGesture) != 0;
            var hasMenuPressed = (m_AimFlags & MetaAimFlags.MenuPressed) != 0;
            var hasValid = (m_AimFlags & MetaAimFlags.Valid) != 0;
            var hasIndexPinching = (m_AimFlags & MetaAimFlags.IndexPinching) != 0;

            if (!hadMenuPressed && hasMenuPressed)
            {
                m_MenuPressed?.Invoke();
            }

            if (hasSystemGesture || hasMenuPressed)
            {
                SetGestureState(SystemGestureState.Started, forceInvoke);
                return;
            }

            if (hasValid)
            {
                SetGestureState(SystemGestureState.Ended, forceInvoke);
                return;
            }

            // We want to keep the system gesture going when the user is still index pinching
            // even though the SystemGesture flag is no longer set.
            if (hasIndexPinching && m_SystemGestureState.Value != SystemGestureState.Ended)
            {
                SetGestureState(SystemGestureState.Started, forceInvoke);
                return;
            }

            SetGestureState(SystemGestureState.Ended, forceInvoke);
        }
#endif

        void OnAimFlagsActionPerformedOrCanceled(InputAction.CallbackContext context)
        {
#if XR_HANDS_1_1_OR_NEWER && (ENABLE_VR || UNITY_GAMECORE)
            UpdateAimFlags((MetaAimFlags)context.ReadValue<int>());
#endif
        }
    }
}
