using System;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
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

        public override void Evaluate(List<RequirementResult> results)
        {
            base.Evaluate(results);
            if (s_LoadedSubsystem == null)
                return;

            var descriptor = s_LoadedSubsystem.subsystemDescriptor;

            if (m_RequiresSaveAnchor)
                results.Add(new RequirementResult(
                    descriptor.supportsSaveAnchor,
                    "Save Anchor",
                    k_SubRequirementRemediationText));

            if (m_RequiresLoadAnchor)
                results.Add(new RequirementResult(
                    descriptor.supportsLoadAnchor,
                    "Load Anchor",
                    k_SubRequirementRemediationText));

            if (m_RequiresEraseAnchor)
                results.Add(new RequirementResult(
                    descriptor.supportsEraseAnchor,
                    "Erase Anchor",
                    k_SubRequirementRemediationText));

            if (m_RequiresGetSavedAnchorIds)
                results.Add(new RequirementResult(
                    descriptor.supportsGetSavedAnchorIds,
                    "Get Saved Anchor IDs",
                    k_SubRequirementRemediationText));

            if (m_RequiresAsyncCancellation)
                results.Add(new RequirementResult(
                    descriptor.supportsAsyncCancellation,
                    "Async Cancellation",
                    k_SubRequirementRemediationText));
        }
    }
}
