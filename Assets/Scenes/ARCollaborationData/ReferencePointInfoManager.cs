using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Displays information about each reference point including
/// whether or not the reference point is local or remote.
/// The reference point prefab is assumed to include a GameObject
/// which can be colored to indicate which session created it.
/// </summary>
[RequireComponent(typeof(ARSessionOrigin))]
[RequireComponent(typeof(ARReferencePointManager))]
public class ReferencePointInfoManager : MonoBehaviour
{
    [SerializeField]
    ARSession m_Session;

    public ARSession session
    {
        get { return m_Session; }
        set { m_Session = value; }
    }

    void OnEnable()
    {
        GetComponent<ARReferencePointManager>().referencePointsChanged += OnReferencePointsChanged;
    }

    void OnDisable()
    {
        GetComponent<ARReferencePointManager>().referencePointsChanged -= OnReferencePointsChanged;
    }

    void OnReferencePointsChanged(ARReferencePointsChangedEventArgs eventArgs)
    {
        foreach (var referencePoint in eventArgs.added)
        {
            UpdateReferencePoint(referencePoint);
        }

        foreach (var referencePoint in eventArgs.updated)
        {
            UpdateReferencePoint(referencePoint);
        }
    }

    unsafe struct byte128
    {
        public fixed byte data[16];
    }

    void UpdateReferencePoint(ARReferencePoint referencePoint)
    {
        var canvas = referencePoint.GetComponentInChildren<Canvas>();
        if (canvas == null)
            return;

        canvas.worldCamera = GetComponent<ARSessionOrigin>().camera;

        var text = canvas.GetComponentInChildren<Text>();
        if (text == null)
            return;

        var sessionId = referencePoint.sessionId;
        if (sessionId.Equals(session.subsystem.sessionId))
        {
            text.text = $"Local";
        }
        else
        {
            text.text = $"Remote";
        }

        var cube = referencePoint.transform.Find("Scale/SessionId Indicator");
        if (cube != null)
        {
            var renderer = cube.GetComponent<Renderer>();
            {
                // Generate a color from the sessionId
                Color color;
                unsafe
                {
                    var bytes = *(byte128*)&sessionId;
                    color = new Color(
                        bytes.data[0] / 255f,
                        bytes.data[4] / 255f,
                        bytes.data[8] / 255f,
                        bytes.data[12] / 255f);
                }
                renderer.material.color = color;
            }
        }
    }
}
