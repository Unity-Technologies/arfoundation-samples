using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RequiresBodyTracking : RequiresARSubsystem<XRHumanBodySubsystem, XRHumanBodySubsystemDescriptor>
    {
        [SerializeField]
        bool m_Requires2DTracking;

        [SerializeField]
        bool m_Requires3DTracking;

        public override bool Evaluate()
        {
            if (!base.Evaluate())
                return false;

            var descriptor = s_LoadedSubsystem.subsystemDescriptor;

            if (m_Requires2DTracking && !descriptor.supportsHumanBody2D)
                return false;

            if (m_Requires3DTracking && !descriptor.supportsHumanBody3D)
                return false;

            return true;
        }
    }
}
