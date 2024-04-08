using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RequiresBoundingBoxes : RequiresARSubsystem<XRBoundingBoxSubsystem, XRBoundingBoxSubsystemDescriptor>
    {
        [SerializeField]
        bool m_RequiresBoundingBoxClassifications;

        public override bool Evaluate()
        {
            if (!base.Evaluate())
                return false;

            var descriptor = s_LoadedSubsystem.subsystemDescriptor;

            if (m_RequiresBoundingBoxClassifications && !descriptor.supportsClassifications)
                return false;

            return true;
        }
    }
}
