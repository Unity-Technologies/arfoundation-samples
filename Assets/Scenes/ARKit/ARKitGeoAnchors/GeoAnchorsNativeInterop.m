#import <ARKit/ARKit.h>

// Gets the Objective-C metaclass for ARGeoTrackingConfiguration
Class ARGeoTrackingConfiguration_class() {
    // ARGeoTrackingConfiguration requires iOS 14
    if (@available(iOS 14, *)) {
        if (ARGeoTrackingConfiguration.isSupported) {
            return [ARGeoTrackingConfiguration class];
        } else {
            NSLog(@"ARGeoTrackingConfiguration is not supported on this device.");
        }
    }
    return NULL;
}

// Adds an ARGeoAnchor to the ARSession
void ARSession_addGeoAnchor(void* self, CLLocationCoordinate2D coordinate, double altitude) {
    if (@available(iOS 14, *)) {
        // Cast the void* back to an ARSession
        ARSession* session = (__bridge ARSession*)self;

        // Create a new ARGeoAnchor
        // See https://developer.apple.com/documentation/arkit/argeoanchor/3551719-initwithcoordinate?language=objc
        ARGeoAnchor* geoAnchor = [[ARGeoAnchor alloc] initWithCoordinate:coordinate altitude:altitude];

        // Add the new anchor to the session
        [session addAnchor:geoAnchor];

        NSLog(@"Added ARGeoAnchor at (%f, %f) at %f meters", coordinate.latitude, coordinate.longitude, altitude);
    }
}

// Flips a transform from ARKit's right-handed coordinate system to Unity's left-handed coordinate system.
static inline simd_float4x4 FlipHandedness(simd_float4x4 transform) {
    const simd_float4* c = transform.columns;
    return simd_matrix(simd_make_float4( c[0].xy, -c[0].z, c[0].w),
                       simd_make_float4( c[1].xy, -c[1].z, c[1].w),
                       simd_make_float4(-c[2].xy,  c[2].z, c[2].w),
                       simd_make_float4( c[3].xy, -c[3].z, c[3].w));
}

// Demonstrates "doing something" with the ARSession
void DoSomethingWithSession(void* sessionPtr) {

    // ===============================
    // Replace this with your own code
    // ===============================

    // In this example, just look for all ARGeoAnchors and print out their transforms.
    if (@available(iOS 14, *)) {
        ARSession* session = (__bridge ARSession*)sessionPtr;

        for (ARAnchor* anchor in session.currentFrame.anchors) {
            if ([anchor isKindOfClass:[ARGeoAnchor class]]) {
                ARGeoAnchor* geoAnchor = (ARGeoAnchor*)anchor;
                const simd_float4x4 transform = FlipHandedness(geoAnchor.transform);
                const simd_float4* c = transform.columns;
                NSLog(@"ARGeoAnchor %@ transform:\n"
                       "[%+f %+f %+f %+f]\n"
                       "[%+f %+f %+f %+f]\n"
                       "[%+f %+f %+f %+f]\n"
                       "[%+f %+f %+f %+f]\n",
                       [geoAnchor.identifier UUIDString],
                       c[0].x, c[1].x, c[2].x, c[3].x,
                       c[0].y, c[1].y, c[2].y, c[3].y,
                       c[0].z, c[1].z, c[2].z, c[3].z,
                       c[0].w, c[1].w, c[2].w, c[3].w);
            }
        }
    }
}

bool ARFoundationSamples_IsiOS14OrLater() {
    if (@available(iOS 14, *)) {
        return true;
    }
    return false;
}
