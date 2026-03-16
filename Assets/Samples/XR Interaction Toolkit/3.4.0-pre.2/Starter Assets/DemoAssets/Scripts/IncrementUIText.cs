using UnityEngine.UI;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// Add this component to a GameObject and call the <see cref="IncrementText"/> method
    /// in response to a Unity Event to update a text display to count up with each event.
    /// </summary>
    public class IncrementUIText : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The Text component this behavior uses to display the incremented value.")]
        Text m_Text;

        /// <summary>
        /// The Text component this behavior uses to display the incremented value.
        /// </summary>
        public Text text
        {
            get => m_Text;
            set => m_Text = value;
        }

        int m_Count;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Awake()
        {
            if (m_Text == null)
                Debug.LogWarning("Missing required Text component reference. Use the Inspector window to assign which Text component to increment.", this);
        }

        /// <summary>
        /// Increment the string message of the Text component.
        /// </summary>
        public void IncrementText()
        {
            m_Count += 1;
            if (m_Text != null)
                m_Text.text = m_Count.ToString();
        }
    }
}
