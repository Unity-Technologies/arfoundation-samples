using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaneClassification : MonoBehaviour
{
    ARPlaneManager m_ARPlaneManager;

    void Awake()
    {
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
    }

    void OnEnable()
    {
        m_ARPlaneManager.planesChanged += OnPlanesAdded;
        m_ARPlaneManager.planesChanged += OnPlanesUpdated;
        m_ARPlaneManager.planesChanged += OnPlanesRemoved;
    }

    void OnDisable()
    {
        m_ARPlaneManager.planesChanged -= OnPlanesAdded;
        m_ARPlaneManager.planesChanged -= OnPlanesUpdated;
        m_ARPlaneManager.planesChanged -= OnPlanesRemoved;
    }

    void OnPlanesAdded(ARPlanesChangedEventArgs eventArgs)
    {
        foreach (var plane in eventArgs.added)
        {
            GameObject textObj = new GameObject();
            textObj.transform.SetParent(plane.gameObject.transform, false);

            TextMesh planeText = textObj.AddComponent<TextMesh>();

            if (planeText)
            {
                planeText.characterSize = 0.05f;
                planeText.text = plane.classification.ToString();
            }
        }
    }

    void OnPlanesUpdated(ARPlanesChangedEventArgs eventArgs)
    {
        foreach (var plane in eventArgs.updated)
        {
            TextMesh planeText = plane.GetComponentInChildren<TextMesh>();
            if (planeText)
            {
                planeText.text = plane.classification.ToString();
            }
        }
    }

    void OnPlanesRemoved(ARPlanesChangedEventArgs eventArgs)
    {
        foreach (var plane in eventArgs.removed)
        {
            Component[] textMeshes;

            textMeshes = plane.gameObject.GetComponentsInChildren(typeof(TextMesh));
            
            foreach (TextMesh planeText in textMeshes)
            {
                Destroy(planeText.gameObject);
            }
        }
    }
}
