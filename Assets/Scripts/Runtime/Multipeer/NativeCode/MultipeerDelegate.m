#import "MultipeerDelegate.h"

@implementation MultipeerDelegate

MCSession* m_Session;
MCPeerID* m_PeerID;
NSMutableArray* m_Queue;
MCNearbyServiceAdvertiser* m_ServiceAdvertiser;
MCNearbyServiceBrowser* m_ServiceBrowser;
BOOL m_Enabled;

- (instancetype)initWithName:(nonnull NSString *)name serviceType:(nonnull NSString *)serviceType
{
    if (self = [super init])
    {
        m_Enabled = false;
        m_Queue = [[NSMutableArray alloc] init];
        m_PeerID = [[MCPeerID alloc] initWithDisplayName: name];
        m_Session = [[MCSession alloc] initWithPeer:m_PeerID
                                   securityIdentity:nil
                               encryptionPreference:MCEncryptionRequired];
        m_Session.delegate = self;

        m_ServiceAdvertiser = [[MCNearbyServiceAdvertiser alloc] initWithPeer:m_PeerID
                                                                discoveryInfo:nil
                                                                  serviceType:serviceType];
        m_ServiceAdvertiser.delegate = self;

        m_ServiceBrowser = [[MCNearbyServiceBrowser alloc] initWithPeer:m_PeerID
                                                            serviceType:serviceType];
        m_ServiceBrowser.delegate = self;
    }

    return self;
}

- (BOOL)enabled
{
    return m_Enabled;
}

- (void)setEnabled:(BOOL)enabled
{
    if (enabled)
    {
        [m_ServiceAdvertiser startAdvertisingPeer];
        [m_ServiceBrowser startBrowsingForPeers];
    }
    else
    {
        [m_ServiceAdvertiser stopAdvertisingPeer];
        [m_ServiceBrowser stopBrowsingForPeers];
        @synchronized (m_Queue)
        {
            [m_Queue removeAllObjects];
        }
    }

    m_Enabled = enabled;
}

- (NSError*)sendToAllPeers:(nonnull NSData*)data withMode:(MCSessionSendDataMode)mode
{
    if (m_Session.connectedPeers.count == 0)
        return nil;

    NSError* error = nil;
    [m_Session sendData:data
                toPeers:m_Session.connectedPeers
               withMode:mode
                  error:&error];

    return error;
}

- (NSUInteger)queueSize
{
    @synchronized (m_Queue)
    {
        return m_Queue.count;
    }
}

- (nonnull NSData*)dequeue
{
    @synchronized (m_Queue)
    {
        NSData* data = [m_Queue objectAtIndex:0];
        [m_Queue removeObjectAtIndex:0];
        return data;
    }
}

- (NSUInteger)connectedPeerCount
{
    return m_Session.connectedPeers.count;
}

- (void)session:(nonnull MCSession *)session didFinishReceivingResourceWithName:(nonnull NSString *)resourceName fromPeer:(nonnull MCPeerID *)peerID atURL:(nullable NSURL *)localURL withError:(nullable NSError *)error {
    // Not used.
}

- (void)session:(nonnull MCSession *)session didReceiveData:(nonnull NSData *)data fromPeer:(nonnull MCPeerID *)peerID
{
    @synchronized (m_Queue)
    {
        [m_Queue addObject:data];
    }
}

- (void)session:(nonnull MCSession *)session didReceiveStream:(nonnull NSInputStream *)stream withName:(nonnull NSString *)streamName fromPeer:(nonnull MCPeerID *)peerID {
    // Not used.
}

- (void)session:(nonnull MCSession *)session didStartReceivingResourceWithName:(nonnull NSString *)resourceName fromPeer:(nonnull MCPeerID *)peerID withProgress:(nonnull NSProgress *)progress {
    // Not used.
}

- (void)session:(nonnull MCSession *)session peer:(nonnull MCPeerID *)peerID didChangeState:(MCSessionState)state {
    // Not used.
}

- (void)advertiser:(nonnull MCNearbyServiceAdvertiser *)advertiser didReceiveInvitationFromPeer:(nonnull MCPeerID *)peerID withContext:(nullable NSData *)context invitationHandler:(nonnull void (^)(BOOL, MCSession * _Nullable))invitationHandler
{
    invitationHandler(YES, m_Session);
}

- (void)browser:(nonnull MCNearbyServiceBrowser *)browser foundPeer:(nonnull MCPeerID *)peerID withDiscoveryInfo:(nullable NSDictionary<NSString *,NSString *> *)info
{
    // Invite the peer to join our session
    [browser invitePeer:peerID
              toSession:m_Session
            withContext:nil
                timeout:10];
}

- (void)browser:(nonnull MCNearbyServiceBrowser *)browser lostPeer:(nonnull MCPeerID *)peerID
{
    // Not used
}

@end
