using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// This example demonstrates how to toggle plane detection,
    /// and also hide or show the existing planes.
    /// </summary>
    [RequireComponent(typeof(ARPlaneManager))]
    public class PlaneDetectionController : MonoBehaviour
    {
        [Tooltip("The UI Text element used to display plane detection messages.")]
        [SerializeField]
        Text m_TogglePlaneDetectionText;

        /// <summary>
        /// The UI Text element used to display plane detection messages.
        /// </summary>
        public Text togglePlaneDetectionText
        {
            get { return m_TogglePlaneDetectionText; }
            set { m_TogglePlaneDetectionText = value; }
        }

        /// <summary>
        /// Toggles plane detection and the visualization of the planes.
        /// </summary>
        public void TogglePlaneDetection()
        {
            m_ARPlaneManager.enabled = !m_ARPlaneManager.enabled;

            string planeDetectionMessage = "";
            if (m_ARPlaneManager.enabled)
            {
                planeDetectionMessage = "Disable Plane Detection and Hide Existing";
                SetAllPlanesActive(true);
            }
            else
            {
                planeDetectionMessage = "Enable Plane Detection and Show Existing";
                SetAllPlanesActive(false);
            }

            if (togglePlaneDetectionText != null)
                togglePlaneDetectionText.text = planeDetectionMessage;
        }

        /// <summary>
        /// Iterates over all the existing planes and activates
        /// or deactivates their <c>GameObject</c>s'.
        /// </summary>
        /// <param name="value">Each planes' GameObject is SetActive with this value.</param>
        void SetAllPlanesActive(bool value)
        {
            foreach (var plane in m_ARPlaneManager.trackables)
                plane.gameObject.SetActive(value);
        }

        void Awake()
        {
            m_ARPlaneManager = GetComponent<ARPlaneManager>();
        }

        ARPlaneManager m_ARPlaneManager;
    }
}