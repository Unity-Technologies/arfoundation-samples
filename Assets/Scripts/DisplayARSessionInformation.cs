using System.Text;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class DisplayARSessionInformation : MonoBehaviour
    {
        /// <summary>
        /// A string builder used for logging.
        /// </summary>
        readonly StringBuilder m_StringBuilder = new StringBuilder();

        /// <summary>
        /// The text UI to display the logging information.
        /// </summary>
        public Text infoText
        {
            get => m_InfoText;
            set => m_InfoText = value;
        }

        [SerializeField]
        Text m_InfoText;

        /// <summary>
        /// The camera manager for logging.
        /// </summary>
        public ARCameraManager cameraManager
        {
            get => m_CameraManager;
            set => m_CameraManager = value;
        }

        [SerializeField]
        ARCameraManager m_CameraManager;

        /// <summary>
        /// The plane manager for logging.
        /// </summary>
        public ARPlaneManager planeManager
        {
            get => m_PlaneManager;
            set => m_PlaneManager = value;
        }

        [SerializeField]
        ARPlaneManager m_PlaneManager;

        /// <summary>
        /// The occlusion manager for logging.
        /// </summary>
        public AROcclusionManager occlusionManager
        {
            get => m_OcclusionManager;
            set => m_OcclusionManager = value;
        }

        [SerializeField]
        AROcclusionManager m_OcclusionManager;

        void Update()
        {
            // Clear the string builder for a new log information.
            m_StringBuilder.Clear();

            // Log the various manager states.
            BuildCameraMangerInfo(m_StringBuilder);
            BuildPlaneMangerInfo(m_StringBuilder);
            BuildOcclusionMangerInfo(m_StringBuilder);

            LogText(m_StringBuilder.ToString());
        }

        /// <summary>
        /// Construct the camera manager information.
        /// </summary>
        void BuildCameraMangerInfo(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("ARCameraManager");
            if (m_CameraManager == null)
            {
                stringBuilder.AppendLine("   <null>");
            }
            else if (!m_CameraManager.enabled)
            {
                stringBuilder.AppendLine("   <disabled>");
            }
            else
            {
                stringBuilder.AppendLine("   Facing direction:");
                stringBuilder.AppendLine($"      Requested: {m_CameraManager.requestedFacingDirection}");
                stringBuilder.AppendLine($"      Current: {m_CameraManager.currentFacingDirection}");
                stringBuilder.AppendLine("   Auto-focus:");
                stringBuilder.AppendLine($"      Requested: {m_CameraManager.autoFocusRequested}");
                stringBuilder.AppendLine($"      Current: {m_CameraManager.autoFocusEnabled}");
            }
        }

        /// <summary>
        /// Construct the plane manager information.
        /// </summary>
        void BuildPlaneMangerInfo(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("ARPlaneManager");
            if (m_PlaneManager == null)
            {
                stringBuilder.AppendLine("   <null>");
            }
            else if (!m_PlaneManager.enabled)
            {
                stringBuilder.AppendLine("   <disabled>");
            }
            else
            {
                stringBuilder.AppendLine("   Detection mode:");
                stringBuilder.AppendLine($"      Requested: {m_PlaneManager.requestedDetectionMode}");
                stringBuilder.AppendLine($"      Current: {m_PlaneManager.currentDetectionMode}");
            }
        }

        /// <summary>
        /// Construct the occlusion manager information.
        /// </summary>
        void BuildOcclusionMangerInfo(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("AROcclusionManager");
            if (m_OcclusionManager == null)
            {
                stringBuilder.AppendLine("   <null>");
            }
            else if (!m_OcclusionManager.enabled)
            {
                stringBuilder.AppendLine("   <disabled>");
            }
            else
            {
                stringBuilder.AppendLine("   Environment depth mode:");
                stringBuilder.AppendLine($"      Requested: {m_OcclusionManager.requestedEnvironmentDepthMode}");
                stringBuilder.AppendLine($"      Current: {m_OcclusionManager.currentEnvironmentDepthMode}");
                stringBuilder.AppendLine("   Human stencil mode:");
                stringBuilder.AppendLine($"      Requested: {m_OcclusionManager.requestedHumanStencilMode}");
                stringBuilder.AppendLine($"      Current: {m_OcclusionManager.currentHumanStencilMode}");
                stringBuilder.AppendLine("   Human depth mode:");
                stringBuilder.AppendLine($"      Requested: {m_OcclusionManager.requestedHumanDepthMode}");
                stringBuilder.AppendLine($"      Current: {m_OcclusionManager.currentHumanDepthMode}");
            }
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
    }
}
