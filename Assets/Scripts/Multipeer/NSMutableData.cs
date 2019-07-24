using System;
using System.Runtime.InteropServices;

namespace Unity.iOS.Multipeer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NSMutableData : IDisposable, IEquatable<NSMutableData>
    {
        IntPtr m_Ptr;

        internal NSMutableData(IntPtr existing) => m_Ptr = existing;

        public bool Created => m_Ptr != IntPtr.Zero;

        public NSData ToNSData() => new NSData(m_Ptr);

        // public void Append(IntPtr bytes, int length)
        // {
        //     if (!Created)
        //         throw new InvalidOperationException($"Cannot append to the {typeof(NSMutableData).Name} because it has not been created.");

        //     Append(m_Ptr, bytes, length);
        // }

        // public static NSMutableData CreateEmpty() => new NSMutableData(Init());

        // public static NSMutableData CreateWithBytes(IntPtr bytes, int length)
        //     => new NSMutableData(InitWithBytes(bytes, length));

        // public static NSMutableData CreateWithBytesNoCopy(IntPtr bytes, int length, bool freeBytesOnDisposal = false)
        //     => new NSMutableData(InitWithBytesNoCopy(bytes, length, freeBytesOnDisposal));

        public void Dispose() => NativeApi.CFRelease(ref m_Ptr);

        public override int GetHashCode() => m_Ptr.GetHashCode();
        public override bool Equals(object obj) => (obj is NSMutableData) && Equals((NSMutableData)obj);
        public bool Equals(NSMutableData other) => m_Ptr == other.m_Ptr;
        public static bool operator==(NSMutableData lhs, NSMutableData rhs) => lhs.Equals(rhs);
        public static bool operator!=(NSMutableData lhs, NSMutableData rhs) => !lhs.Equals(rhs);

        // [DllImport("__Internal", EntryPoint="UnityMultipeer_NSMutableData_init")]
        // static extern IntPtr Init();

        // [DllImport("__Internal", EntryPoint="UnityMultipeer_NSMutableData_initWithBytes")]
        // static extern IntPtr InitWithBytes(IntPtr bytes, int length);

        // [DllImport("__Internal", EntryPoint="UnityARKit_NSMutableData_initWithBytesNoCopy")]
        // static extern IntPtr InitWithBytesNoCopy(IntPtr bytes, int length, bool freeBytesOnDisposal);

        // [DllImport("__Internal", EntryPoint="UnityARKit_NSMutableData_append")]
        // static extern IntPtr Append(IntPtr ptr, IntPtr bytes, int length);
    }
}
