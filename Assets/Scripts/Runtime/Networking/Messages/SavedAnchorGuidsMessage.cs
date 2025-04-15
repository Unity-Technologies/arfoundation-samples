using System;
using Unity.Netcode;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public struct SavedAnchorGuidsMessage : INetworkSerializeByMemcpy
    {
        static byte[] s_GuidBytes = new byte[MetaSharedAnchorsConstants.sizeOfGuidInBytes];

        public ulong originalSenderId;
        public Guid[] guids;

        public void Serialize(FastBufferWriter writer)
        {
            writer.WriteValueSafe(originalSenderId);
            writer.WriteValueSafe((ushort)guids.Length);

            for (var i = 0; i < guids.Length; i += 1)
            {
                writer.WriteBytesSafe(guids[i].ToByteArray());
            }
        }

        public static SavedAnchorGuidsMessage Deserialize(FastBufferReader reader)
        {
            var message = new SavedAnchorGuidsMessage();
            reader.ReadValueSafe(out ulong originalSenderId);
            message.originalSenderId = originalSenderId;

            reader.ReadValueSafe(out ushort length);
            message.guids = new Guid[length];

            for (var i = 0; i < length; i += 1)
            {
                reader.ReadBytesSafe(ref s_GuidBytes, MetaSharedAnchorsConstants.sizeOfGuidInBytes);
                message.guids[i] = new Guid(s_GuidBytes);
            }

            return message;
        }

        public int GetSizeOfMessage()
        {
            var messageSize = guids.Length * MetaSharedAnchorsConstants.sizeOfGuidInBytes;
            messageSize += sizeof(ulong);
            messageSize += sizeof(ushort);
            return messageSize;
        }
    }
}
