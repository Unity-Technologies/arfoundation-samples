using System;
using TMPro;
using Unity.Netcode;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class NetworkSessionSetupUI : MonoBehaviour
    {
        [Header("Class References")]
        [SerializeField]
        ARAnchorManager m_ARAnchorManager;

        [SerializeField]
        NetworkSessionController m_NetworkSessionController;

        [SerializeField]
        NetworkStarter m_NetworkStarter;

        [SerializeField]
        ARPlaceAnchor m_ARPlaceAnchor;

        [Header("Pages")]
        [SerializeField]
        GameObject m_SessionSetupGroup;

        [SerializeField]
        GameObject m_AnchorsList;

        [SerializeField]
        GameObject m_SessionSetupFrontPage;

        [SerializeField]
        GameObject m_SessionSetupConnectingPage;

        [Header("Player Count")]
        [SerializeField]
        TextMeshProUGUI m_PlayerCountLabel;

        [Header("Notifications")]
        [SerializeField]
        GameObject m_InProgressNotification;

        [SerializeField]
        LoadingVisualizer m_LoadingVisualizer;

        [SerializeField]
        GameObject m_ErrorNotification;

        [SerializeField]
        TextMeshProUGUI m_ErrorLabel;

        [SerializeField]
        FadeAfterDuration m_ErrorNotificationFadeAfterDuration;

        bool m_DidConnect;

        void Reset()
        {
            m_NetworkSessionController = FindAnyObjectByType<NetworkSessionController>();
            m_ARPlaceAnchor = FindAnyObjectByType<ARPlaceAnchor>();
        }

        void Start()
        {

            if (m_ARAnchorManager == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_ARAnchorManager)} is null."), this);

            if (m_NetworkSessionController == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_NetworkSessionController)} is null."), this);

            if (m_NetworkStarter == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_NetworkStarter)} is null."), this);

            if (m_ARPlaceAnchor == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_ARPlaceAnchor)} is null."), this);

            m_NetworkStarter.startNetworkRequested.AddListener(OnStartNetworkRequested);
            m_NetworkStarter.networkFailedToStart.AddListener(OnNetworkFailedToStart);

            NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvent;

            m_ErrorNotificationFadeAfterDuration.FadeComplete.AddListener(ShowSessionSetupFrontPage);
        }

        void OnDestroy()
        {
            if (m_NetworkStarter != null)
            {
                m_NetworkStarter.startNetworkRequested.AddListener(OnStartNetworkRequested);
                m_NetworkStarter.networkFailedToStart.AddListener(OnNetworkFailedToStart);
            }

            if (NetworkManager.Singleton != null)
                NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvent;

            if (m_ErrorNotification != null)
                m_ErrorNotificationFadeAfterDuration.FadeComplete.RemoveListener(ShowSessionSetupFrontPage);
        }

        void OnStartNetworkRequested()
        {
            ShowSessionSetupConnectingPage();
            ShowStartingNetworkSessionInProgress();
        }

        void OnNetworkFailedToStart()
        {
            ShowDisconnectionReason("Failed to start the session.");
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
                case ConnectionEvent.PeerConnected:
                case ConnectionEvent.PeerDisconnected:
                    OnPeerConnectionEvent(networkManager);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void OnClientConnected(NetworkManager networkManager, ulong clientId)
        {
            if (clientId != networkManager.LocalClientId)
                return;

            m_PlayerCountLabel.text = networkManager.ConnectedClients.Count.ToString();
            m_DidConnect = true;
            m_LoadingVisualizer.StopAnimating();
            ShowAnchorsPage();
            m_ARPlaceAnchor.enabled = true;
        }

        void OnClientDisconnected(NetworkManager networkManager, ulong clientId)
        {
            if (clientId != networkManager.LocalClientId)
                return;

            ShowSessionSetupConnectingPage();

            if (m_DidConnect)
            {
                m_DidConnect = false;
                ShowDisconnectionReason("Disconnected from the Host.");
                m_ARPlaceAnchor.enabled = false;
                return;
            }

            var disconnectReason =
                string.IsNullOrEmpty(networkManager.DisconnectReason) ?
                    "Failed to connect to the Host." :
                    networkManager.DisconnectReason;

            ShowDisconnectionReason(disconnectReason);
        }

        void OnPeerConnectionEvent(NetworkManager networkManager)
        {
            m_PlayerCountLabel.text = networkManager.ConnectedClients.Count.ToString();
        }

        void ShowSessionSetupFrontPage()
        {
            m_SessionSetupGroup.SetActive(true);
            m_AnchorsList.SetActive(false);

            m_SessionSetupFrontPage.SetActive(true);
            m_SessionSetupConnectingPage.SetActive(false);
        }

        void ShowSessionSetupConnectingPage()
        {
            m_SessionSetupGroup.SetActive(true);
            m_AnchorsList.SetActive(false);

            m_SessionSetupFrontPage.SetActive(false);
            m_SessionSetupConnectingPage.SetActive(true);
        }

        void ShowAnchorsPage()
        {
            m_SessionSetupGroup.SetActive(false);
            m_AnchorsList.SetActive(true);
        }

        void ShowStartingNetworkSessionInProgress()
        {
            m_InProgressNotification.SetActive(true);
            m_ErrorNotification.SetActive(false);
            m_LoadingVisualizer.StartAnimating();
        }

        void ShowDisconnectionReason(string errorMessage)
        {
            m_LoadingVisualizer.StopAnimating();
            m_InProgressNotification.SetActive(false);
            m_ErrorNotification.SetActive(true);
            m_ErrorLabel.text = errorMessage;
        }
    }
}
