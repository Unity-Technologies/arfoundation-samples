using System;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresBodyTracking : RequiresARSubsystem<XRHumanBodySubsystem, XRHumanBodySubsystemDescriptor>
    {
        [SerializeField]
        bool m_Requires2DTracking;

        [SerializeField]
        bool m_Requires3DTracking;

        public override void Evaluate(List<RequirementResult> results)
        {
            base.Evaluate(results);
            if (s_LoadedSubsystem == null)
                return;

            var descriptor = s_LoadedSubsystem.subsystemDescriptor;

            if (m_Requires2DTracking)
                results.Add(new RequirementResult(descriptor.supportsHumanBody2D, nameof(m_Requires2DTracking)));

            if (m_Requires3DTracking)
                results.Add(new RequirementResult(descriptor.supportsHumanBody3D, nameof(m_Requires3DTracking)));
        }
    }
}
