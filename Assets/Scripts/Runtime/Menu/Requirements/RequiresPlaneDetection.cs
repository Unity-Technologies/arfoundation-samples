using System;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresPlaneDetection : RequiresARSubsystem<XRPlaneSubsystem, XRPlaneSubsystemDescriptor>
    {
        [SerializeField]
        bool m_RequiresPlaneClassifications;

        public override void Evaluate(List<RequirementResult> results)
        {
            base.Evaluate(results);
            if (s_LoadedSubsystem == null)
                return;

            if (m_RequiresPlaneClassifications)
                results.Add(new RequirementResult(
                    s_LoadedSubsystem.subsystemDescriptor.supportsClassification,
                    nameof(m_RequiresPlaneClassifications)));
        }
    }
}
