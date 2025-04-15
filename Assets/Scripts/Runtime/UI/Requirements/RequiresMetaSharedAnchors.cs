#if METAOPENXR_2_2_OR_NEWER && UNITY_ANDROID
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.OpenXR.Features.Meta;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RequiresMetaSharedAnchors : RequiresARSubsystem<XRAnchorSubsystem, XRAnchorSubsystemDescriptor>
    {
        public override bool Evaluate()
        {
            if (!base.Evaluate())
                return false;

            if (s_LoadedSubsystem is MetaOpenXRAnchorSubsystem metaAnchorSubsystem)
                return metaAnchorSubsystem.isSharedAnchorsSupported == Supported.Supported;

            return false;
        }
    }
}
#endif
