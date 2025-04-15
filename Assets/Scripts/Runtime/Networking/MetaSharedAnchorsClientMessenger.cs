using Unity.Collections;
using Unity.Netcode;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MetaSharedAnchorsClientMessenger
    {
        /// <summary>
        /// Sends a message to the host to respond with the shared anchors group ID.
        /// This is useful when clients first join a session and need to set the shared anchors group ID.
        /// </summary>
        public void RequestGroupIdFromHost()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                Debug.LogError("Trying to send a message to the host as the host.");
                return;
            }

            using var writer = new FastBufferWriter(0, Allocator.Temp);
            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
                MetaSharedAnchorsConstants.requestGroupIdChannelName,
                NetworkManager.ServerClientId,
                writer);
        }
    }
}
