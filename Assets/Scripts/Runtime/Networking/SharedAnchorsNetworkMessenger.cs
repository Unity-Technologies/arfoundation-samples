using System;
using Unity.Netcode;
using UnityEngine.Events;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Handles messaging the shared anchors groupId between the host and clients.
    /// </summary>
    public class SharedAnchorsNetworkMessenger : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField, Tooltip("The event raised when a client requests the groupId from the host. " +
             "This event passes the client's Id making the request as the parameter.")]
        UnityEvent<ulong> m_GroupIdRequested = new();
        public UnityEvent<ulong> groupIdRequested => m_GroupIdRequested;

        [SerializeField, Tooltip("The event raised when a client receives the group Id from the host.")]
        UnityEvent<SharedAnchorsGroupIdMessage> m_GroupIdReceived = new();
        public UnityEvent<SharedAnchorsGroupIdMessage> groupIdReceived => m_GroupIdReceived;

        MetaSharedAnchorsHostMessenger m_HostMessenger = new();
        MetaSharedAnchorsClientMessenger m_ClientMessenger = new();

        /// <summary>
        /// Sends a message to the host to respond with the shared anchors group ID.
        /// This is useful when clients first join a session and need to set the shared anchors group ID for sharing anchors.
        /// </summary>
        public void RequestGroupIdFromHost()
        {
            m_ClientMessenger.RequestGroupIdFromHost();
        }

        /// <summary>
        /// Sends a <see cref="SerializableGuid"/> to all other clients. Only the host can message other clients.
        /// </summary>
        /// <param name="groupId">The shared anchors group ID that will be sent to another client.</param>
        /// <param name="targetClientId">The Id of the client to send the shared anchors group ID to.</param>
        public void SendGroupIdToClient(SerializableGuid groupId, ulong targetClientId)
        {
            m_HostMessenger.SendGroupIdToClient(groupId, targetClientId);
        }

        void Start()
        {
            NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvent;
        }

        void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvent;
            }
        }

        void OnConnectionEvent(NetworkManager networkManager, ConnectionEventData connectionEventData)
        {
            switch (connectionEventData.EventType)
            {
                case ConnectionEvent.ClientConnected:
                    OnClientConnected(networkManager, connectionEventData.ClientId);
                    break;
                case ConnectionEvent.ClientDisconnected:
                    OnClientDisconnected(networkManager, connectionEventData.ClientId);
                    break;
                case ConnectionEvent.PeerDisconnected:
                case ConnectionEvent.PeerConnected:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void OnClientConnected(NetworkManager networkManager, ulong clientId)
        {
            if (clientId != networkManager.LocalClientId)
                return;

            if (networkManager.IsHost)
            {
                networkManager.CustomMessagingManager.RegisterNamedMessageHandler(
                    MetaSharedAnchorsConstants.requestGroupIdChannelName,
                    OnGroupIdRequested);
            }

            networkManager.CustomMessagingManager.RegisterNamedMessageHandler(
                MetaSharedAnchorsConstants.groupIdMessageChannelName,
                OnGroupIdReceived);
        }

        void OnClientDisconnected(NetworkManager networkManager, ulong clientId)
        {
            if (networkManager == null || networkManager.CustomMessagingManager == null)
                return;

            if (clientId != networkManager.LocalClientId)
                return;

            if (networkManager.IsHost)
            {
                networkManager.CustomMessagingManager.UnregisterNamedMessageHandler(
                    MetaSharedAnchorsConstants.requestGroupIdChannelName);
            }

            networkManager.CustomMessagingManager.UnregisterNamedMessageHandler(
                MetaSharedAnchorsConstants.groupIdMessageChannelName);
        }

        void OnGroupIdRequested(ulong senderClientId, FastBufferReader reader)
        {
            m_GroupIdRequested?.Invoke(senderClientId);
        }

        void OnGroupIdReceived(ulong senderClientId, FastBufferReader reader)
        {
            var message = SharedAnchorsGroupIdMessage.Deserialize(reader);
            m_GroupIdReceived?.Invoke(message);
        }
    }
}
