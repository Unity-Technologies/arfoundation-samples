using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARSessionOrigin))]
public class PlaceOnPlane : MonoBehaviour
{
    [SerializeField] [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    public GameObject PlacedPrefab;

    private List<ARRaycastHit> hits;
    private bool spawnedPrefab = false;
    private GameObject spawnedObject;
    private ARSessionOrigin sessionOrigin;

    void Awake()
    {
        sessionOrigin = GetComponent<ARSessionOrigin>();
        hits = new List<ARRaycastHit>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (!spawnedPrefab)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    if (sessionOrigin.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                    {
                        Pose hitPose = hits[0].pose;
                       
                        spawnedObject = Instantiate(PlacedPrefab, hitPose.position, hitPose.rotation);
                        spawnedPrefab = true;                    
                    }
                }
            }
            else
            {
                if (sessionOrigin.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;
                    spawnedObject.transform.position = hitPose.position;                 
                }
            }
        }
    }
}
