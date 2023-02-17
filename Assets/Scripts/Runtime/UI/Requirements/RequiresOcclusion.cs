using System.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RequiresOcclusion : RequiresARSubsystem<XROcclusionSubsystem, XROcclusionSubsystemDescriptor>
    {
        [SerializeField]
        bool m_RequiresDepth;

        protected override IEnumerator Start()
        {
            yield return base.Start();

            if (m_Button.interactable == false)
                yield break;

            var descriptor = s_LoadedSubsystem.subsystemDescriptor;

            if (m_RequiresDepth && 
                descriptor.environmentDepthImageSupported == Supported.Unsupported &&
                descriptor.humanSegmentationDepthImageSupported == Supported.Unsupported &&
                descriptor.humanSegmentationStencilImageSupported == Supported.Unsupported)
            {
                ARSceneSelectUI.DisableButton(m_Button);
            }
        }
    }
}
