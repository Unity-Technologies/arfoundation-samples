using System;
using Unity.Netcode;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public struct SharedAnchorsGroupIdMessage : INetworkSerializeByMemcpy
    {
        static byte[] s_GuidBytes = new byte[MetaSharedAnchorsConstants.sizeOfGuidInBytes];

        public Guid Id;

        public void Serialize(FastBufferWriter writer)
        {
            writer.WriteBytesSafe(Id.ToByteArray());
        }

        public static SharedAnchorsGroupIdMessage Deserialize(FastBufferReader reader)
        {
            var message = new SharedAnchorsGroupIdMessage();
            reader.ReadBytesSafe(ref s_GuidBytes, MetaSharedAnchorsConstants.sizeOfGuidInBytes);
            message.Id = new Guid(s_GuidBytes);

            return message;
        }

        public int GetSizeOfMessage()
        {
            return MetaSharedAnchorsConstants.sizeOfGuidInBytes;
        }
    }
}
