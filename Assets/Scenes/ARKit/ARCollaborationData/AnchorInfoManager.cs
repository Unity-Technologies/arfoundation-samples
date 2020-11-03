using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Displays information about each reference point including
    /// whether or not the reference point is local or remote.
    /// The reference point prefab is assumed to include a GameObject
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
            get => m_Session;
            set => m_Session = value;
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
            var sessionId = anchor.sessionId;

            var textManager = anchor.GetComponent<CanvasTextManager>();
            if (textManager)
            {
                textManager.text = sessionId.Equals(session.subsystem.sessionId) ? "Local" : "Remote";
            }

            var colorizer = anchor.GetComponent<Colorizer>();
            if (colorizer)
            {
                // Generate a color from the sessionId
                unsafe
                {
                    var bytes = *(byte128*)&sessionId;
                    colorizer.color = new Color(
                        bytes.data[0] / 255f,
                        bytes.data[4] / 255f,
                        bytes.data[8] / 255f,
                        bytes.data[12] / 255f);
                }
            }
        }
    }
}
