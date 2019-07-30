#import <Foundation/Foundation.h>

int UnityMC_NSData_getLength(void* self)
{
    NSData* data = (__bridge NSData*)self;
    return (int)data.length;
}

void* UnityMC_NSData_createWithBytes(void* bytes, int length)
{
    NSData* data = [[NSData alloc] initWithBytes:bytes
                                          length:length];

    return (__bridge_retained void*)data;
}

void* UnityMC_NSData_createWithBytesNoCopy(void* bytes, int length, bool freeWhenDone)
{
    NSData* data = [[NSData alloc] initWithBytesNoCopy:bytes
                                                length:length
                                          freeWhenDone:freeWhenDone];

    return (__bridge_retained void*)data;
}

const void* UnityMC_NSData_getBytes(void* self)
{
    NSData* data = (__bridge NSData*)self;
    return data.bytes;
}
