using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresEnvironmentProbes
        : RequiresARSubsystem<XREnvironmentProbeSubsystem, XREnvironmentProbeSubsystemDescriptor>
    { }
}
