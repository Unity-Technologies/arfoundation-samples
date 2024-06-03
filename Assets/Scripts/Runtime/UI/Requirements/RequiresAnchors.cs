using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RequiresAnchors : RequiresARSubsystem<XRAnchorSubsystem, XRAnchorSubsystemDescriptor>
    {
        [SerializeField]
        bool m_RequiresSaveAnchor;

        [SerializeField]
        bool m_RequiresLoadAnchor;

        [SerializeField]
        bool m_RequiresEraseAnchor;

        [SerializeField]
        bool m_RequiresGetSavedAnchorIds;

        [SerializeField]
        bool m_RequiresAsyncCancellation;

        public override bool Evaluate()
        {
            if (!base.Evaluate())
                return false;

            var descriptor = s_LoadedSubsystem.subsystemDescriptor;

            if (m_RequiresSaveAnchor && !descriptor.supportsSaveAnchor)
                return false;

            if (m_RequiresLoadAnchor && !descriptor.supportsLoadAnchor)
                return false;

            if (m_RequiresEraseAnchor && !descriptor.supportsEraseAnchor)
                return false;

            if (m_RequiresGetSavedAnchorIds && !descriptor.supportsGetSavedAnchorIds)
                return false;

            if (m_RequiresAsyncCancellation && !descriptor.supportsAsyncCancellation)
                return false;

            return true;
        }
    }
}
