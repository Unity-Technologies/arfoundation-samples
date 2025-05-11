using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public static class IPAddressUtil
    {
        public static string GetDeviceIPAddress()
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
    }
}
