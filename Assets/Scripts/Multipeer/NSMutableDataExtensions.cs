using System;

namespace Unity.iOS.Multipeer.Unsafe
{
    public static unsafe class NSMutableDataUnsafeUtility
    {
        // public static NSMutableData CreateWithBytes(byte[] bytes, int offset, int length)
        // {
        //     fixed(byte* ptr = &bytes[offset])
        //     {
        //         return NSMutableData.CreateWithBytes(new IntPtr(ptr), length);
        //     }
        // }
    }
}
