using System;
using System.Runtime.InteropServices;

namespace Unity.iOS.Multipeer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MCSession : IDisposable, IEquatable<MCSession>
    {
        IntPtr m_Ptr;

        public bool Created => m_Ptr != IntPtr.Zero;

        public bool Enabled
        {
            get
            {
                if (!Created)
                    return false;
                return GetEnabled(this);
            }

            set
            {
                if (!Created && value)
                    throw new InvalidOperationException();
                SetEnabled(this, value);
            }
        }

        public MCSession(string peerName, string serviceType)
        {
            if (peerName == null)
                throw new ArgumentNullException(nameof(peerName));

            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            using (var peerName_NSString = new NSString(peerName))
            using (var serviceType_NSString = new NSString(serviceType))
            {
                m_Ptr = InitWithName(peerName_NSString, serviceType_NSString);
            }
        }

        public void SendToAllPeers(NSData data, MCSessionSendDataMode mode)
        {
            if (!Created)
                throw new InvalidOperationException($"The {typeof(MCSession).Name} has not been created.");

            if (!data.Created)
                throw new ArgumentException($"'{nameof(data)}' is not valid.", nameof(data));

            using (var error = SendToAllPeers(this, data, mode))
            {
                if (error.Valid)
                    throw error.ToException();
            }
        }

        public int ReceivedDataQueueSize => GetReceivedDataQueueSize(this);

        public NSData DequeueReceivedData()
        {
            if (!Created)
                throw new InvalidOperationException($"The {typeof(MCSession).Name} has not been created.");

            return DequeueReceivedData(this);
        }

        public int ConnectedPeerCount => GetConnectedPeerCount(this);

        public void Dispose() => NativeApi.CFRelease(ref m_Ptr);
        public override int GetHashCode() => m_Ptr.GetHashCode();
        public override bool Equals(object obj) => (obj is MCSession) && Equals((MCSession)obj);
        public bool Equals(MCSession other) => m_Ptr == other.m_Ptr;
        public static bool operator==(MCSession lhs, MCSession rhs) => lhs.Equals(rhs);
        public static bool operator!=(MCSession lhs, MCSession rhs) => !lhs.Equals(rhs);

        [DllImport("__Internal", EntryPoint="UnityMC_Delegate_sendToAllPeers")]
        static extern NSError SendToAllPeers(MCSession self, NSData data, MCSessionSendDataMode mode);

        [DllImport("__Internal", EntryPoint="UnityMC_Delegate_initWithName")]
        static extern IntPtr InitWithName(NSString name, NSString serviceType);

        [DllImport("__Internal", EntryPoint="UnityMC_Delegate_receivedDataQueueSize")]
        static extern int GetReceivedDataQueueSize(MCSession self);

        [DllImport("__Internal", EntryPoint="UnityMC_Delegate_dequeueReceivedData")]
        static extern NSData DequeueReceivedData(MCSession self);

        [DllImport("__Internal", EntryPoint="UnityMC_Delegate_connectedPeerCount")]
        static extern int GetConnectedPeerCount(MCSession self);

        [DllImport("__Internal", EntryPoint="UnityMC_Delegate_setEnabled")]
        static extern void SetEnabled(MCSession self, bool enabled);

        [DllImport("__Internal", EntryPoint="UnityMC_Delegate_getEnabled")]
        static extern bool GetEnabled(MCSession self);
    }
}
