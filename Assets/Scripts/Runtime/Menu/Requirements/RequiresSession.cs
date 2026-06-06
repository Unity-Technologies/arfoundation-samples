using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresSession : RequiresARSubsystem<XRSessionSubsystem, XRSessionSubsystemDescriptor>
    { }
}
