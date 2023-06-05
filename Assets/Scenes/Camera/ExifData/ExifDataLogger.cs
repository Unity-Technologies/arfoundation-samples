using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class ExifDataLogger : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The active AR Camera Manager.")]
        ARCameraManager m_CameraManager;

        [SerializeField]
        [Tooltip("The logger used to display camera frame EXIF data.")]
        TextMeshProUGUI m_CurrentExifDataLoggerText;

        string m_BackgroundExifDataString;

        void Reset()
        {
            if (!m_CurrentExifDataLoggerText)
            {
                m_CurrentExifDataLoggerText = GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        void OnEnable()
        {
            if (m_CameraManager == null)
#if UNITY_2023_1_OR_NEWER
                m_CameraManager = FindAnyObjectByType<ARCameraManager>();
#else
                m_CameraManager = FindObjectOfType<ARCameraManager>();
#endif
            
            if (m_CameraManager != null)
            {
                m_CameraManager.frameReceived += OnCameraFrameReceived;
            }
            else
            {
                enabled = false;
            }
        }

        void OnDisable()
        {
            if (m_CameraManager != null)
            {
                m_CameraManager.frameReceived -= OnCameraFrameReceived;
            }
        }
        
        /// <summary>
        /// Logs all available EXIF data.
        /// </summary>
        void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
        {
            if (eventArgs.TryGetExifData(out XRCameraFrameExifData exifData))
            {
                m_BackgroundExifDataString = "EXIF data:\n" + exifData.ToString();
            }
            else
            {
                m_BackgroundExifDataString = "No EXIF data.";
            }

            if (m_CurrentExifDataLoggerText != null)
            {
                m_CurrentExifDataLoggerText.text = m_BackgroundExifDataString;
            }
        }
    }
}
