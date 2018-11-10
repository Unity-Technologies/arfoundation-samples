using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// This example demonstrates how to toggle plane detection,
/// and also hide or show the existing planes.
/// </summary>
[RequireComponent(typeof(ARPlaneManager))]
public class PlaneDetectionController : MonoBehaviour
{
    [SerializeField]
    Text m_TogglePlaneDetectionText;

    /// <summary>
    /// Toggles plane detection and the visualization of the planes.
    /// </summary>
    public void TogglePlaneDetection()
    {
        m_ARPlaneManager.enabled = !m_ARPlaneManager.enabled;

        if (m_ARPlaneManager.enabled)
        {
            m_TogglePlaneDetectionText.text = "Disable Plane Detection and Hide Existing";
            SetAllPlanesActive(true);
        }
        else
        {
            m_TogglePlaneDetectionText.text = "Enable Plane Detection and Show Existing";
            SetAllPlanesActive(false);
        }
    }

    /// <summary>
    /// Iterates over all the existing planes and activates
    /// or deactivates their <c>GameObject</c>s'.
    /// </summary>
    /// <param name="value">Each planes' GameObject is SetActive with this value.</param>
    void SetAllPlanesActive(bool value)
    {
        m_ARPlaneManager.GetAllPlanes(s_Planes);
        foreach (var plane in s_Planes)
            plane.gameObject.SetActive(value);
    }

    void Awake()
    {
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
    }

    ARPlaneManager m_ARPlaneManager;

    static List<ARPlane> s_Planes = new List<ARPlane>();
}
