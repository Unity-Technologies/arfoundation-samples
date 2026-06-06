using System;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;
#if METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
using UnityEngine.XR.OpenXR.Features.Meta;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresMetaSharedAnchors : RequiresARSubsystem<XRAnchorSubsystem, XRAnchorSubsystemDescriptor>
    {
        public override void Evaluate(List<RequirementResult> results)
        {
            base.Evaluate(results);
            if (s_LoadedSubsystem == null)
                return;

#if !(METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR))
            results.Add(new RequirementResult(false, GetType().Name));
#else
            var isSupported = s_LoadedSubsystem is MetaOpenXRAnchorSubsystem metaAnchorSubsystem &&
                metaAnchorSubsystem.isSharedAnchorsSupported == Supported.Supported;
            results.Add(new RequirementResult(isSupported, GetType().Name));
#endif
        }
    }
}
