using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RequiresPlaneDetection : RequiresARSubsystem<XRPlaneSubsystem, XRPlaneSubsystemDescriptor>
    {
        [SerializeField]
        bool m_RequiresPlaneClassifications;

        public override bool Evaluate()
        {
            if (!base.Evaluate())
                return false;

            var descriptor = s_LoadedSubsystem.subsystemDescriptor;

            if (m_RequiresPlaneClassifications && !descriptor.supportsClassification)
                return false;

            return true;
        }
    }
}
