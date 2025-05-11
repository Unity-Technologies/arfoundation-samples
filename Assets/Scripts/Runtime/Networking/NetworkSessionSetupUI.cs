using System;

#if METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
using TMPro;
using Unity.Netcode;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.OpenXR.Features.Meta;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class NetworkSessionSetupUI : MonoBehaviour
    {
#if METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
        const int k_ReattemptsLimit = 3;

        [Header("Class References")]
        [SerializeField]
        ARAnchorManager m_ARAnchorManager;

        [SerializeField]
        NetworkSessionController m_NetworkSessionController;

        [SerializeField]
        NetworkStarter m_NetworkStarter;

        [SerializeField]
        MetaColocationDiscovery m_MetaColocationDiscovery;

        [SerializeField]
        ColocationAdvertisementIndicator m_ColocationAdvertisementIndicator;

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
        UIToastNotification m_InProgressNotification;

        [SerializeField]
        LoadingVisualizer m_LoadingVisualizer;

        [SerializeField]
        GameObject m_ErrorNotification;

        [SerializeField]
        TextMeshProUGUI m_ErrorLabel;

        [SerializeField]
        FadeAfterDuration m_ErrorNotificationFadeAfterDuration;

        bool m_DidConnect;
        int m_AdvertisementReattemptsCount;

        public async void HostSession()
        {
            if (!m_MetaColocationDiscovery.IsColocationDiscoverySupported)
                return;

            m_ColocationAdvertisementIndicator.ShowIndicator();

            ShowSessionSetupConnectingPage();
            var result = await m_MetaColocationDiscovery.StartIPAddressAdvertisement();

            if (result.status.IsError())
            {
                ShowDisconnectionReason($"Failed to start host discovery: {result.status.ToString()}");
                return;
            }

            m_NetworkStarter.StartNetworkSession(true);
        }

        public async void JoinSession()
        {
            if (!m_MetaColocationDiscovery.IsColocationDiscoverySupported)
                return;

            m_ColocationAdvertisementIndicator.HideIndicator();

            ShowSessionSetupConnectingPage();
            var resultStatus = await m_MetaColocationDiscovery.StartDiscoveryForHostIPAddress();

            if (resultStatus.IsError())
                ShowDisconnectionReason($"Failed to start host discovery: {resultStatus.statusCode.ToString()}");
        }

        void Reset()
        {
            m_NetworkSessionController = FindAnyObjectByType<NetworkSessionController>();
            m_ARPlaceAnchor = FindAnyObjectByType<ARPlaceAnchor>();
        }

        void Awake()
        {
            if (m_ARAnchorManager == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_ARAnchorManager)} is null."), this);

            if (m_NetworkSessionController == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_NetworkSessionController)} is null."), this);

            if (m_NetworkStarter == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_NetworkStarter)} is null."), this);

            if (m_ARPlaceAnchor == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_ARPlaceAnchor)} is null."), this);
        }

        void Start()
        {
            m_NetworkStarter.startNetworkRequested.AddListener(OnStartNetworkRequested);
            m_NetworkStarter.networkFailedToStart.AddListener(OnNetworkFailedToStart);

            NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvent;

            m_ErrorNotificationFadeAfterDuration.FadeComplete.AddListener(ShowSessionSetupFrontPage);

            m_MetaColocationDiscovery.advertisementStateChanged.AddListener(OnAdvertisementStateChanged);
            m_MetaColocationDiscovery.discoveryStateChanged.AddListener(OnDiscoveryStateChanged);
            m_MetaColocationDiscovery.hostIPAddressDiscovered.AddListener(OnHostIPAddressDiscovered);
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

            if (m_MetaColocationDiscovery != null)
            {
                m_MetaColocationDiscovery.advertisementStateChanged.RemoveListener(OnAdvertisementStateChanged);
                m_MetaColocationDiscovery.discoveryStateChanged.RemoveListener(OnDiscoveryStateChanged);
                m_MetaColocationDiscovery.hostIPAddressDiscovered.RemoveListener(OnHostIPAddressDiscovered);
            }
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

            StopAdvertisementAndDiscovery();

            foreach (var anchor in m_ARAnchorManager.trackables)
            {
                m_ARAnchorManager.TryRemoveAnchor(anchor);
            }

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

        async void OnAdvertisementStateChanged(XRResultStatus resultStatus, ColocationState state)
        {
            m_ColocationAdvertisementIndicator.SetStatus(state);
            switch (state)
            {
                case ColocationState.Active:
                    m_AdvertisementReattemptsCount = 0;
                    break;
                case ColocationState.Inactive:
                    while (resultStatus.IsError() && m_AdvertisementReattemptsCount < k_ReattemptsLimit)
                    {
                        m_AdvertisementReattemptsCount += 1;
                        var result = await m_MetaColocationDiscovery.StartIPAddressAdvertisement();
                        resultStatus = result.status;
                    }
                    break;
                case ColocationState.Starting:
                case ColocationState.Stopping:
                    break;
            }
        }

        void OnDiscoveryStateChanged(XRResultStatus resultStatus, ColocationState state)
        {
            switch (state)
            {
                case ColocationState.Starting:
                    ShowSessionSetupConnectingPage();
                    ShowDiscoveringHostInProgress();
                    break;
                case ColocationState.Inactive:
                    if (resultStatus.IsError())
                        ShowDisconnectionReason($"Failed to discover the host: {resultStatus.statusCode.ToString()}");
                    break;
                case ColocationState.Active:
                case ColocationState.Stopping:
                    break;
            }
        }

        async void OnHostIPAddressDiscovered(SerializableGuid advertisementId, string ipAddress)
        {
            await m_MetaColocationDiscovery.StopDiscoveryForHostIPAddress();
            m_NetworkStarter.SetConnectionIPAddress(ipAddress);
            m_NetworkStarter.StartNetworkSession(false);
            ShowStartingNetworkSessionInProgress();
        }

        async void StopAdvertisementAndDiscovery()
        {
            if (m_MetaColocationDiscovery != null &&
                m_MetaColocationDiscovery.discoveryState is not ColocationState.Inactive)
                await m_MetaColocationDiscovery.StopDiscoveryForHostIPAddress();

            if (m_MetaColocationDiscovery != null &&
                m_MetaColocationDiscovery.advertisementState is not ColocationState.Inactive)
                await m_MetaColocationDiscovery.StopIPAddressAdvertisement();
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

        void ShowDiscoveringHostInProgress()
        {
            m_InProgressNotification.SetNotificationLabel("Searching for host...");
            m_InProgressNotification.gameObject.SetActive(true);
            m_ErrorNotification.SetActive(false);
            m_LoadingVisualizer.StartAnimating();
        }

        void ShowStartingNetworkSessionInProgress()
        {
            m_InProgressNotification.SetNotificationLabel("Connecting to host...");
            m_InProgressNotification.gameObject.SetActive(true);
            m_ErrorNotification.SetActive(false);
            m_LoadingVisualizer.StartAnimating();
        }

        void ShowDisconnectionReason(string errorMessage)
        {
            m_LoadingVisualizer.StopAnimating();
            m_InProgressNotification.gameObject.SetActive(false);
            m_ErrorNotification.SetActive(true);
            m_ErrorLabel.text = errorMessage;
        }
#endif
    }
}
