using System.Text;
using TMPro;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class DisplayCameraExposureInformation : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI m_InfoText;

        [SerializeField]
        ARCameraManager m_CameraManager;

        [SerializeField]
        CameraExposureController m_CameraExposureController;

#if UNITY_IOS
        /// <summary>
        /// A string builder used for logging.
        /// </summary>
        readonly StringBuilder m_StringBuilder = new();

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
            if (m_CameraExposureController == null)
            {
                Debug.LogWarning(
                    $"A reference to {nameof(CameraExposureController)} is required. {nameof(DisplayCameraExposureInformation)} will be disabled.");
                enabled = false;
            }
        }

        void Update()
        {
            if (!m_CameraExposureController.enabled)
            {
                enabled = false;
                return;
            }

            // Clear the string builder for a new log information.
            m_StringBuilder.Clear();

            // Log the exposure states.
            BuildExposureInfo();

            LogText(m_StringBuilder.ToString());
        }

        void BuildExposureInfo()
        {
            m_StringBuilder.AppendLine("Camera Exposure");
            var locked = m_CameraExposureController.cameraLocked ? "Yes" : "No";
            m_StringBuilder.AppendLine($"      Locked: {locked}");
            m_StringBuilder.AppendLine($"      Mode: {m_CameraExposureController.currentExposureMode}");

            var exposure = m_CameraExposureController.currentExposure;
            m_StringBuilder.AppendLine($"      Duration: {exposure.duration}");
            m_StringBuilder.AppendLine($"      ISO: {exposure.iso}");
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
