namespace UnityEngine.XR.Interaction.Toolkit.Samples.Hands
{
    /// <summary>
    /// Toggles the active state of a GameObject.
    /// </summary>
    public class ToggleGameObject : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The GameObject to toggle the active state for.")]
        GameObject m_ActivationGameObject;

        /// <summary>
        /// The GameObject to toggle the active state for.
        /// </summary>
        public GameObject activationGameObject
        {
            get => m_ActivationGameObject;
            set => m_ActivationGameObject = value;
        }

        [SerializeField]
        [Tooltip("Whether the GameObject is currently active.")]
        bool m_CurrentlyActive;

        /// <summary>
        /// Whether the GameObject is currently active.
        /// </summary>
        public bool currentlyActive
        {
            get => m_CurrentlyActive;
            set
            {
                m_CurrentlyActive = value;
                activationGameObject.SetActive(m_CurrentlyActive);
            }
        }

        /// <summary>
        /// Toggles the active state of the GameObject.
        /// </summary>
        public void ToggleActiveState()
        {
            m_CurrentlyActive = !m_CurrentlyActive;
            activationGameObject.SetActive(m_CurrentlyActive);
        }
    }
}
