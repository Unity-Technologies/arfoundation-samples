using System;
using System.Text;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(Text))]
    public class CurrentRenderModeText : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The ARCameraManager which will produce frame events.")]
        ARCameraManager m_CameraManager;

        [SerializeField]
        [Tooltip("Captures on render mode changed events.")]
        BackgroundRenderOrderController m_BackgroundRenderOrderController;

        Text m_CurrentRenderModeText;

        bool m_FrameTimingSupported;
        double m_MaxGPUFrameTime;
        double m_MinGPUFrameTime = 1000.0;

        string m_BackgroundRenderString;

        readonly StringBuilder m_StringBuilder = new();

        XRCameraBackgroundRenderingMode m_LastRenderMode = XRCameraBackgroundRenderingMode.None;

        XRSupportedCameraBackgroundRenderingMode requestedRenderingMode =>
            m_CameraManager?.requestedBackgroundRenderingMode.ToXRSupportedCameraBackgroundRenderingMode() ?? XRSupportedCameraBackgroundRenderingMode.None;

        XRCameraBackgroundRenderingMode activeRenderMode => m_CameraManager?.currentRenderingMode ?? XRCameraBackgroundRenderingMode.None;

        void Awake()
        {
            m_FrameTimingSupported = FrameTimingManager.GetGpuTimerFrequency() != 0;
        }

        void OnEnable()
        {
            m_CurrentRenderModeText = GetComponent<Text>();
            if (!FindObjectIfNull(ref m_CameraManager))
            {
                enabled = false;
            }

            if (FindObjectIfNull(ref m_BackgroundRenderOrderController))
            {
                m_BackgroundRenderOrderController.OnValueChanged += OnRenderModeChangeRequested;
            }
        }

        void OnDisable()
        {
            Reset();
        }

        string GetGPUFrameTimesText()
        {
            FrameTimingManager.CaptureFrameTimings();
            var frameTimings = new FrameTiming[1];
            FrameTimingManager.GetLatestTimings(1, frameTimings);
            m_MaxGPUFrameTime = Math.Max(m_MaxGPUFrameTime, frameTimings[0].gpuFrameTime);
            m_MinGPUFrameTime = Math.Min(m_MinGPUFrameTime, frameTimings[0].gpuFrameTime);
            m_StringBuilder.Clear();
            m_StringBuilder.Append("GPU Frame Time:\n\t").Append(frameTimings[0].gpuFrameTime).Append("ms\n");
            m_StringBuilder.Append("Max GPU Frame Time:\n\t").Append(m_MaxGPUFrameTime.ToString()).Append("ms\n");
            m_StringBuilder.Append("Min GPU Frame Time:\n\t").Append(m_MinGPUFrameTime.ToString()).Append("ms");
            return m_StringBuilder.ToString();
        }

        void OnRenderModeChangeRequested()
        {
            Reset();
            m_BackgroundRenderString = $"Current Render Mode: {activeRenderMode.ToString()}\nRequested Render Mode: {requestedRenderingMode.ToString()}";

            if (!m_FrameTimingSupported)
            {
                m_CurrentRenderModeText.text = $"{m_BackgroundRenderString}\nGPU Frame Timing is unsupported.";
            }
        }

        void Update()
        {
            if (m_LastRenderMode != activeRenderMode)
            {
                m_LastRenderMode = activeRenderMode;
                OnRenderModeChangeRequested();
            }

            if (m_FrameTimingSupported)
            {
                m_CurrentRenderModeText.text = $"{m_BackgroundRenderString}\n{GetGPUFrameTimesText()}";
            }
        }

        void Reset()
        {
            m_MaxGPUFrameTime = 0.0;
            m_MinGPUFrameTime = 1000.0;
        }

        static bool FindObjectIfNull<T>(ref T obj) where T : MonoBehaviour
        {
            if (obj == null)
            {
                obj = FindAnyObjectByType<T>();
                return obj != null;
            }

            return true;
        }
    }
}
