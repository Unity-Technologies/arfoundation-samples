using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ToggleManagers : MonoBehaviour
{
    [SerializeField]
    public bool m_PlaneManager;

    public bool planeManager
    {
        get { return m_PlaneManager; }
        set { m_PlaneManager = value; }
    }

    [SerializeField]
    public bool m_FaceManager;

    public bool faceManager
    {
        get { return m_FaceManager; }
        set { m_FaceManager = value; }
    }

    void ToggleComponent<T>() where T : MonoBehaviour
    {
        var behaviour = GetComponent<T>();
        if (behaviour == null)
            return;

        behaviour.enabled = !behaviour.enabled;
        var enabledText = behaviour.enabled ? "enabled" : "disabled";
        Logger.Log($"{typeof(T).Name} {enabledText}");
    }

    void Update()
    {
        if (Input.touchCount < 1)
            return;

        var touch = Input.touches[0];
        if (touch.phase == TouchPhase.Began)
        {
            if (planeManager)
                ToggleComponent<ARPlaneManager>();

            if (faceManager)
                ToggleComponent<ARFaceManager>();
        }
    }
}
