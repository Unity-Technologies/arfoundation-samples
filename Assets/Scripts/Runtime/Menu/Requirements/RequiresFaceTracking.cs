using System;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresFaceTracking : RequiresARSubsystem<XRFaceSubsystem, XRFaceSubsystemDescriptor>
    {
        [SerializeField]
        bool m_RequiresEyeTracking;

        public override void Evaluate(List<RequirementResult> results)
        {
            base.Evaluate(results);
            if (s_LoadedSubsystem == null)
                return;

            if (m_RequiresEyeTracking)
                results.Add(new RequirementResult(
                    s_LoadedSubsystem.subsystemDescriptor.supportsEyeTracking,
                    nameof(m_RequiresEyeTracking)));
        }
    }
}
