using System;
using System.Net;

public struct NetworkDataEncoder
{
    byte[] m_Buffer;

    int m_Offset;

    public NetworkDataEncoder(byte[] buffer)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));

        m_Buffer = buffer;
        m_Offset = 0;
    }

    public int length => m_Offset;

    public unsafe void Encode(float value) => Encode(*(int*)&value);

    public unsafe void Encode(double value) => Encode(*(long*)&value);

    public void Encode(ushort value) => Encode((short)value);

    public void Encode(uint value) => Encode((int)value);

    public void Encode(ulong value) => Encode((long)value);

    public void Encode(byte value)
    {
        if (m_Offset + 1 > m_Buffer.Length)
            throw new InvalidOperationException("Buffer is full. Cannot write more data.");

        m_Buffer[m_Offset++] = value;
    }

    public unsafe void Encode(short value)
    {
        int newOffset = m_Offset + 2;
        if (newOffset > m_Buffer.Length)
            throw new InvalidOperationException("Buffer is full. Cannot write more data.");

        fixed(byte* ptr = &m_Buffer[m_Offset])
        {
            *(short*)ptr = IPAddress.HostToNetworkOrder(value);
        }
        m_Offset = newOffset;
    }

    public unsafe void Encode(int value)
    {
        int newOffset = m_Offset + 4;
        if (newOffset > m_Buffer.Length)
            throw new InvalidOperationException("Buffer is full. Cannot write more data.");

        fixed(byte* ptr = &m_Buffer[m_Offset])
        {
            *(int*)ptr = IPAddress.HostToNetworkOrder(value);
        }
        m_Offset = newOffset;
    }

    public unsafe void Encode(long value)
    {
        int newOffset = m_Offset + 8;
        if (newOffset > m_Buffer.Length)
            throw new InvalidOperationException("Buffer is full. Cannot write more data.");

        fixed(byte* ptr = &m_Buffer[m_Offset])
        {
            *(long*)ptr = IPAddress.HostToNetworkOrder(value);
        }
        m_Offset = newOffset;
    }
}
