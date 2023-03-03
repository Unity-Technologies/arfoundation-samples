# AR Foundation Samples

Example AR scenes that use [AR Foundation 5.1](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@5.1/manual/index.html) and demonstrate its features. Each feature is used in a minimal sample scene with example code that you can modify or copy into your project.

This sample project depends on four Unity packages:

* [AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@5.1/manual/index.html)
* [Google ARCore XR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.arcore@5.1/manual/index.html) on Android
* [Apple ARKit XR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.arkit@5.1/manual/index.html) on iOS
* [OpenXR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.5/manual/index.html) on HoloLens 2

## Which version should I use?

The `main` branch of this repository uses AR Foundation 5.1 and is compatible with Unity 2021.2 and later. To access sample scenes for previous versions of AR Foundation, refer to the table below for links to other branches.

| Unity Version  | AR Foundation Version |
| -------------- | --------------------- |
| 2023.1 (beta)  | 5.1 (prerelease)      |
| 2022.2         | [5.0 (release)](https://github.com/Unity-Technologies/arfoundation-samples/tree/5.0) |
| 2022.1         | [5.0 (release)](https://github.com/Unity-Technologies/arfoundation-samples/tree/5.0) |
| 2021.3         | [4.2 (release)](https://github.com/Unity-Technologies/arfoundation-samples/tree/4.2) |
| 2020.3         | [4.1 (release)](https://github.com/Unity-Technologies/arfoundation-samples/tree/4.1) |

## How to use these samples

### Build and run on device

You can build the AR Foundation Samples project directly to device, which can be a helpful introduction to using AR Foundation features for the first time.

To build to device, follow the steps below:

1. Install Unity 2021.2 or later and clone this repository.

2. Open the Unity project at the root of this repository.

3. As with any other Unity project, go to [Build Settings](https://docs.unity3d.com/Manual/BuildSettings.html), select your target platform, and build this project.

### Understand the sample code

All sample scenes in this project can be found in the `Assets/Scenes` folder. To learn more about the AR Foundation components used in each scene, see the [AR Foundation Documentation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@5.0/manual/index.html). Each scene is explained in more detail below.

# Table of Contents

| Sample scene(s) | Description |
| :-------------- | :---------- |
| [Simple AR](#simple-ar) | Demonstrates basic Plane detection and Raycasting
| [Camera](#camera) | Scenes that demonstrate Camera features |
| [Plane detection](#plane-detection) | Scenes that demonstrate Plane detection |
| [Image tracking](#image-tracking) | Scenes that demonstrate Image tracking |
| [Object tracking](#object-tracking) | Demonstrates Object tracking |
| [Face tracking](#face-tracking) | Scenes that demonstrate Face tracking |
| [Body tracking](#body-tracking) | Scenes that demonstrate Body tracking |
| [Point clouds](#point-clouds) | Demonstrates Point clouds |
| [Anchors](#anchors) | Demonstrates Anchors |
| [Meshing](#meshing) | Scenes that demonstrate Meshing |
| [Environment Probes](#environment-probes) | Demonstrates Environment Probes |
| [Occlusion](#occlusion) | Scenes that demonstrate Occlusion |
| [Check support](#check-support) | Demonstrates checking for AR support on device |
| [Interation](#interaction) | Demonstrates AR Foundation paired with the [XR Interaction Toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@latest) package |
| [Configuration Chooser](#configuration-chooser) | Demonstrates AR Foundation's Configuration Chooser |
| [Debug Menu](#debug-menu) | Visualize trackables and configurations on device |
| [ARKit](#arkit) | iOS-specific sample scenes |
| [ARCore Record Session](#arcore-record-session) | Demonstrates ARCore's session recording feature |

## Simple AR

This is a good starting sample that enables point cloud visualization and plane detection. There are buttons on screen that let you pause, resume, reset, and reload the ARSession.

When a plane is detected, you can tap on the detected plane to place a cube on it. This uses the `ARRaycastManager` to perform a raycast against the plane. If the plane is in `TrackingState.Limited`, it will highlight red. In the case of ARCore, this means that raycasting will not be available until the plane is in `TrackingState.Tracking` again.

| Action | Meaning |
| ------ | ------- |
| Pause  | Pauses the ARSession, meaning device tracking and trackable detection (e.g., plane detection) is temporarily paused. While paused, the ARSession does not consume CPU resources. |
| Resume | Resumes a paused ARSession. The device will attempt to relocalize and previously detected objects may shift around as tracking is reestablished. |
| Reset | Clears all detected trackables and effectively begins a new ARSession. |
| Reload | Completely destroys the ARSession GameObject and re-instantiates it. This simulates the behavior you might experience during scene switching. |

## Camera

### CPU Images

This samples shows how to acquire and manipulate textures obtained from AR Foundation on the CPU. Most textures in ARFoundation (e.g., the pass-through video supplied by the `ARCameraManager`, and the human depth and human stencil buffers provided by the `AROcclusionManager`) are GPU textures. Computer vision or other CPU-based applications often require the pixel buffers on the CPU, which would normally involve an expensive GPU readback. AR Foundation provides an API for obtaining these textures on the CPU for further processing, without incurring the costly GPU readback.

The relevant script is [`CpuImageSample.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/CpuImageSample.cs).

The resolution of the camera image is affected by the camera's configuration. The current configuration is indicated at the bottom left of the screen inside a dropdown box which lets you select one of the supported camera configurations. The [`CameraConfigController.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/CameraConfigController.cs) demonstrates enumerating and selecting a camera configuration. It is on the `CameraConfigs` GameObject.

Where available (currently iOS 13+ only), the human depth and human stencil textures are also available on the CPU. These appear inside two additional boxes underneath the camera's image.

### Basic Light Estimation

Demonstrates basic light estimation information from the camera frame. You should see values for "Ambient Intensity" and "Ambient Color" on screen. The relevant script is [`BasicLightEstimation.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/BasicLightEstimation.cs) script.

### HDR Light Estimation

This sample attempts to read HDR lighting information. You should see values for "Ambient Intensity", "Ambient Color", "Main Light Direction", "Main Light Intensity Lumens", "Main Light Color", and "Spherical Harmonics". Most devices only support a subset of these 6, so some will be listed as "Unavailable." The relevant script is [`HDRLightEstimation.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/HDRLightEstimation.cs) script.

On iOS, this is only available when face tracking is enabled and requires a device that supports face tracking (such as an iPhone X, XS or 11). When available, a virtual arrow appears in front of the camera which indicates the estimated main light direction. The virtual light direction is also updated, so that virtual content appears to be lit from the direction of the real light source.

When using `HDRLightEstimation`, the sample will automatically pick the supported camera facing direction for you, for example `World` on Android and `User` on iOS, so it does not matter which facing direction you select in the `ARCameraManager` component.

### Background Rendering Order

Produces a visual example of how changing the background rendering between `BeforeOpaqueGeometry` and `AfterOpaqueGeometry` would effect a rudimentary AR application. Leverages Occlusion where available to display `AfterOpaqueGeometry` support for AR Occlusion.

### Camera Grain (ARKit)

This sample demonstrates the camera grain effect. Once a plane is detected, you can place a cube on it with a material that simulates the camera grain noise in the camera feed. See the `CameraGrain.cs` script. Also see  `CameraGrain.shader` which animates and applies the camera grain texture (through linear interpolation) in screenspace.

This sample requires a device running iOS 13 or later and Unity 2020.2 or later.

## Plane Detection 

### Feathered Planes

This sample demonstrates basic plane detection, but uses a better looking prefab for the `ARPlane`. Rather than being drawn as exactly defined, the plane fades out towards the edges.

### Toggle Plane Detection

This sample shows how to toggle plane detection on and off. When off, it will also hide all previously detected planes by disabling their GameObjects. See [`PlaneDetectionController.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/PlaneDetectionController.cs).

### Plane Classification

This sample shows how to query for a plane's classification. Some devices attempt to classify planes into categories such as "door", "seat", "window", and "floor". This scene enables plane detection using the `ARPlaneManager`, and uses a prefab which includes a component which displays the plane's classification, or "none" if it cannot be classified.

### Plane Masking

This sample demonstrates basic plane detection, but uses an occlusion shader for the plane's material. This makes the plane appear invisible, but virtual objects behind the plane are culled. This provides an additional level of realism when, for example, placing objects on a table.

Move the device around until a plane is detected (its edges are still drawn) and then tap on the plane to place/move content.

## Image Tracking

There are two samples demonstrating image tracking. The image tracking samples are supported on ARCore and ARKit. To enable image tracking, you must first create an `XRReferenceImageLibrary`. This is the set of images to look for in the environment. [Click here](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@5.0/manual/arsubsystems/image-tracking.html) for instructions on creating one.

You can also add images to the reference image library at runtime. This sample includes a button that adds the images `one.png` and `two.png` to the reference image library. See the script `DynamicLibrary.cs` for example code.

Run the sample on an ARCore or ARKit-capable device and point your device at one of the images in [`Assets/Scenes/ImageTracking/Images`](https://github.com/Unity-Technologies/arfoundation-samples/tree/master/Assets/Scenes/ImageTracking/Images). They can be displayed on a computer monitor; they do not need to be printed out.

### Basic Image Tracking

At runtime, ARFoundation will generate an `ARTrackedImage` for each detected reference image. This sample uses the [`TrackedImageInfoManager.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scenes/ImageTracking/BasicImageTracking/TrackedImageInfoManager.cs) script to overlay the original image on top of the detected image, along with some meta data.

### Image Tracking With Multiple Prefabs

With [`PrefabImagePairManager.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scenes/ImageTracking/ImageTrackingWithMultiplePrefabs/PrefabImagePairManager.cs) script, you can assign different prefabs for each image in the reference image library.

You can also change prefabs at runtime. This sample includes a button that switch between the original and alternative prefab for the first image in the reference image library. See the script [`DynamicPrefab.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scenes/ImageTracking/ImageTrackingWithMultiplePrefabs/DynamicPrefab.cs) for example code.

## Object Tracking

Similar to the image tracking sample, this sample detects a 3D object from a set of reference objects in an `XRReferenceObjectLibrary`. [Click here](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@5.0/manual/arsubsystems/object-tracking.html) for instructions on creating one.

To use this sample, you must have a physical object the device can recognize. The sample's reference object library is built using two reference objects. The sample includes [printable templates](https://github.com/Unity-Technologies/arfoundation-samples/tree/master/Assets/Scenes/Object%20Tracking/Printable%20Templates) which can be printed on 8.5x11 inch paper and folded into a cube and cylinder.

Alternatively, you can [scan your own objects](https://developer.apple.com/documentation/arkit/scanning_and_detecting_3d_objects) and add them to the reference object library.

This sample requires iOS 12 or above.

## Face Tracking

There are several samples showing different face tracking features. Some are ARCore specific and some are ARKit specific.

### Face Pose

This is the simplest face tracking sample and simply draws an axis at the detected face's pose.

This sample uses the front-facing (i.e., selfie) camera.

### Face Mesh

This sample instantiates and updates a mesh representing the detected face. Information about the device support (e.g., number of faces that can be simultaneously tracked) is displayed on the screen.

This sample uses the front-facing (i.e., selfie) camera.

### Face Regions (ARCore)

"Face regions" are an ARCore-specific feature which provides pose information for specific "regions" on the detected face, e.g., left eyebrow. In this example, axes are drawn at each face region. See the [`ARCoreFaceRegionManager.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/ARCoreFaceRegionManager.cs).

This sample uses the front-facing (i.e., selfie) camera.

### Blend Shapes (ARKit)

"Blend shapes" are an ARKit-specific feature which provides information about various facial features on a scale of 0..1. For instance, "wink" and "frown". In this sample, blend shapes are used to puppet a cartoon face which is displayed over the detected face. See the [`ARKitBlendShapeVisualizer.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/ARKitBlendShapeVisualizer.cs).

This sample uses the front-facing (i.e., selfie) camera.

### Eye Lasers, Eye Poses, and Fixation Point (ARKit)

These samples demonstrate eye and fixation point tracking. Eye tracking produces a pose (position and rotation) for each eye in the detected face, and the "fixation point" is the point the face is looking at (i.e., fixated upon). `EyeLasers` uses the eye pose to draw laser beams emitted from the detected face.

This sample uses the front-facing (i.e., selfie) camera and requires an iOS device with a TrueDepth camera.

### Rear Camera (ARKit)

iOS 13 adds support for face tracking while the world-facing (i.e., rear) camera is active. This means the user-facing (i.e., front) camera is used for face tracking, but the pass through video uses the world-facing camera. To enable this mode in ARFoundation, you must enable an `ARFaceManager`, set the `ARSession` tracking mode to "Position and Rotation" or "Don't Care", and set the `ARCameraManager`'s facing direction to "World". Tap the screen to toggle between the user-facing and world-facing cameras.

The sample code in `DisplayFaceInfo.OnEnable` shows how to detect support for these face tracking features.

When using the world-facing camera, a cube is displayed in front of the camera whose orientation is driven by the face in front of the user-facing camera.

This feature requires a device with a TrueDepth camera and an A12 bionic chip running iOS 13.

## Body Tracking

### Body Tracking 2D

This sample demonstrates 2D screen space body tracking. A 2D skeleton is generated when a person is detected. See the [`ScreenSpaceJointVisualizer.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/ScreenSpaceJointVisualizer.cs) script.

This sample requires a device with an A12 bionic chip running iOS 13 or above.

### Body Tracking 3D

This sample demonstrates 3D world space body tracking. A 3D skeleton is generated when a person is detected. See the [`HumanBodyTracker.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/HumanBodyTracker.cs) script.

This sample requires a device with an A12 bionic chip running iOS 13 or above.

## Point Clouds

This sample shows all feature points over time, not just the current frame's feature points as the "AR Default Point Cloud" prefab does. It does this by using a slightly modified version of the `ARPointCloudParticleVisualizer` component that stores all the feature points in a Dictionary. Since each feature point has a unique identifier, it can look up the stored point and update its position in the dictionary if it already exists. This can be a useful starting point for custom solutions that require the entire map of point cloud points, e.g., for custom mesh reconstruction techniques.

This sample has two UI components:
* A button in the lower left which allows you to switch between visualizing "All" the points and just those in the "Current Frame".
* Text in the upper right which displays the number of points in each point cloud (ARCore & ARKit will only ever have one).

## Anchors

This sample shows how to create anchors as the result of a raycast hit. The "Clear Anchors" button removes all created anchors. See the [`AnchorCreator.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/AnchorCreator.cs) script.

This script can create two kinds of anchors:
1. If a feature point is hit, it creates a normal anchor at the hit pose using the `GameObject.AddComponent<ARAnchor>()` method.
1. If a plane is hit, it creates an anchor "attached" to the plane using the [AttachAnchor](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@5.0/api/UnityEngine.XR.ARFoundation.ARAnchorManager.html#UnityEngine_XR_ARFoundation_ARAnchorManager_AttachAnchor_UnityEngine_XR_ARFoundation_ARPlane_Pose_) method.

## Meshing

These meshing scenes use features of some devices to construct meshes from scanned data of real world surfaces. These meshing scenes will not work on all devices.

For ARKit, this functionality requires at least iPadOS 13.4 running on a device with a LiDAR scanner.

### Classification Meshes

This scene demonstrates mesh classification functionality. With mesh classification enabled, each triangle in the mesh surface is identified as one of several surface types. This sample scene creates submeshes for each classification type and renders each mesh type with a different color.

This scene only works on ARKit.

### Normal Meshes

This scene renders an overlay on top of the real world scanned geometry illustrating the normal of the surface.

### Occlusion Meshes

At first, this scene may appear to be doing nothing. However, it is rendering a depth texture on top of the scene based on the real world geometry. This allows for the real world to occlude virtual content. The scene has a script on it that fires a red ball into the scene when you tap. You will see the occlusion working by firing the red balls into a space which you can then move the iPad camera behind some other real world object to see that the virtual red balls are occluded by the real world object.

## Environment Probes

This sample demonstrates environment probes, a feature which attempts to generate a 3D texture from the real environment and applies it to reflection probes in the scene. The scene includes several spheres which start out completely black, but will change to shiny spheres which reflect the real environment when possible.

## Occlusion

### SimpleOcclusion

This sample demonstrates occlusion of virtual content by real world content through the use of environment depth images on supported Android and iOS devices.

### Depth Images

This sample demonstrates raw texture depth images from different methods.
* Environment depth (certain Android devices and Apple devices with the LiDAR sensor)
* Human stencil (Apple devices with an A12 bionic chip (or later) running iOS 13 or later)
* Human depth (Apple devices with an A12 bionic chip (or later) running iOS 13 or later)

## Check Support

Demonstrates checking for AR support and logs the results to the screen. The relevant script is [`SupportChecker.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/SupportChecker.cs).

## Interaction

This sample scene demonstrates the functionality of the `XR Interaction Toolkit` package. In the scene, you are able to place a cube on a plane which you can translate, rotate and scale with gestures. See the [`XR Interaction Toolkit Documentation`](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.0/manual/index.html) for more details.

## Configuration Chooser

Demonstrates how to use the AR Foundation session's ConfigurationChooser to swap between rear and front-facing camera configurations.

## Debug Menu

The AR Foundation Debug Menu allows you to visualize trackables and configurations on device.

## ARKit

These samples are only available on iOS devices.

### Coaching Overlay

The coaching overlay is an ARKit-specific feature which will overlay a helpful UI guiding the user to perform certain actions to achieve some "goal", such as finding a horizontal plane.

The coaching overlay can be activated automatically or manually, and you can set its goal. In this sample, we've set the goal to be "Any plane", and for it to activate automatically. This will display a special UI on the screen until a plane is found. There is also a button to activate it manually.

The sample includes a MonoBehavior to define the settings of the coaching overlay. See [`ARKitCoachingOverlay.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scenes/ARKit/ARKitCoachingOverlay/ARKitCoachingOverlay.cs).

This sample also shows how to subscribe to ARKit session callbacks. See [CustomSessionDelegate](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scenes/ARKit/ARKitCoachingOverlay/CustomSessionDelegate.cs).

This sample requires iOS 13 or above.

### Thermal State

This sample contains the code required to query for an iOS device's thermal state so that the thermal state may be used with C# game code. This sample illustrates how the thermal state may be used to disable AR Foundation features to reduce the thermal state of the device.

### AR World Map

An `ARWorldMap` is an ARKit-specific feature which lets you save a scanned area. ARKit can optionally relocalize to a saved world map at a later time. This can be used to synchronize multiple devices to a common space, or for curated experiences specific to a location, such as a museum exhibition or other special installation. Read more about world maps [here](https://developer.apple.com/documentation/arkit/arworldmap). A world map will store most types of trackables, such as reference points and planes.

The [`ARWorldMapController.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/ARWorldMapController.cs) performs most of the logic in this sample.

This sample requires iOS 12 or above.

### Geo Anchors

[ARKit's ARGeoAnchors](https://developer.apple.com/documentation/arkit/argeoanchor?language=objc) are not yet supported by ARFoundation, but you can still access this feature with a bit of Objective-C. This sample uses a custom [ConfigurationChooser](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@5.0/api/UnityEngine.XR.ARSubsystems.ConfigurationChooser.html) to instruct the Apple ARKit XR Plug-in to use an [ARGeoTrackingConfiguration](https://developer.apple.com/documentation/arkit/argeotrackingconfiguration?language=objc).

This sample also shows how to interpret the [nativePtr](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@5.0/api/UnityEngine.XR.ARSubsystems.XRSessionSubsystem.html#UnityEngine_XR_ARSubsystems_XRSessionSubsystem_nativePtr) provided by the [XRSessionSubsystem](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@5.0/api/UnityEngine.XR.ARSubsystems.XRSessionSubsystem.html) as an ARKit [ARSession](https://developer.apple.com/documentation/arkit/arsession?language=objc) pointer.

This sample requires an iOS device running iOS 14.0 or later, an A12 chip or later, location services enabled, and cellular capability.

### AR Collaboration Data

Similar to an `ARWorldMap`, a "collaborative session" is an ARKit-specific feature which allows multiple devices to share session information in real time. Each device will periodically produce `ARCollaborationData` which should be sent to all other devices in the collaborative session. ARKit will share each participant's pose and all reference points. Other types of trackables, such as detected planes, are not shared.

See [`CollaborativeSession.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scenes/ARKit/ARCollaborationData/CollaborativeSession.cs). Note there are two types of collaboration data: "Critical" and "Optional". "Critical" data is available periodically and should be sent to all other devices reliably. "Optional" data is available nearly every frame and may be sent unreliably. Data marked as "optional" includes data about the device's location, which is why it is produced very frequently (i.e., every frame).

Note that ARKit's support for collaborative sessions does not include any networking; it is up to the developer to manage the connection and send data to other participants in the collaborative session. For this sample, we used Apple's [MultipeerConnectivity Framework](https://developer.apple.com/documentation/multipeerconnectivity). Our implementation can be found [here](https://github.com/Unity-Technologies/arfoundation-samples/tree/master/Assets/Scripts/Multipeer).

You can create reference points by tapping on the screen. Reference points are created when the tap results in a raycast which hits a point in the point cloud.

This sample requires iOS 13 or above.

### High Resolution CPU Image

This sample demonstrates high resolution CPU image capture on iOS 16 and newer. See the [High Resolution CPU Image](https://docs.unity3d.com/Packages/com.unity.xr.arkit@5.1/manual/arkit-camera.html#high-resolution-cpu-image) package documentation to learn more about this feature.

### Camera Exposure

This sample shows how to lock the device camera and set the camera exposure mode, duration, and ISO. See [CameraExposureController.cs](https://github.com/Unity-Technologies/arfoundation-samples/blob/main/Assets/Scenes/ARKit/CameraExposure/CameraExposureController.cs) for example code.

This sample requires iOS 16 or newer and a device with an ultra-wide camera.

## ARCore Record Session

This sample demonstrates the session recording and playback functionality available in ARCore. This feature allows you to record the sensor and camera telemetry during a live session, and then reply it at later time. When replayed, ARCore runs on the target device using the recorded telemetry rather than live data. See [ARCoreSessionRecorder.cs](https://github.com/Unity-Technologies/arfoundation-samples/blob/main/Assets/Scenes/ARCore/ARCoreSessionRecorder.cs) for example code.

## Additional demos

While no longer actively maintained, Unity has a separate [AR Foundation Demos](https://github.com/Unity-Technologies/arfoundation-demos) repository that contains some larger samples including localization, mesh placement, shadows, and user onboarding UX.

# Community and Feedback

The intention of this reposititory is to provide a means for getting started with the features in AR Foundation. The samples are intentionally simplistic with a focus on teaching basic scene setup and APIs. If you you have a question, find a bug, or would like to request a new feature concerning any of the AR Foundation packages or these samples please [submit a GitHub issue](https://github.com/Unity-Technologies/arfoundation-samples/issues). New issues are reviewed regularly.

## Contributions and Pull Requests

We are not accepting pull requests at this time. If you find an issue with the samples, or would like to request a new sample, please [submit a GitHub issue](https://github.com/Unity-Technologies/arfoundation-samples/issues).
