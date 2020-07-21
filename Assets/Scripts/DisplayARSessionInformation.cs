using System;
using System.Text;
using Unity.Collections;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
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
            LogCameraManger();
            LogPlaneManger();
            LogOcclusionManger();

            // If there is a text UI, set the text to the UI. Otherwise, debug log to console.
            if (m_InfoText == null)
            {
                Debug.Log(m_StringBuilder.ToString());
            }
            else
            {
                m_InfoText.text = m_StringBuilder.ToString();
            }
        }

        void LogCameraManger()
        {
            m_StringBuilder.Append("ARCameraManager\n");
            if (m_CameraManager == null)
            {
                m_StringBuilder.Append("   <null>\n");
            }
            else if (!m_CameraManager.enabled)
            {
                m_StringBuilder.Append("   <disabled>\n");
            }
            else
            {
                m_StringBuilder.Append("   Facing direction:\n");
                m_StringBuilder.Append($"      Requested: {m_CameraManager.requestedFacingDirection}\n");
                m_StringBuilder.Append($"      Current: {m_CameraManager.currentFacingDirection}\n");
                m_StringBuilder.Append("   Auto-focus:\n");
                m_StringBuilder.Append($"      Requested: {m_CameraManager.autoFocusRequested}\n");
                m_StringBuilder.Append($"      Current: {m_CameraManager.autoFocusEnabled}\n");
            }
        }

        void LogPlaneManger()
        {
            m_StringBuilder.Append("ARPlaneManager\n");
            if (m_PlaneManager == null)
            {
                m_StringBuilder.Append("   <null>\n");
            }
            else if (!m_PlaneManager.enabled)
            {
                m_StringBuilder.Append("   <disabled>\n");
            }
            else
            {
                m_StringBuilder.Append("   Detection mode:\n");
                m_StringBuilder.Append($"      Requested: {m_PlaneManager.requestedDetectionMode}\n");
                m_StringBuilder.Append($"      Current: {m_PlaneManager.currentDetectionMode}\n");
            }
        }

        void LogOcclusionManger()
        {
            m_StringBuilder.Append("AROcclusionManager\n");
            if (m_OcclusionManager == null)
            {
                m_StringBuilder.Append("   <null>\n");
            }
            else if (!m_OcclusionManager.enabled)
            {
                m_StringBuilder.Append("   <disabled>\n");
            }
            else
            {
                m_StringBuilder.Append("   Human stencil mode:\n");
                m_StringBuilder.Append($"      Requested: {m_OcclusionManager.requestedHumanStencilMode}\n");
                m_StringBuilder.Append($"      Current: {m_OcclusionManager.currentHumanStencilMode}\n");
                m_StringBuilder.Append("   Human depth mode:\n");
                m_StringBuilder.Append($"      Requested: {m_OcclusionManager.requestedHumanDepthMode}\n");
                m_StringBuilder.Append($"      Current: {m_OcclusionManager.currentHumanDepthMode}\n");
            }
        }
    }
}
