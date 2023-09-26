using System.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RequiresImageStabilization : RequiresARSubsystem<XRCameraSubsystem, XRCameraSubsystemDescriptor>
    {
        protected override IEnumerator Start()
        {
            yield return base.Start();

            if (m_Button.interactable == false)
                yield break;

            var descriptor = s_LoadedSubsystem.subsystemDescriptor;

            if (descriptor.supportsImageStabilization == Supported.Unsupported)
            {
                ARSceneSelectUI.DisableButton(m_Button);
            }
        }
    }
}
