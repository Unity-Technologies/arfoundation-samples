using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresParticipants : RequiresARSubsystem<XRParticipantSubsystem, XRParticipantSubsystemDescriptor>
    { }
}
