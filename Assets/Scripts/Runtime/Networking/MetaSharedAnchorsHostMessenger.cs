using Unity.Collections;
using Unity.Netcode;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MetaSharedAnchorsHostMessenger
    {
        /// <summary>
        /// Sends a <see cref="SerializableGuid"/> to all other clients.
        /// Only the host can message other clients.
        /// </summary>
        /// <param name="groupId">The shared anchors group ID that will be sent to another client.</param>
        /// <param name="clientId">The Id of the client to send the shared anchors group ID to.</param>
        public void SendGroupIdToClient(SerializableGuid groupId, ulong clientId)
        {
            if (!NetworkManager.Singleton.IsHost)
            {
                Debug.LogError("Cannot send messages to other clients when you are not the host.");
                return;
            }

            var message = new SharedAnchorsGroupIdMessage
            {
                Id = groupId.guid
            };

            var messageSize = message.GetSizeOfMessage();
            using var writer = new FastBufferWriter(messageSize, Allocator.Temp);
            message.Serialize(writer);

            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
                MetaSharedAnchorsConstants.groupIdMessageChannelName,
                clientId,
                writer);
        }
    }
}
