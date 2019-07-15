using System;
using System.Net;

public struct NetworkDataDecoder
{
    byte[] m_Buffer;

    int m_Offset;

    int m_Length;

    public NetworkDataDecoder(byte[] buffer, int size)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));

        if (size > buffer.Length)
            throw new ArgumentOutOfRangeException(nameof(size), size, $"'{nameof(size)}' is greater than the length of {nameof(buffer)} ({buffer.Length}).");

        m_Buffer = buffer;
        m_Offset = 0;
        m_Length = size;
    }

    public unsafe float DecodeFloat()
    {
        var value = DecodeInt();
        return *(float*)&value;
    }

    public unsafe double DecodeDouble()
    {
        var value = DecodeLong();
        return *(double*)&value;
    }

    public ushort DecodeUShort() => (ushort)DecodeShort();

    public uint DecodeUInt() => (uint)DecodeInt();

    public ulong DecodeULong() => (ulong)DecodeLong();

    public byte DecodeByte()
    {
        if (m_Offset >= m_Length)
            throw new InvalidOperationException("Buffer is exhausted. Cannot decode more data.");

        return m_Buffer[m_Offset++];
    }

    public unsafe short DecodeShort()
    {
        if (m_Offset + 2 > m_Length)
            throw new InvalidOperationException("Buffer is exhausted. Cannot decode more data.");

        fixed(byte* ptr = &m_Buffer[m_Offset])
        {
            m_Offset += 2;
            return IPAddress.NetworkToHostOrder(*(short*)ptr);
        }
    }

    public unsafe int DecodeInt()
    {
        if (m_Offset + 4 > m_Length)
            throw new InvalidOperationException("Buffer is exhausted. Cannot decode more data.");

        fixed(byte* ptr = &m_Buffer[m_Offset])
        {
            m_Offset += 4;
            return IPAddress.NetworkToHostOrder(*(int*)ptr);
        }
    }

    public unsafe long DecodeLong()
    {
        if (m_Offset + 8 > m_Length)
            throw new InvalidOperationException("Buffer is exhausted. Cannot decode more data.");

        fixed(byte* ptr = &m_Buffer[m_Offset])
        {
            m_Offset += 8;
            return IPAddress.NetworkToHostOrder(*(long*)ptr);
        }
    }
}
