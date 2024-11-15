using System;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Use this class in combination with `UnityEvent` to enable or disable a component as a response to an event.
    /// </summary>
    public class ToggleComponent : MonoBehaviour
    {
        [SerializeField]
        MonoBehaviour m_TargetComponent;

        public MonoBehaviour targetComponent
        {
            get => m_TargetComponent;
            set => m_TargetComponent = value;
        }

        void OnEnable()
        {
            if (m_TargetComponent == null)
            {
                Debug.LogError($"{nameof(m_TargetComponent)} is null in {nameof(ToggleComponent)} on GameObject {name}. Disabling component.", this);
                enabled = false;
            }
        }

        public void Toggle()
        {
            if (!enabled)
                return;

            m_TargetComponent.enabled = !m_TargetComponent.enabled;
        }
    }
}
