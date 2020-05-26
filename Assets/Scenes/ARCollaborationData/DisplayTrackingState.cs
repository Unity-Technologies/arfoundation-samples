using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// When relocalizing with ARCollaborationData or ARWorldMaps, the tracking state
    /// should change to TrackingState.Limited until the device has successfully
    /// relocalized to the new data. If it remains TrackingState.Tracking, then
    /// it is not working.
    /// </summary>
    [RequireComponent(typeof(ARSession))]
    public class DisplayTrackingState : MonoBehaviour
    {
        [SerializeField]
        Text m_Text;

        public Text text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }

        ARSession m_Session;

        void Start()
        {
            m_Session = GetComponent<ARSession>();
        }

        void Update()
        {
            if (text != null)
            {
                text.text = $"Session ID = {m_Session.subsystem.sessionId}\n" +
                            $"Session state = {ARSession.state.ToString()}\n" +
                            $"Tracking state = {m_Session.subsystem.trackingState}";
            }
        }
    }
}