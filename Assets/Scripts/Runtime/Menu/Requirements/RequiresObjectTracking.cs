using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresObjectTracking
        : RequiresARSubsystem<XRObjectTrackingSubsystem, XRObjectTrackingSubsystemDescriptor>
    { }
}
