using System;
using UnityEngine.Events;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.OpenXR;

#if METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
using System.Text;
using UnityEngine.XR.OpenXR.Features.Meta;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MetaColocationDiscovery : MonoBehaviour
    {
#if METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
        [SerializeField]
        UnityEvent<XRResultStatus, ColocationState> m_AdvertisementStateChanged;
        public UnityEvent<XRResultStatus, ColocationState> advertisementStateChanged => m_AdvertisementStateChanged;

        [SerializeField]
        UnityEvent<XRResultStatus, ColocationState> m_DiscoveryStateChanged;
        public UnityEvent<XRResultStatus, ColocationState> discoveryStateChanged => m_DiscoveryStateChanged;

        [SerializeField]
        UnityEvent<SerializableGuid, string> m_HostIPAddressDiscovered;
        public UnityEvent<SerializableGuid, string> hostIPAddressDiscovered => m_HostIPAddressDiscovered;

        public ColocationState advertisementState => m_ColocationDiscoveryFeature.advertisementState;

        public ColocationState discoveryState => m_ColocationDiscoveryFeature.discoveryState;

        bool m_IsColocationDiscoverySupported;
        public bool IsColocationDiscoverySupported => m_IsColocationDiscoverySupported;

        ColocationDiscoveryFeature m_ColocationDiscoveryFeature;

        public async Awaitable<Result<SerializableGuid>> StartIPAddressAdvertisement()
        {
            var ipAddress = IPAddressUtil.GetDeviceIPAddress();
            var bytes = Encoding.ASCII.GetBytes(ipAddress);

            return await m_ColocationDiscoveryFeature.TryStartAdvertisementAsync(bytes.AsSpan());
        }

        public async Awaitable StopIPAddressAdvertisement()
        {
            await m_ColocationDiscoveryFeature.TryStopAdvertisementAsync();
        }

        public async Awaitable<XRResultStatus> StartDiscoveryForHostIPAddress()
        {
            return await m_ColocationDiscoveryFeature.TryStartDiscoveryAsync();
        }

        public async Awaitable StopDiscoveryForHostIPAddress()
        {
            await m_ColocationDiscoveryFeature.TryStopDiscoveryAsync();
        }

        void Awake()
        {
            m_ColocationDiscoveryFeature = OpenXRSettings.Instance.GetFeature<ColocationDiscoveryFeature>();
            m_IsColocationDiscoverySupported = m_ColocationDiscoveryFeature.enabled;

            m_ColocationDiscoveryFeature.advertisementStateChanged += OnAdvertismentStateChanged;
            m_ColocationDiscoveryFeature.discoveryStateChanged += OnDiscoveryStateChanged;
            m_ColocationDiscoveryFeature.messageDiscovered += OnMessageDiscovered;
        }

        async void OnDestroy()
        {
            m_ColocationDiscoveryFeature.advertisementStateChanged -= OnAdvertismentStateChanged;
            m_ColocationDiscoveryFeature.discoveryStateChanged -= OnDiscoveryStateChanged;
            m_ColocationDiscoveryFeature.messageDiscovered -= OnMessageDiscovered;

            if (discoveryState is ColocationState.Active or ColocationState.Starting)
                await StopDiscoveryForHostIPAddress();

            if (advertisementState is ColocationState.Active or ColocationState.Starting)
                await StopIPAddressAdvertisement();
        }

        void OnAdvertismentStateChanged(object sender, Result<ColocationState> result)
        {
            m_AdvertisementStateChanged?.Invoke(result.status, result.value);
        }

        void OnDiscoveryStateChanged(object sender, Result<ColocationState> result)
        {
            m_DiscoveryStateChanged?.Invoke(result.status, result.value);
        }

        void OnMessageDiscovered(object sender, ColocationDiscoveryMessage message)
        {
            var discoveredIPAddress = Encoding.ASCII.GetString(message.data);
            m_HostIPAddressDiscovered?.Invoke(message.advertisementId, discoveredIPAddress);
        }
#endif
    }
}
