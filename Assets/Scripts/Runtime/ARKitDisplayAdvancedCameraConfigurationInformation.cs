using System;
using System.Text;
using TMPro;

namespace UnityEngine.XR.ARFoundation.Samples
{
#if UNITY_IOS
    public abstract class ARKitDisplayAdvancedCameraConfigurationInformation<TController, TMode, TConfigValue> : MonoBehaviour
        where TController : ARKitAdvancedCameraConfigurationController<TMode, TConfigValue>
        where TMode : Enum
        where TConfigValue : struct
#else
    public abstract class ARKitDisplayAdvancedCameraConfigurationInformation<TController> : MonoBehaviour
        where TController : ARKitAdvancedCameraConfigurationController
#endif
    {
        [SerializeField]
        TextMeshProUGUI m_InfoText;

        [SerializeField]
        ARCameraManager m_CameraManager;

        [SerializeField]
        protected TController m_Controller;

#if UNITY_IOS
        /// <summary>
        /// A string builder used for logging.
        /// </summary>
        protected readonly StringBuilder m_StringBuilder = new();

        /// <summary>
        /// The text UI to display the logging information.
        /// </summary>
        public TextMeshProUGUI infoText
        {
            get => m_InfoText;
            set => m_InfoText = value;
        }

        /// <summary>
        /// The camera manager for logging.
        /// </summary>
        public ARCameraManager cameraManager
        {
            get => m_CameraManager;
            set => m_CameraManager = value;
        }

        void Awake()
        {
            if (m_Controller == null)
            {
                Debug.LogWarning(
                    $"A reference to {nameof(TController)} is required. {GetType().Name} will be disabled.");
                enabled = false;
            }
        }

        void Update()
        {
            if (!m_Controller.enabled)
            {
                enabled = false;
                return;
            }

            // Clear the string builder for a new log information.
            m_StringBuilder.Clear();

            // Log the config states.
            BuildInfo();

            LogText(m_StringBuilder.ToString());
        }

        void BuildInfo()
        {
            var locked = m_Controller.cameraLocked ? "Yes" : "No";
            m_StringBuilder.AppendLine($"Locked: {locked}");
            m_StringBuilder.AppendLine($"Mode: {m_Controller.currentMode}");

            var configValue = m_Controller.currentValue;
            m_StringBuilder.AppendLine(configValue.GetType().Name);
            m_StringBuilder.AppendLine(configValue.ToString());
        }

        /// <summary>
        /// Log the given text to the screen if the image info UI is set. Otherwise, log the string to debug.
        /// </summary>
        /// <param name="text">The text string to log.</param>
        void LogText(string text)
        {
            if (m_InfoText != null)
            {
                m_InfoText.text = text;
            }
            else
            {
                Debug.Log(text);
            }
        }
#endif // UNITY_IOS
    }
}
