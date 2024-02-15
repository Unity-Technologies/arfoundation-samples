using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RequiresFaceTracking : RequiresARSubsystem<XRFaceSubsystem, XRFaceSubsystemDescriptor>
    {
        [SerializeField]
        bool m_RequiresEyeTracking;

        public override bool Evaluate()
        {
            if (!base.Evaluate())
                return false;

            if (m_RequiresEyeTracking && !s_LoadedSubsystem.subsystemDescriptor.supportsEyeTracking)
                return false;

            return true;
        }
    }
}
