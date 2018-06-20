using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARSessionOrigin))]
public class PlaceOnPlane : MonoBehaviour
{
    [SerializeField] [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    public GameObject placedPrefab;

    List<ARRaycastHit> m_Hits;
    GameObject m_SpawnedObject;
    ARSessionOrigin m_SessionOrigin;

    void Awake()
    {
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
        m_Hits = new List<ARRaycastHit>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (m_SessionOrigin.Raycast(touch.position, m_Hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = m_Hits[0].pose;

                if (m_SpawnedObject == null)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        m_SpawnedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
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
