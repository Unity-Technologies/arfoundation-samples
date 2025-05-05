using UnityEngine.UI;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.Hands
{
    /// <summary>
    /// Toggles between two graphic objects based on the state of a toggle.
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class ToggleGraphicToggler : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Graphic object representing the toggle on position.")]
        Graphic m_ToggleOnGraphic;

        [SerializeField]
        [Tooltip("Graphic object representing the toggle off position.")]
        Graphic m_ToggleOffGraphic;

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
            if (isOn)
                m_TargetToggle.targetGraphic = m_ToggleOnGraphic;
            else
                m_TargetToggle.targetGraphic = m_ToggleOffGraphic;

            m_ToggleOnGraphic.gameObject.SetActive(isOn);
        }
    }
}
