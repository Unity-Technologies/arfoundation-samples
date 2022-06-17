using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Manages the plane material color for each recognized plane based on
    /// the <see cref="UnityEngine.XR.ARSubsystems.TrackingState"/> enumeration defined in ARSubsystems.
    /// </summary>
    [RequireComponent(typeof(ARPlane))]
    [RequireComponent(typeof(MeshRenderer))]
    public class PlaneTrackingModeVisualizer : MonoBehaviour
    {
        ARPlane m_ARPlane;
        MeshRenderer m_PlaneMeshRenderer;
        Color m_OriginalColor;

        void Awake()
        {
            m_ARPlane = GetComponent<ARPlane>();
            m_PlaneMeshRenderer = GetComponent<MeshRenderer>();
            m_OriginalColor =  m_PlaneMeshRenderer.material.color;
        }

        void Update()
        {
            UpdatePlaneColor();
        }

        void UpdatePlaneColor()
        {

            Color planeMatColor = Color.cyan;

            switch (m_ARPlane.trackingState)
            {
                case TrackingState.None:
                    planeMatColor = Color.grey;
                    break;
                case TrackingState.Limited:
                    planeMatColor = Color.red;
                    break;
                case TrackingState.Tracking:
                    planeMatColor = m_OriginalColor;
                    break;
            }

            planeMatColor.a = m_OriginalColor.a;
            m_PlaneMeshRenderer.material.color = planeMatColor;
        }
    }
}
