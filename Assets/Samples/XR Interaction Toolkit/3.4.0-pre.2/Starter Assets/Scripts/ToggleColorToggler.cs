using UnityEngine.UI;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// Updates the normal color of a toggle based on the state of the toggle.
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class ToggleColorToggler : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Normal color for the toggle in the on state.")]
        Color m_OnColor = new Color(32 / 255f, 150 / 255f, 243 / 255f);

        /// <summary>
        /// Normal color for the toggle in the on state.
        /// </summary>
        public Color onColor
        {
            get => m_OnColor;
            set => m_OnColor = value;
        }

        [SerializeField]
        [Tooltip("Normal color for the toggle in the off state.")]
        Color m_OffColor = new Color(46 / 255f, 46 / 255f, 46 / 255f);

        /// <summary>
        /// Normal color for the toggle in the off state.
        /// </summary>
        public Color offColor
        {
            get => m_OffColor;
            set => m_OffColor = value;
        }

        Toggle m_TargetToggle;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Awake()
        {
            m_TargetToggle = GetComponent<Toggle>();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnEnable()
        {
            m_TargetToggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnDisable()
        {
            m_TargetToggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }

        void OnToggleValueChanged(bool isOn)
        {
            var toggleColors = m_TargetToggle.colors;
            toggleColors.normalColor = isOn ? m_OnColor : m_OffColor;
            m_TargetToggle.colors = toggleColors;
        }
    }
}
