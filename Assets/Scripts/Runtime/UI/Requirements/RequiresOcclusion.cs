using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RequiresOcclusion : RequiresARSubsystem<XROcclusionSubsystem, XROcclusionSubsystemDescriptor>
    {
        [SerializeField]
        bool m_RequiresDepth;

        public override bool Evaluate()
        {
            if (!base.Evaluate())
                return false;

            var descriptor = s_LoadedSubsystem.subsystemDescriptor;

            if (m_RequiresDepth && 
                descriptor.environmentDepthImageSupported == Supported.Unsupported &&
                descriptor.humanSegmentationDepthImageSupported == Supported.Unsupported &&
                descriptor.humanSegmentationStencilImageSupported == Supported.Unsupported)
            {
                return false;
            }

            return true;
        }
    }
}
