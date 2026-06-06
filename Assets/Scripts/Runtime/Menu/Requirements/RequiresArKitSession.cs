using System;
using System.Collections.Generic;
#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.XR.ARKit;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresArKitSession : RequiresSession
    {
        [SerializeField]
        bool m_RequiresWorldMap;

        [SerializeField]
        bool m_RequiresGeoAnchors;

        [SerializeField]
        bool m_RequiresCollaborativeParticipants;

        [SerializeField]
        bool m_RequiresCoachingOverlay;

        public override void Evaluate(List<RequirementResult> results)
        {
            base.Evaluate(results);
            if (s_LoadedSubsystem == null)
                return;

#if !UNITY_IOS || UNITY_EDITOR
            var worldMapSupported = false;
            var geoAnchorsSupported = false;
            var collaborativeParticipantsSupported = false;
            var coachingOverlaySupported = false;
#else
            var worldMapSupported = ARKitSessionSubsystem.worldMapSupported;
            var geoAnchorsSupported = EnableGeoAnchors.IsSupported;
            var collaborativeParticipantsSupported = ARKitSessionSubsystem.supportsCollaboration;
            var coachingOverlaySupported = ARKitSessionSubsystem.coachingOverlaySupported;
#endif

            if (m_RequiresWorldMap)
                results.Add(new RequirementResult(worldMapSupported, nameof(m_RequiresWorldMap)));

            if (m_RequiresGeoAnchors)
                results.Add(new RequirementResult(geoAnchorsSupported, nameof(m_RequiresGeoAnchors)));

            if (m_RequiresCollaborativeParticipants)
                results.Add(new RequirementResult(
                    collaborativeParticipantsSupported, nameof(m_RequiresCollaborativeParticipants)));

            if (m_RequiresCoachingOverlay)
                results.Add(new RequirementResult(coachingOverlaySupported, nameof(m_RequiresCoachingOverlay)));
        }
    }
}
