using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class NetworkStarter : MonoBehaviour
    {
        [SerializeField]
        UnityEvent m_StartNetworkRequested = new();
        public UnityEvent startNetworkRequested => m_StartNetworkRequested;

        [SerializeField]
        UnityEvent m_networkFailedToStart = new();
        public UnityEvent networkFailedToStart => m_networkFailedToStart;

        UnityTransport m_UnityTransport;

        public void StartNetworkSession(bool startAsHost)
        {
            m_StartNetworkRequested?.Invoke();

            var success = startAsHost ? NetworkManager.Singleton.StartHost() : NetworkManager.Singleton.StartClient();
            if (!success)
                networkFailedToStart?.Invoke();
        }

        public void SetConnectionIPAddress(string ipAddress)
        {
            m_UnityTransport.ConnectionData.Address = ipAddress;
        }

        void Start()
        {
            m_UnityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            if (m_UnityTransport == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_UnityTransport)} is null."), this);
        }
    }
}
