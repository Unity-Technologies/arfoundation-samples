using System;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresOcclusion : RequiresARSubsystem<XROcclusionSubsystem, XROcclusionSubsystemDescriptor>
    {
        [SerializeField]
        bool m_RequiresDepth;

        public override void Evaluate(List<RequirementResult> results)
        {
            base.Evaluate(results);
            if (s_LoadedSubsystem == null)
                return;

            if (m_RequiresDepth)
            {
                var descriptor = s_LoadedSubsystem.subsystemDescriptor;
                var depthSupported =
                    descriptor.environmentDepthImageSupported != Supported.Unsupported ||
                    descriptor.humanSegmentationDepthImageSupported != Supported.Unsupported ||
                    descriptor.humanSegmentationStencilImageSupported != Supported.Unsupported;
                results.Add(new RequirementResult(depthSupported, nameof(m_RequiresDepth)));
            }
        }
    }
}
