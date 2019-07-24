#import <Foundation/Foundation.h>

void* UnityMultipeer_NSMutableData_init()
{
    NSMutableData* data = [[NSMutableData alloc] init];
    return (__bridge_retained void*)data;
}

void* UnityMultipeer_NSMutableData_initWithBytes(const void* bytes, int length)
{
    NSMutableData* data = [[NSMutableData alloc] initWithBytes:bytes
                                                        length:length];
    return (__bridge_retained void*)data;
}

void* UnityMultipeer_NSMutableData_initWithBytesNoCopy(void* bytes, int length, bool freeWhenDone)
{
    NSMutableData* data = [[NSMutableData alloc] initWithBytesNoCopy:bytes
                                                              length:length
                                                        freeWhenDone:freeWhenDone];
    return (__bridge_retained void*)data;
}
