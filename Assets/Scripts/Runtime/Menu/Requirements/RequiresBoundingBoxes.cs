using System;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresBoundingBoxes : RequiresARSubsystem<XRBoundingBoxSubsystem, XRBoundingBoxSubsystemDescriptor>
    {
        [SerializeField]
        bool m_RequiresBoundingBoxClassifications;

        public override void Evaluate(List<RequirementResult> results)
        {
            base.Evaluate(results);
            if (s_LoadedSubsystem == null)
                return;

            if (m_RequiresBoundingBoxClassifications)
                results.Add(new RequirementResult(
                    s_LoadedSubsystem.subsystemDescriptor.supportsClassifications,
                    nameof(m_RequiresBoundingBoxClassifications)));
        }
    }
}
