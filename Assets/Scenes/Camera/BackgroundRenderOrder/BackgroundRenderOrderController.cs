using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(Dropdown))]
    public class BackgroundRenderOrderController : MonoBehaviour
    {
        CameraBackgroundRenderingMode m_RequestedRenderingMode;

        List<string> m_RenderModeOptions;

        Dropdown m_Dropdown;

        [SerializeField]
        [Tooltip("The ARCameraManager which will produce frame events.")]
        ARCameraManager m_CameraManager;

        public Action OnValueChanged;

        /// <summary>
        /// Get or set the <c>ARCameraManager</c>.
        /// </summary>
        public ARCameraManager cameraManager
        {
            get => m_CameraManager;
            set => m_CameraManager = value;
        }

        /// <summary>
        /// Callback invoked when the dropdown value changes. This
        /// lets us change the requested render order when the user changes
        /// the selection in the UI.
        /// </summary>
        /// <param name="dropdown">The <c>Dropdown</c> which changed.</param>
        public void OnDropdownValueChanged(Dropdown dropdown)
        {
            if ((cameraManager == null) || cameraManager.subsystem is not { running: true })
            {
                return;
            }

            var renderOrderModeIndex = dropdown.value;
            m_RequestedRenderingMode = (CameraBackgroundRenderingMode)renderOrderModeIndex;
            cameraManager.requestedBackgroundRenderingMode = m_RequestedRenderingMode;

            OnValueChanged?.Invoke();
        }

        void Awake()
        {
            if (cameraManager == null)
#if UNITY_2023_1_OR_NEWER
                cameraManager = FindAnyObjectByType<ARCameraManager>();
#else
                cameraManager = FindObjectOfType<ARCameraManager>();
#endif

            m_Dropdown = GetComponent<Dropdown>();
            m_Dropdown.ClearOptions();
            m_Dropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(m_Dropdown); });
            m_RenderModeOptions = new List<string>();
        }

        void OnEnable()
        {
            if (cameraManager == null)
#if UNITY_2023_1_OR_NEWER
                cameraManager = FindAnyObjectByType<ARCameraManager>();
#else
                cameraManager = FindObjectOfType<ARCameraManager>();
#endif

            if (cameraManager != null)
            {
                PopulateDropdown();
            }
            else
            {
                enabled = false;
            }
        }

        void PopulateDropdown()
        {
            m_RenderModeOptions = new List<string>(Enum.GetNames(typeof(CameraBackgroundRenderingMode)));
            m_Dropdown.AddOptions(m_RenderModeOptions);
            m_Dropdown.value = 0;
            for (var i = 0; i < m_RenderModeOptions.Count; i++)
            {
                if (m_RenderModeOptions[i] == ((CameraBackgroundRenderingMode)i).ToString())
                {
                    m_Dropdown.value = i;
                    break;
                }
            }
        }

        void Update()
        {
            if (m_RenderModeOptions.Count == 0)
                PopulateDropdown();
        }
    }
}
