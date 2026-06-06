using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// This component is designed to easily toggle a specific component and GameObject on or off when an object
    /// enters the specified <see cref="triggerVolume"/>.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ToggleComponentZone : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Collider that will trigger the component to turn on or off when entering the Trigger Volume. Must have a Rigidbody component and be on the same physics layer as the Trigger Volume.")]
        Collider m_ActivationObject;

        /// <summary>
        /// Collider that will trigger the component to turn on or off when entering the Trigger Volume.
        /// Must have a Rigidbody component and be on the same physics layer as the Trigger Volume.
        /// </summary>
        public Collider activationObject
        {
            get => m_ActivationObject;
            set => m_ActivationObject = value;
        }

        [SerializeField]
        [Tooltip("Sets whether to enable or disable the Component To Toggle and GameObject To Toggle upon entry into the Trigger Volume.")]
        bool m_EnableOnEntry = true;

        /// <summary>
        /// Sets whether to enable or disable the Component To Toggle and GameObject To Toggle upon entry into the Trigger Volume.
        /// </summary>
        public bool enableOnEntry
        {
            get => m_EnableOnEntry;
            set => m_EnableOnEntry = value;
        }

        [SerializeField]
        [Tooltip("Components to set the enabled state for. Will set the value to the Enable On Entry value upon entry and revert to original value on exit.")]
        List<Behaviour> m_ComponentsToToggle = new List<Behaviour>();

        /// <summary>
        /// Component to set the enabled state for. Will set the value to the
        /// Enable On Entry value upon entry and revert to original value on exit.
        /// </summary>
        public List<Behaviour> componentsToToggle
        {
            get => m_ComponentsToToggle;
            set => m_ComponentsToToggle = value;
        }

        [SerializeField]
        [Tooltip("Array of GameObjects to set the enabled state for. Will set the value to the Enable On Entry value upon entry and revert to original value on exit.")]
        List<GameObject> m_GameObjectsToToggle = new List<GameObject>();

        /// <summary>
        /// GameObject to set the enabled state for. Will set the value to the
        /// Enable On Entry value upon entry and revert to original value on exit.
        /// </summary>
        public List<GameObject> gameObjectsToToggle
        {
            get => m_GameObjectsToToggle;
            set => m_GameObjectsToToggle = value;
        }

        Collider m_TriggerVolume;
        Dictionary<Behaviour, bool> m_InitialComponentStateOnEntry;
        Dictionary<GameObject, bool> m_InitialGameObjectStateOnEntry;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Start()
        {
            if (m_TriggerVolume == null && !TryGetComponent(out m_TriggerVolume))
            {
                enabled = false;
                return;
            }

            if (!m_TriggerVolume.isTrigger)
            {
                m_TriggerVolume.isTrigger = true;
                Debug.LogWarning($"Trigger Volume \"{m_TriggerVolume}\" was not set as trigger, which the Toggle Component Zone expects. It has been forced to be a trigger.", this);
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnTriggerEnter(Collider other)
        {
            if (other == null || other != m_ActivationObject)
                return;

            // Save the target GameObject(s) active state to restore when leaving the zone
            if (m_GameObjectsToToggle != null && m_GameObjectsToToggle.Count > 0)
            {
                m_InitialGameObjectStateOnEntry ??= new Dictionary<GameObject, bool>(m_GameObjectsToToggle.Count);
                m_InitialGameObjectStateOnEntry.Clear();

                for (var i = 0; i < m_GameObjectsToToggle.Count; ++i)
                {
                    var target = m_GameObjectsToToggle[i];
                    m_InitialGameObjectStateOnEntry.Add(target, target.activeSelf);
                    target.SetActive(m_EnableOnEntry);
                }
            }

            // Save the target component(s) enabled state to restore when leaving the zone
            if (m_ComponentsToToggle != null && m_ComponentsToToggle.Count > 0)
            {
                m_InitialComponentStateOnEntry ??= new Dictionary<Behaviour, bool>(m_ComponentsToToggle.Count);
                m_InitialComponentStateOnEntry.Clear();

                for (var i = 0; i < m_ComponentsToToggle.Count; ++i)
                {
                    var target = m_ComponentsToToggle[i];
                    m_InitialComponentStateOnEntry.Add(target, target.enabled);
                    target.enabled = m_EnableOnEntry;
                }
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnTriggerExit(Collider other)
        {
            if (other == null || other != m_ActivationObject)
                return;

            // Restore original target component(s) enabled state
            if (m_ComponentsToToggle != null && m_ComponentsToToggle.Count > 0 && m_InitialComponentStateOnEntry != null)
            {
                if (m_InitialComponentStateOnEntry.Count == m_ComponentsToToggle.Count)
                {
                    for (var i = 0; i < m_ComponentsToToggle.Count; ++i)
                    {
                        var component = m_ComponentsToToggle[i];
                        if (m_InitialComponentStateOnEntry.TryGetValue(component, out var initialState))
                            component.enabled = initialState;
                    }
                }
                else
                {
                    Debug.LogWarning("List of Components to Toggle changed in count between entering and exiting the Trigger Volume," +
                        " which is not supported by this component. Cannot restore original enabled state.", this);
                }
            }

            // Restore original target GameObject(s) active state
            if (m_GameObjectsToToggle != null && m_GameObjectsToToggle.Count > 0 && m_InitialGameObjectStateOnEntry != null)
            {
                if (m_InitialGameObjectStateOnEntry.Count == m_GameObjectsToToggle.Count)
                {
                    for (var i = 0; i < m_GameObjectsToToggle.Count; ++i)
                    {
                        var go = m_GameObjectsToToggle[i];
                        if (m_InitialGameObjectStateOnEntry.TryGetValue(go, out var initialState))
                            go.SetActive(initialState);
                    }
                }
                else
                {
                    Debug.LogWarning("List of GameObjects to Toggle changed in count between entering and exiting the Trigger Volume," +
                        " which is not supported by this component. Cannot restore original active state.", this);
                }
            }
        }
    }
}
