using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Moves the ARSessionOrigin in such a way that it makes the given content appear to be
/// at a given location acquired via a raycast.
/// </summary>
[RequireComponent(typeof(ARSessionOrigin))]
public class MakeAppearOnPlane : MonoBehaviour
{
    [SerializeField]
    [Tooltip("A transform which should be made to appear to be at the touch point.")]
    Transform m_Content;

    /// <summary>
    /// A transform which should be made to appear to be at the touch point.
    /// </summary>
    public Transform content
    {
        get { return m_Content; }
        set { m_Content = value; }
    }

    [SerializeField]
    [Tooltip("The rotation the content should appear to have.")]
    Quaternion m_Rotation;

    /// <summary>
    /// The rotation the content should appear to have.
    /// </summary>
    public Quaternion rotation
    {
        get { return m_Rotation; }
        set
        {
            m_Rotation = value;
            if (m_SessionOrigin != null)
                m_SessionOrigin.MakeContentAppearAt(content, content.transform.position, m_Rotation);
        }
    }

    void Awake()
    {
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
    }

    void Update()
    {
        if (Input.touchCount == 0 || m_Content == null)
            return;

        var touch = Input.GetTouch(0);

        if (m_SessionOrigin.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            // Raycast hits are sorted by distance, so the first one
            // will be the closest hit.
            var hitPose = s_Hits[0].pose;

            // This does not move the content; instead, it moves and orients the ARSessionOrigin
            // such that the content appears to be at the raycast hit position.
            m_SessionOrigin.MakeContentAppearAt(content, hitPose.position, m_Rotation);
        }
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARSessionOrigin m_SessionOrigin;
}
