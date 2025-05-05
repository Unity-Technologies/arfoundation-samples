using UnityEngine.XR.ARSubsystems;
#if METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
using UnityEngine.XR.OpenXR.Features.Meta;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RequiresMetaSharedAnchors : RequiresARSubsystem<XRAnchorSubsystem, XRAnchorSubsystemDescriptor>
    {
        public override bool Evaluate()
        {
            if (!base.Evaluate())
                return false;

#if !(METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR))
            return false;
#else
            if (s_LoadedSubsystem is MetaOpenXRAnchorSubsystem metaAnchorSubsystem)
                return metaAnchorSubsystem.isSharedAnchorsSupported == Supported.Supported;

            return false;
#endif
        }
    }
}
