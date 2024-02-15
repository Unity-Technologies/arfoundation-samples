#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.XR.ARKit;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
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

        public override bool Evaluate()
        {
            if (!base.Evaluate())
                return false;

#if !UNITY_IOS || UNITY_EDITOR
            return false;
#endif

#pragma warning disable 0162 // disable unreachable code warning in Editor
#if UNITY_IOS && !UNITY_EDITOR
            if (m_RequiresWorldMap && !ARKitSessionSubsystem.worldMapSupported)
                return false;

            if (m_RequiresGeoAnchors && !EnableGeoAnchors.IsSupported)
                return false;

            if (m_RequiresCollaborativeParticipants && !ARKitSessionSubsystem.supportsCollaboration)
                return false;

            if (m_RequiresCoachingOverlay && !ARKitSessionSubsystem.coachingOverlaySupported)
                return false;
#endif // UNITY_IOS && !UNITY_EDITOR

            return true;
#pragma warning restore 0162
        }
    }
}
