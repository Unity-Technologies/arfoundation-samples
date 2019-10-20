namespace Unity.iOS.Multipeer
{
    /// <summary>
    /// MCSession send modes
    /// </summary>
    public enum MCSessionSendDataMode
    {
        /// <summary>
        /// Guaranteed reliable and in-order delivery.
        /// </summary>
        Reliable,

        /// <summary>
        /// Sent immediately without queuing, no guaranteed delivery.
        /// </summary>
        Unreliable
    }
}
