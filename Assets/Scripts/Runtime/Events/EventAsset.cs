using System;
#if VISUALSCRIPTING_1_8_OR_NEWER
using Unity.VisualScripting;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
#if VISUALSCRIPTING_1_8_OR_NEWER
    [TypeIcon(typeof(EventUnit<EventArgs>))]
#endif
    [CreateAssetMenu(menuName = "XR/AR Foundation/Events/EventAsset")]
    public class EventAsset : ScriptableObject
    {
        public event EventHandler eventRaised;

        public void Raise()
        {
            eventRaised?.Invoke(this, EventArgs.Empty);
        }
    }
}
