using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARSessionOrigin))]
public class PlaceOnPlane : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_PlacedPrefab;

    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject placedPrefab
    {
        get { return m_PlacedPrefab; }
        set { m_PlacedPrefab = value; }
    }

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject placedObject { get; private set; }

    void Awake()
    {
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
    }

    void Update()
    {
        if (Input.touchCount == 0)
            return;

        var touch = Input.GetTouch(0);

        var hits = s_RaycastHits;
        
        if (!m_SessionOrigin.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
            return;

        placementHit = hits[0];
    }

    ARSessionOrigin m_SessionOrigin;

    ARRaycastHit m_PlacementHit;

    ARRaycastHit placementHit
    {
        get { return m_PlacementHit; }
        set
        {
            m_PlacementHit = value;

            if (placedObject == null && m_PlacedPrefab != null)
            {
                placedObject = Instantiate(m_PlacedPrefab);
            }

            if (placedObject != null)
            {
                var pose = m_PlacementHit.pose;
                placedObject.transform.position = pose.position;
                placedObject.transform.rotation = pose.rotation;
            }
        }
    }

    List<ARRaycastHit> s_RaycastHits = new List<ARRaycastHit>();
}
