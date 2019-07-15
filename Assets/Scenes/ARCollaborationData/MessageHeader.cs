public struct MessageHeader : IMessage
{
    public int messageSize;

    public MessageType messageType;

    public int EncodeTo(byte[] bytes)
    {
        var encoder = new NetworkDataEncoder(bytes);
        encoder.Encode(messageSize);
        encoder.Encode((byte)messageType);
        return encoder.length;
    }

    public const int k_EncodedSize = sizeof(int) + sizeof(byte);

    public MessageHeader(byte[] bytes, int size)
    {
        var decoder = new NetworkDataDecoder(bytes, size);
        messageSize = decoder.DecodeInt();
        messageType = (MessageType)decoder.DecodeByte();
    }
}
