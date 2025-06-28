namespace UnityEngine.XR.ARFoundation.Samples
{
    public class AnchorAddRemoveLogger : MonoBehaviour
    {
        [SerializeField]
        ARAnchorManager m_Manager;

        void OnEnable()
        {
            m_Manager.trackablesChanged.AddListener(OnTrackablesChanged);
        }

        void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARAnchor> eventArgs)
        {
            foreach (var anchor in eventArgs.added)
            {
                Debug.Log($"Anchor added:\n{anchor}", this);
            }
            foreach (var pair in eventArgs.removed)
            {
                Debug.Log($"Anchor removed:\n{pair.Key}", this);
            }
        }

        void Reset()
        {
            m_Manager = FindAnyObjectByType<ARAnchorManager>();
        }
    }
}
