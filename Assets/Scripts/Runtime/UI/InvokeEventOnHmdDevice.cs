using System;
using UnityEngine.Events;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class InvokeEventOnHmdDevice : MonoBehaviour
    {
        [SerializeField, Tooltip("Forwards MonoBehaviour.OnEnable on HMDs only")]
        UnityEvent m_EnabledEvent;

        [SerializeField, Tooltip("Forwards MonoBehaviour.OnDisable on HMDs only")]
        UnityEvent m_DisabledEvent;

        public UnityEvent loadingHmdScene
        {
            get => m_EnabledEvent;
            set => m_EnabledEvent = value;
        }

        public UnityEvent disabledEvent
        {
            get => m_DisabledEvent;
            set => m_DisabledEvent = value;
        }

        void OnEnable()
        {
            if (MenuLoader.IsHmdDevice())
                m_EnabledEvent.Invoke();
        }

        void OnDisable()
        {
            if (MenuLoader.IsHmdDevice())
                m_DisabledEvent.Invoke();
        }
    }
}
