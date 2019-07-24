using NSData = Unity.iOS.Multipeer.NSData;

namespace UnityEngine.XR.ARKit
{
    public static class SerializedARCollaborationDataExtensions
    {
        public static unsafe NSData ToNSData(this SerializedARCollaborationData serializedData)
        {
            return *(NSData*)&serializedData;
        }

        public static unsafe SerializedARCollaborationData ToSerializedCollaborationData(this NSData data)
        {
            return *(SerializedARCollaborationData*)&data;
        }
    }
}
