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
    
    static List<ARRaycastHit> s_Hits;
    GameObject m_SpawnedObject;
    ARSessionOrigin m_SessionOrigin;

    void Awake()
    {
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
        s_Hits = new List<ARRaycastHit>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (m_SessionOrigin.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = s_Hits[0].pose;

                if (m_SpawnedObject == null)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        m_SpawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                    }
                }
                else
                {
                    m_SpawnedObject.transform.position = hitPose.position;
                }
            }
        }
    }
}
