using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Displays information about each anchor including
/// whether or not the anchor is local or remote.
/// The anchor prefab is assumed to include a GameObject
/// which can be colored to indicate which session created it.
/// </summary>
[RequireComponent(typeof(ARSessionOrigin))]
[RequireComponent(typeof(ARAnchorManager))]
public class AnchorInfoManager : MonoBehaviour
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
        GetComponent<ARAnchorManager>().anchorsChanged += OnAnchorsChanged;
    }

    void OnDisable()
    {
        GetComponent<ARAnchorManager>().anchorsChanged -= OnAnchorsChanged;
    }

    void OnAnchorsChanged(ARAnchorsChangedEventArgs eventArgs)
    {
        foreach (var anchor in eventArgs.added)
        {
            UpdateAnchor(anchor);
        }

        foreach (var anchor in eventArgs.updated)
        {
            UpdateAnchor(anchor);
        }
    }

    unsafe struct byte128
    {
        public fixed byte data[16];
    }

    void UpdateAnchor(ARAnchor anchor)
    {
        var canvas = anchor.GetComponentInChildren<Canvas>();
        if (canvas == null)
            return;

        canvas.worldCamera = GetComponent<ARSessionOrigin>().camera;

        var text = canvas.GetComponentInChildren<Text>();
        if (text == null)
            return;

        var sessionId = anchor.sessionId;
        if (sessionId.Equals(session.subsystem.sessionId))
        {
            text.text = $"Local";
        }
        else
        {
            text.text = $"Remote";
        }

        var cube = anchor.transform.Find("Scale/SessionId Indicator");
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
