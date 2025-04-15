using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class IPAddressVisualizer : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI m_IPAddressTitleLabel;

        [SerializeField]
        TextMeshProUGUI m_IPAddressValueLabel;

        void Start()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                m_IPAddressTitleLabel.text = "My IP:";
                m_IPAddressValueLabel.text = GetDeviceIPAddress();
            }
            else
            {
                m_IPAddressTitleLabel.text = "Host IP:";
                m_IPAddressValueLabel.text = GetConnectedSessionIPAddress();
            }
        }

        string GetDeviceIPAddress()
        {
            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.OperationalStatus != OperationalStatus.Up ||
                    networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                    continue;

                foreach (var ipInfo in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (ipInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                        return ipInfo.Address.ToString();
                }
            }

            return "Unknown";
        }

        string GetConnectedSessionIPAddress()
        {
            var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            return unityTransport != null ? unityTransport.ConnectionData.Address : "Unknown";
        }
    }
}
