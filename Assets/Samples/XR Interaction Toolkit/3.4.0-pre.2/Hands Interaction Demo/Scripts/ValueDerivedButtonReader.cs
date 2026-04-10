using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.Hands
{
    /// <summary>
    /// Example class that reads a float value from an <see cref="XRInputValueReader"/> and converts it to a bool.
    /// Useful with hand interaction because the bool select value can be unreliable when the hand is near the tight internal threshold.
    /// </summary>
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_XRInputDeviceButtonReader)]
    public class ValueDerivedButtonReader : MonoBehaviour, IXRInputButtonReader
    {
        [SerializeField]
        [Tooltip("The input reader used to reference the float value to convert to a bool.")]
        XRInputValueReader<float> m_ValueInput = new XRInputValueReader<float>("Value");

        /// <summary>
        /// The input reader used to reference the float value to convert to a bool.
        /// </summary>
        public XRInputValueReader<float> valueInput
        {
            get => m_ValueInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_ValueInput, value, this);
        }

        [SerializeField]
        [Tooltip("The threshold value to use to determine when the button is pressed. Considered pressed equal to or greater than this value.")]
        [Range(0f, 1f)]
        float m_PressThreshold = 0.8f;

        /// <summary>
        /// The threshold value to use to determine when the button is pressed. Considered pressed equal to or greater than this value.
        /// </summary>
        public float pressThreshold
        {
            get => m_PressThreshold;
            set => m_PressThreshold = value;
        }

        [SerializeField]
        [Tooltip("The threshold value to use to determine when the button is released when it was previously pressed. Keeps being pressed until falls back to a value of or below this value.")]
        [Range(0f, 1f)]
        float m_ReleaseThreshold = 0.25f;

        /// <summary>
        /// The threshold value to use to determine when the button is released when it was previously pressed.
        /// Keeps being pressed until falls back to a value of or below this value.
        /// </summary>
        public float releaseThreshold
        {
            get => m_ReleaseThreshold;
            set => m_ReleaseThreshold = value;
        }

        bool m_IsPerformed;
        bool m_WasPerformedThisFrame;
        bool m_WasCompletedThisFrame;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnEnable()
        {
            m_ValueInput?.EnableDirectActionIfModeUsed();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnDisable()
        {
            m_ValueInput?.DisableDirectActionIfModeUsed();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Update()
        {
            var prevPerformed = m_IsPerformed;
            var pressAmount = m_ValueInput.ReadValue();

            var newValue = pressAmount >= m_PressThreshold;
            if (!newValue && prevPerformed)
                newValue = pressAmount > m_ReleaseThreshold;

            m_IsPerformed = newValue;
            m_WasPerformedThisFrame = !prevPerformed && m_IsPerformed;
            m_WasCompletedThisFrame = prevPerformed && !m_IsPerformed;
        }

        /// <inheritdoc />
        public bool ReadIsPerformed()
        {
            return m_IsPerformed;
        }

        /// <inheritdoc />
        public bool ReadWasPerformedThisFrame()
        {
            return m_WasPerformedThisFrame;
        }

        /// <inheritdoc />
        public bool ReadWasCompletedThisFrame()
        {
            return m_WasCompletedThisFrame;
        }

        /// <inheritdoc />
        public float ReadValue()
        {
            return m_ValueInput.ReadValue();
        }

        /// <inheritdoc />
        public bool TryReadValue(out float value)
        {
            return m_ValueInput.TryReadValue(out value);
        }
    }
}
