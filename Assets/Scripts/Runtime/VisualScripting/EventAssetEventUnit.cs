#if VISUALSCRIPTING_1_8_OR_NEWER
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.Samples.VisualScripting
{
    /// <summary>
    /// Custom <see cref="EventUnit{TArgs}"/> that triggers when the input <see cref="EventAsset"/> is raised.
    /// </summary>
    [UnitTitle(k_UnitTitle)]
    [UnitCategory("Events/AR Foundation/Samples")]
    public sealed class EventAssetEventUnit : EventUnit<EventArgs>
    {
        const string k_UnitTitle = "On EventAsset Raised";

        /// <summary>
        /// Set this to true if this unit will listen for its event on the visual scripting event bus.
        /// </summary>
        /// <remarks>
        /// This unit directly subscribes to the input <c>EventAsset</c> and does not need to listen for anything
        /// from the event bus.
        /// </remarks>
        protected override bool register => false;

        Dictionary<GraphReference, EventHandler> m_DelegateClosures = new();

        /// <summary>
        /// The input <see cref="EventAsset"/>.
        /// </summary>
        [DoNotSerialize]
        public ValueInput eventAsset { get; private set; }

        /// <summary>
        /// Defines the unit.
        /// </summary>
        protected override void Definition()
        {
            base.Definition();
            eventAsset = ValueInput<EventAsset>(nameof(eventAsset));
        }

        /// <summary>
        /// Fired when this unit should start listening for its event.
        /// </summary>
        /// <param name="stack">The <c>GraphStack</c>.</param>
        public override void StartListening(GraphStack stack)
        {
            base.StartListening(stack);

            var graphReference = stack.AsReference();
            var asset = GetEventAsset(graphReference);
            if (asset == null)
                return;

            void OnEventRaised(object sender, EventArgs args) { Trigger(graphReference); }
            asset.eventRaised += OnEventRaised;
            m_DelegateClosures.Add(graphReference, OnEventRaised);
        }

        EventAsset GetEventAsset(GraphReference graphReference)
        {
            if (!eventAsset.hasAnyConnection)
            {
                Debug.LogError($"{k_UnitTitle} node has no input connection. Make sure there is a non-null {nameof(EventAsset)}.");
                return null;
            }
        
            var flow = Flow.New(graphReference);
            var asset = flow.GetValue(eventAsset) as EventAsset;
            if (asset == null)
                Debug.LogError($"{k_UnitTitle} node has an invalid input connection. Make sure there is a non-null {nameof(EventAsset)}");

            flow.Dispose();
            return asset;
        }

        void Trigger(GraphReference graphReference)
        {
            var flow = Flow.New(graphReference);
            flow.Run(trigger);
        }

        /// <summary>
        /// Fired when this unit should stop listening for its event.
        /// </summary>
        /// <param name="stack">The <c>GraphStack</c>.</param>
        public override void StopListening(GraphStack stack)
        {
            base.StopListening(stack);

            var graphReference = stack.AsReference();
            if (!m_DelegateClosures.ContainsKey(graphReference))
                return;

            var asset = GetEventAsset(graphReference);
            var action = m_DelegateClosures[graphReference];
            asset.eventRaised -= action;
            m_DelegateClosures.Remove(graphReference);
        }
    }
}
#endif // VISUALSCRIPTING_1_8_OR_NEWER
