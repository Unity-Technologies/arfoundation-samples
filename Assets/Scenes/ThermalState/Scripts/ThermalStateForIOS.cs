using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// A component that provides functionality to query for thermal state on iOS devices and to subscribe to an event
    /// that fires when the thermal state changes.
    /// </summary>
    public class ThermalStateForIOS : MonoBehaviour
    {
#if UNITY_IOS && !UNITY_EDITOR
        /// <summary>
        /// The thermal state on the previous update, used for determining when to fire the state changed event.
        /// </summary>
        ThermalState? m_PreviousThermalState;
#endif // UNITY_IOS && !UNITY_EDITOR

        /// <summary>
        /// Event that fires when the thermal state has changed.
        /// </summary>
        public Action<ThermalStateChange> stateChanged;

        /// <summary>
        /// iOS thermal states as documented by
        /// https://developer.apple.com/library/archive/documentation/Performance/Conceptual/power_efficiency_guidelines_osx/RespondToThermalStateChanges.html
        /// <summary>
        public enum ThermalState
        {
            /// <summary>
            /// Thermal state is unknown.
            /// </summary>
            [Description("Unknown")]
            Unknown = 0,

            /// <summary>
            /// The thermal state is at an acceptable level.
            /// </summary>
            [Description("Nominal")]
            Nominal = 1,

            /// <summary>
            /// The thermal state is minimally elevated.
            /// </summary>
            [Description("Fair")]
            Fair = 2,

            /// <summary>
            /// The thermal state is highly elevated.
            /// </summary>
            [Description("Serious")]
            Serious = 3,

            /// <summary>
            /// The thermal state is significantly elevated.
            /// </summary>
            [Description("Critical")]
            Critical = 4,
        }

        /// <summary>
        /// Queries the current thermal state of the iOS device.
        /// </summary>
        public ThermalState currentThermalState => NativeApi.GetCurrentThermalState();

#if UNITY_IOS && !UNITY_EDITOR
        /// <summary>
        /// On enable, initialize the previous thermal state to null.
        /// </summary>
        void OnEnable()
        {
            m_PreviousThermalState = null;
        }

        /// <summary>
        /// On each update, query the current thermal state, and fire the state changed event if the thermal state has
        /// changed since the previous update.
        /// </summary>
        void Update()
        {
            ThermalState thermalState = currentThermalState;

            if (m_PreviousThermalState.HasValue && (thermalState != m_PreviousThermalState.Value))
            {
                if (stateChanged != null)
                {
                    stateChanged(new ThermalStateChange(m_PreviousThermalState.Value, thermalState));
                }
            }

            m_PreviousThermalState = thermalState;
        }
#endif // UNITY_IOS && !UNITY_EDITOR

        /// <summary>
        /// Struct containing both the previous and current thermal states when a state change occurs.
        /// </summary>
        public struct ThermalStateChange
        {
            /// <summary>
            /// The previous thermal state for a state change event.
            /// </summary>
            ThermalState m_PreviousThermalState;

            /// <summary>
            /// The current thermal state for a state change event.
            /// </summary>
            ThermalState m_CurrentThermalState;

            /// <summary>
            /// The previous thermal state for a state change event.
            /// </summary>
            public ThermalState previousThermalState => m_PreviousThermalState;

            /// <summary>
            /// The current thermal state for a state change event.
            /// </summary>
            public ThermalState currentThermalState => m_CurrentThermalState;

            /// <summary>
            /// Constructs a thermal state change with the previous and current thermal states.
            /// </summary>
            /// <param name="previousThermalState">The previous thermal state for a state change event.</param>
            /// <param name="currentThermalState">The current thermal state for a state change event.</param>
            public ThermalStateChange (ThermalState previousThermalState, ThermalState currentThermalState)
            {
                m_PreviousThermalState = previousThermalState;
                m_CurrentThermalState = currentThermalState;
            }
        }

        /// <summary>
        /// Native API for querying the thermal state on iOS. For other platforms, this API is stubbed out.
        /// /<summary>
        static class NativeApi
        {
#if UNITY_IOS && !UNITY_EDITOR
            [DllImport("__Internal", EntryPoint = "ARFoundationSamples_GetCurrentThermalState")]
            public static extern ThermalState GetCurrentThermalState();
#else // UNITY_IOS && !UNITY_EDITOR
            public static ThermalState GetCurrentThermalState() => ThermalState.Unknown;
#endif // UNITY_IOS && !UNITY_EDITOR
        }
    }
}
