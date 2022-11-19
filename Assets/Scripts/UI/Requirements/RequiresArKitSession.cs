using System.Collections;
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

        protected override IEnumerator Start()
        {
            yield return base.Start();

            if (m_Button.interactable == false)
                yield break;

#if !UNITY_IOS || UNITY_EDITOR
            ARSceneSelectUI.DisableButton(m_Button);
            yield break;
#endif

#if UNITY_IOS && !UNITY_EDITOR
            // disable unreachable code warning in Editor
#pragma warning disable 0162
            if (m_RequiresWorldMap && !ARKitSessionSubsystem.worldMapSupported)
            {
                ARSceneSelectUI.DisableButton(m_Button);
                yield break;
            }

            if (m_RequiresGeoAnchors && !EnableGeoAnchors.IsSupported)
            {
                ARSceneSelectUI.DisableButton(m_Button);
                yield break;
            }

            if (m_RequiresCollaborativeParticipants && !ARKitSessionSubsystem.supportsCollaboration)
            {
                ARSceneSelectUI.DisableButton(m_Button);
                yield break;
            }

            if (m_RequiresCoachingOverlay && !ARKitSessionSubsystem.coachingOverlaySupported)
                ARSceneSelectUI.DisableButton(m_Button);
#pragma warning restore 0162

#endif
        }
    }
}
