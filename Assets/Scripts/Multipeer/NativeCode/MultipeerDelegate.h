#import <MultipeerConnectivity/MultipeerConnectivity.h>

@interface MultipeerDelegate : NSObject<MCSessionDelegate, MCNearbyServiceAdvertiserDelegate, MCNearbyServiceBrowserDelegate>

- (nullable instancetype)initWithName:(nonnull NSString *)name serviceType:(nonnull NSString*)serviceType;
- (nullable NSError*)sendToAllPeers:(nonnull NSData*)data withMode:(MCSessionSendDataMode)mode;
- (NSUInteger)connectedPeerCount;
- (NSUInteger)queueSize;
- (nonnull NSData*)dequeue;

@property BOOL enabled;

@end
