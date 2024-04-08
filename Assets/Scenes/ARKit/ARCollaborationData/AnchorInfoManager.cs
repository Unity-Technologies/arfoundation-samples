using UnityEngine;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Displays information about each reference point including
    /// whether or not the reference point is local or remote.
    /// The reference point prefab is assumed to include a GameObject
    /// which can be colored to indicate which session created it.
    /// </summary>
    [RequireComponent(typeof(XROrigin))]
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
            GetComponent<ARAnchorManager>().trackablesChanged.AddListener(OnAnchorsChanged);
        }

        void OnDisable()
        {
            GetComponent<ARAnchorManager>().trackablesChanged.RemoveListener(OnAnchorsChanged);
        }

        void OnAnchorsChanged(ARTrackablesChangedEventArgs<ARAnchor> eventArgs)
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

        void UpdateAnchor(ARAnchor anchor)
        {
            var arAnchorDebugVisualizer = anchor.GetComponent<ARAnchorDebugVisualizer>();
            if (arAnchorDebugVisualizer != null)
            {
                arAnchorDebugVisualizer.CurrentSubsystemSessionId = session.subsystem.sessionId;
            }
        }
    }
}
