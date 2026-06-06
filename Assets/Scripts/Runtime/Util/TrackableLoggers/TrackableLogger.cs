using System;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public abstract class TrackableLogger<TManager, TTrackable> : MonoBehaviour
        where TManager : MonoBehaviour, ITrackablesChanged<TTrackable>
        where TTrackable : ARTrackable
    {
        [SerializeField]
        TManager m_Manager;

        void OnEnable()
        {
            m_Manager.trackablesChanged.AddListener(OnTrackablesChanged);
        }

        void OnDisable()
        {
            m_Manager.trackablesChanged.RemoveListener(OnTrackablesChanged);
        }

        void OnTrackablesChanged(ARTrackablesChangedEventArgs<TTrackable> eventArgs)
        {
            foreach (var trackable in eventArgs.added)
            {
                Debug.Log($"Trackable added:\n{trackable}", this);
            }
            foreach (var pair in eventArgs.removed)
            {
                Debug.Log($"Trackable removed:\n{pair.Key}", this);
            }
        }

        void Reset()
        {
            m_Manager = FindAnyObjectByType<TManager>();
        }
    }
}
