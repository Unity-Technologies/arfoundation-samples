#include "MultipeerDelegate.h"

typedef void* ManagedMultipeerDelegate;
typedef void* ManagedNSError;

ManagedMultipeerDelegate UnityMC_Delegate_initWithName(void* name_nsstring, void* serviceType_nsstring)
{
    NSString* name = (__bridge NSString*)name_nsstring;
    MultipeerDelegate* delegate = [[MultipeerDelegate alloc] initWithName:name
                                                              serviceType:(__bridge NSString*)serviceType_nsstring];
    return (__bridge_retained void*)delegate;
}

ManagedNSError UnityMC_Delegate_sendToAllPeers(void* self, void* nsdata, int length, int mode)
{
    NSData* data = (__bridge NSData*)nsdata;
    MultipeerDelegate* delegate = (__bridge MultipeerDelegate*)self;
    NSError* error = [delegate sendToAllPeers:data withMode:(MCSessionSendDataMode)mode];
    return (__bridge_retained void*)error;
}

int UnityMC_Delegate_ReceivedDataQueueSize(void* self)
{
    if (self == NULL)
        return 0;
    
    MultipeerDelegate* delegate = (__bridge MultipeerDelegate*)self;
    return (int)delegate.queueSize;
}

void* UnityMC_Delegate_DequeueReceivedData(void* self)
{
    MultipeerDelegate* delegate = (__bridge MultipeerDelegate*)self;
    return (__bridge_retained void*)delegate.dequeue;
}

int UnityMC_Delegate_ConnectedPeerCount(void* self)
{
    MultipeerDelegate* delegate = (__bridge MultipeerDelegate*)self;
    return (int)delegate.connectedPeerCount;
}

void UnityMC_Delegate_SetEnabled(void* self, bool enabled)
{
    MultipeerDelegate* delegate = (__bridge MultipeerDelegate*)self;
    delegate.enabled = enabled;
}

bool UnityMC_Delegate_GetEnabled(void* self)
{
    MultipeerDelegate* delegate = (__bridge MultipeerDelegate*)self;
    return delegate.enabled;
}

void UnityMultipeer_CFRelease(void* ptr)
{
    if (ptr)
    {
        CFRelease(ptr);
    }
}

