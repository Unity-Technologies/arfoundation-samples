using System;
using UnityEngine;
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
        double m_MaxGPUFrameTime = 0.0;
        double m_MinGPUFrameTime = 1000.0;

        string m_BackgroundRenderString;

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
            return $"GPU Frame Time:\n\t{frameTimings[0].gpuFrameTime}ms\n" +
                $"Max GPU Frame Time:\n\t{m_MaxGPUFrameTime}ms\n" +
                $"Min GPU Frame Time:\n\t{m_MinGPUFrameTime}ms";
        }

        void OnRenderModeChangeRequested()
        {
            Reset();
            m_BackgroundRenderString = $"Current Render Mode: {activeRenderMode}\nRequested Render Mode: {requestedRenderingMode}";

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
#if UNITY_2023_1_OR_NEWER
                obj = FindAnyObjectByType<T>();
#else
                obj = FindObjectOfType<T>();
#endif
                return obj != null;
            }

            return true;
        }
    }
}
