using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

#if UNITY_IOS
using System.Runtime.InteropServices;
using UnityEngine.XR.ARKit;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
#if UNITY_IOS && !UNITY_EDITOR
    // This custom ConfigurationChooser tells the Apple ARKit XR Plug-in to use an ARGeoTrackingConfiguration
    // See https://developer.apple.com/documentation/arkit/argeotrackingconfiguration?language=objc
    class ARGeoAnchorConfigurationChooser : ConfigurationChooser
    {
        static readonly ConfigurationChooser s_DefaultChooser = new DefaultConfigurationChooser();

        static readonly ConfigurationDescriptor s_ARGeoConfigurationDescriptor = new ConfigurationDescriptor(
            // On iOS, the "identifier" used by the ConfigurationDescriptor is the Objective-C metaclass for the type
            // of configuration that should be used. In general, you should not create new ConfigurationDescriptors
            // (you should pick one of the descriptors passed into ChooseConfiguration); however, this will do for this
            // specific case.
            ARGeoTrackingConfigurationClass,

            // These are the "features" supported by the ARGeoTrackingConfiguration
            Feature.WorldFacingCamera |
            Feature.PositionAndRotation |
            Feature.ImageTracking |
            Feature.PlaneTracking |
            Feature.ObjectTracking |
            Feature.EnvironmentProbes,

            // Rank is meant to be used as a tie breaker in our implementation of ChooseConfiguration, but since we will
            // always choose this descriptor, it doesn't matter what value we use here.
            0);

        public override Configuration ChooseConfiguration(NativeSlice<ConfigurationDescriptor> descriptors, Feature requestedFeatures)
        {
            // If location services are running, then we can request an ARGeoTrackingConfiguration by its class pointer
            return Input.location.status == LocationServiceStatus.Running
                ? new Configuration(s_ARGeoConfigurationDescriptor, requestedFeatures.Intersection(s_ARGeoConfigurationDescriptor.capabilities))
                : s_DefaultChooser.ChooseConfiguration(descriptors, requestedFeatures);
        }

        public static extern IntPtr ARGeoTrackingConfigurationClass
        {
            [DllImport("__Internal", EntryPoint = "ARGeoTrackingConfiguration_class")]
            get;
        }
    }
#endif

    [RequireComponent(typeof(ARSession))]
    public class EnableGeoAnchors : PressInputBase
    {
#if UNITY_IOS && !UNITY_EDITOR
        public static bool IsSupported => ARGeoAnchorConfigurationChooser.ARGeoTrackingConfigurationClass != IntPtr.Zero;

        // See https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.0/manual/extensions.html
        public struct NativePtrData
        {
            public int version;
            public IntPtr sessionPtr;
        }

        // See https://developer.apple.com/documentation/corelocation/cllocationcoordinate2d?language=objc
        public struct CLLocationCoordinate2D
        {
            public double latitude;
            public double longitude;
        }

        // ARGeoAnchors requires location services
        void Start() => Input.location.Start();

        void OnGUI()
        {
            GUI.skin.label.fontSize = 50;
            GUILayout.Space(100);

            if (ARGeoAnchorConfigurationChooser.ARGeoTrackingConfigurationClass == IntPtr.Zero)
            {
                GUILayout.Label("ARGeoTrackingConfiguration is not supported on this device.");
                return;
            }

            switch (Input.location.status)
            {
                case LocationServiceStatus.Initializing:
                    GUILayout.Label("Waiting for location services...");
                    break;
                case LocationServiceStatus.Stopped:
                    GUILayout.Label("Location services stopped. Unable to use geo anchors.");
                    break;
                case LocationServiceStatus.Failed:
                    GUILayout.Label("Location services failed. Unable to use geo anchors.");
                    break;
                case LocationServiceStatus.Running:
                    GUILayout.Label("Tap screen to add a geo anchor.");
                    break;
            }
        }

        protected override void OnPress(Vector3 position)
        {
            // Can't do anything interesting until location services are running.
            if (Input.location.status != LocationServiceStatus.Running)
                return;

            if (GetComponent<ARSession>().subsystem is ARKitSessionSubsystem subsystem)
            {
                var isValidSession = TryGetSession(subsystem, out var session);
                if(!isValidSession)
                    return;

                // Get last known location data
                var locationData = Input.location.lastData;

                // Add a geo anchor. See GeoAnchorsNativeInterop.m to see how this works.
                AddGeoAnchor(session, new CLLocationCoordinate2D
                {
                    latitude = locationData.latitude,
                    longitude = locationData.longitude
                }, locationData.altitude);
            }
        }

        void Update()
        {
            if (GetComponent<ARSession>().subsystem is ARKitSessionSubsystem subsystem)
            {
                if (!(subsystem.configurationChooser is ARGeoAnchorConfigurationChooser))
                {
                    // Replace the config chooser with our own
                    subsystem.configurationChooser = new ARGeoAnchorConfigurationChooser();
                }

                // We don't have to do this, but it will silence a warning message in Xcode
                // since the ARGeoTrackingConfiguration can only use the GravityAndHeading value.
                subsystem.requestedWorldAlignment = ARWorldAlignment.GravityAndHeading;

                var isValidSession = TryGetSession(subsystem, out var session);
                if(!isValidSession)
                    return;

                DoSomethingWithSession(session);
            }
        }

        bool TryGetSession(ARKitSessionSubsystem subsystem, out IntPtr session)
        {
            if (subsystem.nativePtr == IntPtr.Zero)
            {
                session = IntPtr.Zero;
                return false;
            }

            session = Marshal.PtrToStructure<NativePtrData>(subsystem.nativePtr).sessionPtr;
            if (session == IntPtr.Zero)
                return false;
            else
                return true;
        }

        [DllImport("__Internal")]
        static extern void DoSomethingWithSession(IntPtr session);

        [DllImport("__Internal", EntryPoint = "ARSession_addGeoAnchor")]
        static extern void AddGeoAnchor(IntPtr session, CLLocationCoordinate2D coordinate, double altitude);
#else
        public static bool IsSupported => false;
#endif
    }
}
