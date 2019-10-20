using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.iOS.Multipeer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NSString : IDisposable, IEquatable<NSString>
    {
        IntPtr m_Ptr;

        internal NSString(IntPtr existing) => m_Ptr = existing;

        public NSString(string text) => m_Ptr = CreateWithString(text, text.Length);

        public NSString(NSData serializedString)
        {
            if (!serializedString.Created)
                throw new ArgumentException("The serialized string is not valid.", nameof(serializedString));

            m_Ptr = Deserialize(serializedString);
        }

        public bool Created => m_Ptr != IntPtr.Zero;

        public int Length => GetLength(this);

        public override unsafe string ToString()
        {
            if (!Created)
                return string.Empty;

            using (var buffer = new NativeArray<byte>(GetLengthOfBytes(this), Allocator.TempJob, NativeArrayOptions.UninitializedMemory))
            {
                if (GetBytes(this, buffer.GetUnsafePtr(), buffer.Length))
                {
                    return Marshal.PtrToStringUni(new IntPtr(buffer.GetUnsafePtr()), Length);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public NSData Serialize()
        {
            if (!Created)
                throw new InvalidOperationException($"The {typeof(NSString).Name} has not been created.");

            return Serialize(this);
        }

        public void Dispose() => NativeApi.CFRelease(ref m_Ptr);
        public override int GetHashCode() => m_Ptr.GetHashCode();
        public override bool Equals(object obj) => (obj is NSString) && Equals((NSString)obj);
        public bool Equals(NSString other) => m_Ptr == other.m_Ptr;
        public static bool operator==(NSString lhs, NSString rhs) => lhs.Equals(rhs);
        public static bool operator!=(NSString lhs, NSString rhs) => !lhs.Equals(rhs);

        [DllImport("__Internal", EntryPoint="UnityMC_NSString_createWithString")]
        static extern IntPtr CreateWithString([MarshalAs(UnmanagedType.LPWStr)] string text, int length);

        [DllImport("__Internal", EntryPoint="UnityMC_NSString_lengthOfBytesUsingEncoding")]
        static extern int GetLengthOfBytes(NSString self);

        [DllImport("__Internal", EntryPoint="UnityMC_NSString_getLength")]
        static extern int GetLength(NSString self);

        [DllImport("__Internal", EntryPoint="UnityMC_NSString_getBytes")]
        static extern unsafe bool GetBytes(NSString self, void* buffer, int length);

        [DllImport("__Internal", EntryPoint="UnityMC_NSString_serialize")]
        static extern NSData Serialize(NSString self);

        [DllImport("__Internal", EntryPoint="UnityMC_NSString_deserialize")]
        static extern IntPtr Deserialize(NSData data);
    }
}
