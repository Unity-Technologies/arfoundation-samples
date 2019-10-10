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
        m_ARPlaneManager.planesChanged += OnPlanesChanged;
    }

    void OnDisable()
    {
        m_ARPlaneManager.planesChanged -= OnPlanesChanged;
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs eventArgs)
    {
        OnPlanesAdded(eventArgs.added);
        OnPlanesUpdated(eventArgs.updated);
        OnPlanesRemoved(eventArgs.removed);
    }

    void OnPlanesAdded(List<ARPlane> addedPlanes)
    {
        foreach (var plane in addedPlanes)
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

    void OnPlanesUpdated(List<ARPlane> updatedPlanes)
    {
        foreach (var plane in updatedPlanes)
        {
            TextMesh planeText = plane.GetComponentInChildren<TextMesh>();
            if (planeText)
            {
                planeText.text = plane.classification.ToString();
            }
        }
    }

    void OnPlanesRemoved(List<ARPlane> removedPlanes)
    {
        foreach (var plane in removedPlanes)
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
