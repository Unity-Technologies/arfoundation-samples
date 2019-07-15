using System;
using System.Net.Sockets;
using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

/// <summary>
/// Sends and receives data using a fixed size byte[] buffer. Because the
/// buffer is reused, no additional GC allocations are made after construction.
/// </summary>
public struct NetworkBuffer
{
    byte[] m_Buffer;

    public NetworkBuffer(int bufferSize)
    {
        m_Buffer = new byte[bufferSize];
    }

    public byte[] buffer => m_Buffer;

    public int bufferSize => (m_Buffer == null) ? 0 : m_Buffer.Length;

    public int Read(NetworkStream stream, int offset, int size)
    {
        ValidateAndThrow(stream);

        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), offset, $"{nameof(offset)} must be greater than or equal to zero.");

        if (size < 0)
            throw new ArgumentOutOfRangeException(nameof(size), size, $"{nameof(size)} must be greater than or equal to zero.");

        if (offset + size > m_Buffer.Length)
            throw new InvalidOperationException($"Reading {size} bytes starting at offset {offset} would read past the end of the buffer (buffer length = {m_Buffer.Length}).");

        int bytesRemaining = size;
        while (bytesRemaining > 0)
        {
            int bytesRead = stream.Read(m_Buffer, offset, bytesRemaining);
            CollaborationNetworkingIndicator.NotifyIncomingDataReceived();
            offset += bytesRead;
            bytesRemaining -= bytesRead;
        }

        return size;
    }

    public void Send(NetworkStream stream, int offset, int size)
    {
        ValidateAndThrow(stream);

        if (offset + size > m_Buffer.Length)
            throw new InvalidOperationException($"Writing {size} bytes starting at offset {offset} would write past the end of the buffer (buffer length = {m_Buffer.Length}).");

        try
        {
            stream.Write(m_Buffer, offset, size);
            CollaborationNetworkingIndicator.NotifyOutgoingDataSent();
        }
        catch (SocketException socketException)
        {
            Logger.Log($"Socket exception: {socketException}");
        }
    }

    public unsafe void Send(NetworkStream stream, NativeSlice<byte> bytes)
    {
        ValidateAndThrow(stream);

        var basePtr = new IntPtr(bytes.GetUnsafeReadOnlyPtr());
        int bytesRemaining = bytes.Length;
        int offset = 0;

        while (bytesRemaining > 0)
        {
            // Memcpy next chunk into destinationBuffer
            int size = Mathf.Min(m_Buffer.Length, bytesRemaining);
            fixed(byte* dst = m_Buffer)
            {
                var src = basePtr + offset;
                UnsafeUtility.MemCpy(dst, (void*)src, size);
            }

            bytesRemaining -= size;
            offset += size;

            Send(stream, 0, size);
        }
    }

    public void Send<T>(NetworkStream stream, T message) where T : struct, IMessage
    {
        ValidateAndThrow(stream);

        int size = message.EncodeTo(m_Buffer);
        Send(stream, 0, size);
    }

    void ValidateAndThrow(NetworkStream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        if (m_Buffer == null)
            throw new InvalidOperationException($"{nameof(NetworkBuffer)} has not been initialized.");
    }
}
