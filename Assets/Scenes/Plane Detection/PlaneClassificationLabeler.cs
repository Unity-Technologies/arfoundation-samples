using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Manages the label and plane material color for each recognized plane based on
    /// the PlaneClassification enumeration defined in ARSubsystems.
    /// </summary>

    [RequireComponent(typeof(ARPlane))]
    [RequireComponent(typeof(MeshRenderer))]
    public class PlaneClassificationLabeler : MonoBehaviour
    {
        ARPlane m_ARPlane;
        MeshRenderer m_PlaneMeshRenderer;
        TextMesh m_TextMesh;
        GameObject m_TextObj;
        Vector3 m_TextFlipVec = new Vector3(0, 180, 0);

        void Awake()
        {
            m_ARPlane = GetComponent<ARPlane>();
            m_PlaneMeshRenderer = GetComponent<MeshRenderer>();

            // Setup label
            m_TextObj = new GameObject();
            m_TextMesh = m_TextObj.AddComponent<TextMesh>();
            m_TextMesh.characterSize = 0.05f;
            m_TextMesh.color = Color.black;
        }

        void Update()
        {
            UpdateLabel();
            UpdatePlaneColor();
        }

        void UpdateLabel()
        {
            // Update text
            m_TextMesh.text = m_ARPlane.classification.ToString();

            // Update Pose
            m_TextObj.transform.position = m_ARPlane.center;
            m_TextObj.transform.LookAt(Camera.main.transform);
            m_TextObj.transform.Rotate(m_TextFlipVec);
        }

        void UpdatePlaneColor()
        {
            Color planeMatColor = Color.cyan;

            switch (m_ARPlane.classification)
            {
                case PlaneClassification.None:
                    planeMatColor = Color.cyan;
                    break;
                case PlaneClassification.Wall:
                    planeMatColor = Color.white;
                    break;
                case PlaneClassification.Floor:
                    planeMatColor = Color.green;
                    break;
                case PlaneClassification.Ceiling:
                    planeMatColor = Color.blue;
                    break;
                case PlaneClassification.Table:
                    planeMatColor = Color.yellow;
                    break;
                case PlaneClassification.Seat:
                    planeMatColor = Color.magenta;
                    break;
                case PlaneClassification.Door:
                    planeMatColor = Color.red;
                    break;
                case PlaneClassification.Window:
                    planeMatColor = Color.clear;
                    break;
            }

            planeMatColor.a = 0.33f;
            m_PlaneMeshRenderer.material.color = planeMatColor;
        }

        void OnDestroy()
        {
            Destroy(m_TextObj);
        }
    }
}