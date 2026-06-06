using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.Hands
{
    /// <summary>
    /// An input button reader based on another <see cref="XRInputButtonReader"/> and holds it true until falling below a lower release threshold.
    /// Useful with hand interaction because the bool select value can bounce when the hand is near the tight internal threshold,
    /// so using this will keep the pinch true until moving the fingers much further away than the pinch activation threshold.
    /// </summary>
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_XRInputDeviceButtonReader)]
    public class ReleaseThresholdButtonReader : MonoBehaviour, IXRInputButtonReader
    {
        [SerializeField]
        [Tooltip("The source input that this component reads to create a processed button value.")]
        XRInputButtonReader m_ValueInput = new XRInputButtonReader("Value");

        /// <summary>
        /// The source input that this component reads to create a processed button value.
        /// </summary>
        public XRInputButtonReader valueInput
        {
            get => m_ValueInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_ValueInput, value, this);
        }

        [SerializeField]
        [Tooltip("The threshold value to use to determine when the button is pressed. Considered pressed equal to or greater than this value.")]
        [Range(0f, 1f)]
        float m_PressThreshold = 1f;

        /// <summary>
        /// The threshold value to use to determine when the button is pressed. Considered pressed equal to or greater than this value.
        /// </summary>
        /// <remarks>
        /// This reader will also be considered performed if the source input is performed.
        /// </remarks>
        public float pressThreshold
        {
            get => m_PressThreshold;
            set => m_PressThreshold = value;
        }

        [SerializeField]
        [Tooltip("The threshold value to use to determine when the button is released when it was previously pressed. Keeps being pressed until falls back to a value of or below this value.")]
        [Range(0f, 1f)]
        float m_ReleaseThreshold = 0.9f;

        /// <summary>
        /// The threshold value to use to determine when the button is released when it was previously pressed.
        /// Keeps being pressed until falls back to a value of or below this value.
        /// </summary>
        /// <remarks>
        /// This reader will still be considered performed if the source input is still performed
        /// when this threshold is reached.
        /// </remarks>
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
            // Go true when either the press threshold is reached or the bool is already performed.
            // Only drop back to false when the release threshold is reached and the bool is no longer performed.
            var prevPerformed = m_IsPerformed;
            var pressAmount = m_ValueInput.ReadValue();
            m_IsPerformed = m_ValueInput.ReadIsPerformed() || prevPerformed ? pressAmount > m_ReleaseThreshold : pressAmount >= m_PressThreshold;

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
