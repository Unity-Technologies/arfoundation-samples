using System;
using TMPro;
#if METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
using Unity.Netcode;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.OpenXR.Features.Meta;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Handles requesting the groupId from the host if the user is a client. handles responding to groupId requests from
    /// clients if the user is the host.
    /// </summary>
    public class MetaSharedAnchorsGroupIdSetup : MonoBehaviour
    {
        [SerializeField]
        ARAnchorManager m_AnchorManager;

        [SerializeField]
        SharedAnchorsNetworkMessenger m_SharedAnchorsNetworkMessenger;

        [SerializeField]
        TextMeshProUGUI m_GroupIdLabel;

#if METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
        MetaOpenXRAnchorSubsystem m_MetaOpenXRAnchorSubsystem;
        bool m_IsSharedAnchorsSupported;

        void Reset()
        {
            m_AnchorManager = FindAnyObjectByType<ARAnchorManager>();
            m_SharedAnchorsNetworkMessenger = FindAnyObjectByType<SharedAnchorsNetworkMessenger>();
        }

        void Start()
        {
            if (m_AnchorManager == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_AnchorManager)} is null."), this);

            if (m_SharedAnchorsNetworkMessenger == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_SharedAnchorsNetworkMessenger)} is null."), this);

            if (m_GroupIdLabel == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_GroupIdLabel)} is null."), this);

            NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvent;
        }

        void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
                NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvent;
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

            if (m_AnchorManager.subsystem is MetaOpenXRAnchorSubsystem metaOpenXRAnchorSubsystem)
                m_MetaOpenXRAnchorSubsystem = metaOpenXRAnchorSubsystem;

            if (!networkManager.IsHost)
            {
                m_SharedAnchorsNetworkMessenger.groupIdReceived.AddListener(OnGroupIdReceived);
                m_SharedAnchorsNetworkMessenger.RequestGroupIdFromHost();
                return;
            }

            m_SharedAnchorsNetworkMessenger.groupIdRequested.AddListener(OnGroupIdRequested);

            if (m_MetaOpenXRAnchorSubsystem is not null)
            {
                m_IsSharedAnchorsSupported = m_MetaOpenXRAnchorSubsystem.isSharedAnchorsSupported == Supported.Supported;
                m_MetaOpenXRAnchorSubsystem.sharedAnchorsGroupId = new SerializableGuid(Guid.NewGuid());
                m_GroupIdLabel.text = m_MetaOpenXRAnchorSubsystem.sharedAnchorsGroupId.ToString();
            }
        }

        void OnClientDisconnected(NetworkManager networkManager, ulong clientId)
        {
            if (clientId != networkManager.LocalClientId)
                return;

            if (networkManager.IsHost)
            {
                m_SharedAnchorsNetworkMessenger.groupIdRequested.RemoveListener(OnGroupIdRequested);
            }
            else
            {
                m_SharedAnchorsNetworkMessenger.groupIdReceived.RemoveListener(OnGroupIdReceived);
            }
        }

        void OnGroupIdRequested(ulong clientId)
        {
            if (!m_IsSharedAnchorsSupported)
                return;

            m_SharedAnchorsNetworkMessenger.SendGroupIdToClient(
                m_MetaOpenXRAnchorSubsystem.sharedAnchorsGroupId, clientId);
        }

        void OnGroupIdReceived(SharedAnchorsGroupIdMessage sharedAnchorsGroupIdMessage)
        {
            var groupId = new SerializableGuid(sharedAnchorsGroupIdMessage.Id);

            if (m_MetaOpenXRAnchorSubsystem is not null)
            {
                m_MetaOpenXRAnchorSubsystem.sharedAnchorsGroupId = groupId;
                m_GroupIdLabel.text = groupId.ToString();
            }

        }
#endif // METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
    }
}
