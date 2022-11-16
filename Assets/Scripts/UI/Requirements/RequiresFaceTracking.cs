using System.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RequiresFaceTracking : RequiresARSubsystem<XRFaceSubsystem, XRFaceSubsystemDescriptor>
    {
        [SerializeField]
        bool m_RequiresEyeTracking;

        protected override IEnumerator Start()
        {
            yield return base.Start();

            if (m_Button.interactable == false)
                yield break;

            if (m_RequiresEyeTracking && !s_LoadedSubsystem.subsystemDescriptor.supportsEyeTracking)
                ARSceneSelectUI.DisableButton(m_Button);
        }
    }
}
