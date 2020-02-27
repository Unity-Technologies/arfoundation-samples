using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// On ARKit, HDR light estimation only works in face tracking mode.
/// On ARCore, HDR light estimation only works when NOT in face tracking mode.
/// This script enables face tracking on iOS and disables it otherwise.
/// </summary>
[RequireComponent(typeof(ARSessionOrigin))]
[RequireComponent(typeof(ARFaceManager))]
public class PlatformSelector : MonoBehaviour
{
    [SerializeField]
    GameObject m_WorldSpaceObject;

    public GameObject worldSpaceObject
    {
        get => m_WorldSpaceObject;
        set => m_WorldSpaceObject = value;
    }

#if UNITY_IOS
    const bool k_PlatformIsIOS = true;
#else
    const bool k_PlatformIsIOS = false;
#endif

    void OnEnable()
    {
        GetComponent<ARFaceManager>().enabled = k_PlatformIsIOS;
        if (!k_PlatformIsIOS)
        {
            worldSpaceObject?.SetActive(true);
            Application.onBeforeRender += OnBeforeRender;
        }
    }

    void OnDisable()
    {
        GetComponent<ARFaceManager>().enabled = false;
        Application.onBeforeRender -= OnBeforeRender;
    }

    void OnBeforeRender()
    {
        var camera = GetComponent<ARSessionOrigin>().camera;
        if (camera && worldSpaceObject)
        {
            worldSpaceObject.transform.position = camera.transform.position + camera.transform.forward;
        }
    }
}
