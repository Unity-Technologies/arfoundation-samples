using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

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
            get => m_Text;
            set => m_Text = value;
        }

        readonly StringBuilder m_StringBuilder = new();

        ARSession m_Session;
        Guid m_SessionId;
        ARSessionState m_SessionState;
        TrackingState m_TrackingState;

        void Start()
        {
            m_Session = GetComponent<ARSession>();
        }

        void Update()
        {
            if (text == null)
                return;

            var sessionId = m_Session.subsystem.sessionId;
            var sessionState = ARSession.state;
            var trackingState = m_Session.subsystem.trackingState;

            if (sessionId == m_SessionId && sessionState == m_SessionState && trackingState == m_TrackingState)
                return;

            m_SessionId = sessionId;
            m_SessionState = sessionState;
            m_TrackingState = trackingState;

            m_StringBuilder.Clear();
            m_StringBuilder.Append("Session ID = ").Append(sessionId.ToString()).Append('\n');
            m_StringBuilder.Append("Session state = ").Append(sessionState.ToString()).Append('\n');
            m_StringBuilder.Append("Tracking state = ").Append(trackingState.ToString());
            text.text = m_StringBuilder.ToString();
        }
    }
}