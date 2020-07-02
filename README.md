# AR Foundation Samples

Example projects that use [*AR Foundation 4.1*](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/manual/index.html) and demonstrate its functionality with sample assets and components.

This set of samples relies on five Unity packages:

* ARSubsystems ([documentation](https://docs.unity3d.com/Packages/com.unity.xr.arsubsystems@4.1/manual/index.html))
* ARCore XR Plugin ([documentation](https://docs.unity3d.com/Packages/com.unity.xr.arcore@4.1/manual/index.html))
* ARKit XR Plugin ([documentation](https://docs.unity3d.com/Packages/com.unity.xr.arkit@4.1/manual/index.html))
* ARKit Face Tracking ([documentation](https://docs.unity3d.com/Packages/com.unity.xr.arkit-face-tracking@4.1/manual/index.html))
* ARFoundation ([documentation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/manual/index.html))

## What version should I use?

A Unity package is either "Preview" or "Verified". The latest version of ARFoundation is usually marked as preview and may include experimental or unstable features. A "verified" package is developed targeting a specific version of Unity (though it may work with earlier version as well). All packages verified for the same version of Unity are known to work well together.

In ARFoundation, this means:

| Unity Version | ARFoundation Version |
| ------------- | -------------------- |
|    2018.4     | [1.5 (preview)](https://github.com/Unity-Technologies/arfoundation-samples/tree/1.5-preview)  |
|    2019.3     | [2.1 (verified)](https://github.com/Unity-Technologies/arfoundation-samples/tree/2.1)         |
|    2020.1     | 3.0 (verified)                                                                                |
|    2020.2     | 4.1 (preview)                                                                                 |

## ARSubsystems

ARFoundation is built on "[subsystems](https://docs.unity3d.com/2019.3/Documentation/ScriptReference/Subsystem.html)" and depends on a separate package called [ARSubsystems](https://docs.unity3d.com/Packages/com.unity.xr.arsubsystems@4.1/manual/index.html). ARSubsystems defines an interface, and the platform-specific implementations are in the [ARCore](https://docs.unity3d.com/Packages/com.unity.xr.arcore@4.1/manual/index.html) and [ARKit](https://docs.unity3d.com/Packages/com.unity.xr.arkit@4.1/manual/index.html) packages. ARFoundation turns the AR data provided by ARSubsystems into Unity `GameObject`s and `MonoBehavour`s.

The `master` branch is compatible with Unity 2019.3 and later. For 2018.4, see the [1.5-preview branch](https://github.com/Unity-Technologies/arfoundation-samples/tree/1.5-preview).

## Why is ARKit Face Tracking a separate package?

For privacy reasons, use of ARKit's face tracking feature requires additional validation in order to publish your app on the App Store. If your application binary contains certain face tracking related symbols, your app may fail validation. For this reason, we provide this feature as a separate package which must be explicitly included.

## Instructions for installing AR Foundation

1. Download the latest version of Unity 2019.3 or later.

2. Open Unity, and load the project at the root of the *arfoundation-samples* repository.

3. Open your choice of sample scene.

4. See the [AR Foundation Documentation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/manual/index.html) for usage instructions and more information.

# Samples

## SimpleAR

This is a good starting sample that enables point cloud visualization and plane detection. There are buttons on screen that let you pause, resume, reset, and reload the ARSession.

When a plane is detected, you can tap on the detected plane to place a cube on it. This uses the `ARRaycastManager` to perform a raycast against the plane.

| Action | Meaning |
| ------ | ------- |
| Pause  | Pauses the ARSession, meaning device tracking and trackable detection (e.g., plane detection) is temporarily paused. While paused, the ARSession does not consume CPU resources. |
| Resume | Resumes a paused ARSession. The device will attempt to relocalize and previously detected objects may shift around as tracking is reestablished. |
| Reset | Clears all detected trackables and effectively begins a new ARSession. |
| Reload | Completely destroys the ARSession GameObject and re-instantiates it. This simulates the behavior you might experience during scene switching. |

## Check Support

Demonstrates checking for AR support and logs the results to the screen. The relevant script is [`SupportChecker.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/SupportChecker.cs).

## LightEstimation

Demonstrates light estimation information from the camera frame. You should see values for "Brightness", "Color Temp", and "Color Correct" on screen. Most devices only support a subset of these 3, so some will be listed as "unavailable."

This sample also attempts to read HDR lighting information. On iOS, this is only available when face tracking is enabled and requires a device with a TrueDepth camera (such as an iPhone X, XS or 11). When available, a virtual arrow appears in front of the camera which indicates the estimated main light direction. The virtual light direction is also updated, so that virtual content appears to be lit from the direction of the real light source.

The relevant scripts are on the "Directional Light" GameObject.

## Anchors

This sample shows how to create anchors as the result of a raycast hit. The "Clear Anchors" button removes all created anchors. See the [`AnchorCreator.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/AnchorCreator.cs) script.

## Scale

This sample shows how to adjust the apparent scale of content in an AR scene. It does this by moving, rotating, and scaling the `ARSessionOrigin` instead of the content. Complex scenes often can't be moved after creation (e.g., terrain), and scale can negatively affect other systems such as physics, particle effects, and AI navigation. The `ARSessionOrigin`'s scale feature is useful if you want to make your content "appear" at a position on a detected plane and to scale, for example, a building sized object to a table-top miniature.

To use this sample, first move the device around until a plane is detected, then tap on the plane. Content will appear at the touch point. After the content is placed, you can adjust its rotation and scale using the on-screen sliders. Note that the content itself is never moved, rotated, or scaled.

The relevant script is [`MakeAppearOnPlane.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/MakeAppearOnPlane.cs).

## CpuImages

This samples shows how to acquire and manipulate textures obtained from ARFoundation on the CPU. Most textures in ARFoundation (e.g., the pass-through video supplied by the `ARCameraManager`, and the human depth and human stencil buffers provided by the `AROcclusionManager`) are GPU textures. Computer vision or other CPU-based applications often require the pixel buffers on the CPU, which would normally involve an expensive GPU readback. ARFoundation provides an API for obtaining these textures on the CPU for further processing, without incurring the costly GPU readback.

The relevant script is [`CpuImageSample.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/CpuImageSample.cs).

The resolution of the camera image is affected by the camera's configuration. The current configuration is indicated at the bottom left of the screen inside a dropdown box which lets you select one of the supported camera configurations. The [`CameraConfigController.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/CameraConfigController.cs) demonstrates enumerating and selecting a camera configuration. It is on the `CameraConfigs` GameObject.

Where available (currently iOS 13+ only), the human depth and human stencil textures are also available on the CPU. These appear inside two additional boxes underneath the camera's image.

## TogglePlaneDetection

This sample shows how to toggle plane detection on and off. When off, it will also hide all previously detected planes by disabling their GameObjects. See [`PlaneDetectionController.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/PlaneDetectionController.cs).

## PlaneClassification

This sample shows how to query for a plane's classification. Some devices attempt to classify planes into categories such as "door", "seat", "window", and "floor". This scene enables plane detection using the `ARPlaneManager`, and uses a prefab which includes a component which displays the plane's classification, or "none" if it cannot be classified.

## FeatheredPlanes

This sample demonstrates basic plane detection, but uses a better looking prefab for the `ARPlane`. Rather than being drawn as exactly defined, the plane fades out towards the edges.

## PlaneOcclusion

This sample demonstrates basic plane detection, but uses an occlusion shader for the plane's material. This makes the plane appear invisible, but virtual objects behind the plane are culled. This provides an additional level of realism when, for example, placing objects on a table.

Move the device around until a plane is detected (its edges are still drawn) and then tap on the plane to place/move content.

## UX

This sample demonstrates some UI that may be useful when guiding new users through an AR application. It uses the script [`UIManager.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/UX/UIManager.cs) to trigger different UI animations based on events (e.g., a plane being detected).

The functionality demonstrated here is conceptually similar to the `ARKitCoachingOverlay` sample.

## EnvironmentProbes

This sample demonstrates environment probes, a feature which attempts to generate a 3D texture from the real environment and applies it to reflection probes in the scene. The scene includes several spheres which start out completely black, but will change to shiny spheres which reflect the real environment when possible.

## ARWorldMap

An `ARWorldMap` is an ARKit-specific feature which lets you save a scanned area. ARKit can optionally relocalize to a saved world map at a later time. This can be used to synchronize multiple devices to a common space, or for curated experiences specific to a location, such as a museum exhibition or other special installation. Read more about world maps [here](https://developer.apple.com/documentation/arkit/arworldmap). A world map will store most types of trackables, such as reference points and planes.

The [`ARWorldMapController.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/ARWorldMapController.cs) performs most of the logic in this sample.

This sample requires iOS 12.

## ARCollaborationData

Similar to an `ARWorldMap`, a "collaborative session" is an ARKit-specific feature which allows multiple devices to share session information in real time. Each device will periodically produce `ARCollaborationData` which should be sent to all other devices in the collaborative session. ARKit will share each participant's pose and all reference points. Other types of trackables, such as detected planes, are not shared.

See [`CollaborativeSession.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scenes/ARCollaborationData/CollaborativeSession.cs). Note there are two types of collaboration data: "Critical" and "Optional". "Critical" data is available periodically and should be sent to all other devices reliably. "Optional" data is available nearly every frame and may be sent unreliably. Data marked as "optional" includes data about the device's location, which is why it is produced very frequently (i.e., every frame).

Note that ARKit's support for collaborative sessions does not include any networking; it is up to the developer to manage the connection and send data to other participants in the collaborative session. For this sample, we used Apple's [MultipeerConnectivity Framework](https://developer.apple.com/documentation/multipeerconnectivity). Our implementation can be found [here](https://github.com/Unity-Technologies/arfoundation-samples/tree/master/Assets/Scripts/Multipeer).

You can create reference points by tapping on the screen. Reference points are created when the tap results in a raycast which hits a point in the point cloud.

This sample requires iOS 13.

## ARKitCoachingOverlay

The coaching overlay is an ARKit-specific feature which will overlay a helpful UI guiding the user to perform certain actions to achieve some "goal", such as finding a horizontal plane.

The coaching overlay can be activated automatically or manually, and you can set its goal. In this sample, we've set the goal to be "Any plane", and for it to activate automatically. This will display a special UI on the screen until a plane is found. There is also a button to activate it manually.

The sample includes a MonoBehavior to define the settings of the coaching overlay. See [`ARKitCoachingOverlay.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scenes/ARKitCoachingOverlay/ARKitCoachingOverlay.cs).

This sample requires iOS 13.

## ImageTracking

This sample demonstrates image tracking. Image tracking is supported on ARCore and ARKit. To enable image tracking, you must first create an `XRReferenceImageLibrary`. This is the set of images to look for in the environment. [Click here](https://docs.unity3d.com/Packages/com.unity.xr.arsubsystems@4.1/manual/image-tracking.html) for instructions on creating one.

You can also add images to the reference image library at runtime. This sample includes a button that adds the images `one.png` and `two.png` to the reference image library. See the script `DynamicLibrary.cs` for example code.

At runtime, ARFoundation will generate an `ARTrackedImage` for each detected reference image. This sample uses the [`TrackedImageInfoManager.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scenes/ImageTracking/TrackedImageInfoManager.cs) script to overlay the original image on top of the detected image, along with some meta data.

Run the sample on an ARCore or ARKit-capable device and point your device at one of the images in [`Assets/Scenes/ImageTracking/Images`](https://github.com/Unity-Technologies/arfoundation-samples/tree/master/Assets/Scenes/ImageTracking/Images). They can be displayed on a computer monitor; they do not need to be printed out.

## ObjectTracking

Similar to the image tracking sample, this sample detects a 3D object from a set of reference objects in an `XRReferenceObjectLibrary`. [Click here](https://docs.unity3d.com/Packages/com.unity.xr.arsubsystems@4.1/manual/object-tracking.html) for instructions on creating one.

To use this sample, you must have a physical object the device can recognize. The sample's reference object library is built using two reference objects. The sample includes [printable templates](https://github.com/Unity-Technologies/arfoundation-samples/tree/master/Assets/Scenes/Object%20Tracking/Printable%20Templates) which can be printed on 8.5x11 inch paper and folded into a cube and cylinder.

Alternatively, you can [scan your own objects](https://developer.apple.com/documentation/arkit/scanning_and_detecting_3d_objects) and add them to the reference object library.

This sample requires iOS 12 and is not supported on Android.

## Face Tracking

There are several samples showing different face tracking features. Some are ARCore specific and some are ARKit specific.

### FacePose

This is the simplest face tracking sample and simply draws an axis at the detected face's pose.

This sample uses the front-facing (i.e., selfie) camera.

### FaceMesh

This sample instantiates and updates a mesh representing the detected face. Information about the device support (e.g., number of faces that can be simultaneously tracked) is displayed on the screen.

This sample uses the front-facing (i.e., selfie) camera.

### ARKitFaceBlendShapes

"Blend shapes" are an ARKit-specific feature which provides information about various facial features on a scale of 0..1. For instance, "wink" and "frown". In this sample, blend shapes are used to puppet a cartoon face which is displayed over the detected face. See the [`ARKitBlendShapeVisualizer.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/ARKitBlendShapeVisualizer.cs).

This sample uses the front-facing (i.e., selfie) camera.

### ARCoreFaceRegions

"Face regions" are an ARCore-specific feature which provides pose information for specific "regions" on the detected face, e.g., left eyebrow. In this example, axes are drawn at each face region. See the [`ARCoreFaceRegionManager.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/ARCoreFaceRegionManager.cs).

This sample uses the front-facing (i.e., selfie) camera.

### EyeLasers, EyePoses, FixationPoint

These samples demonstrate eye and fixation point tracking. Eye tracking produces a pose (position and rotation) for each eye in the detected face, and the "fixation point" is the point the face is looking at (i.e., fixated upon). `EyeLasers` uses the eye pose to draw laser beams emitted from the detected face.

This sample uses the front-facing (i.e., selfie) camera and requires an iOS device with a TrueDepth camera.

### WorldCameraWithUserFacingFaceTracking

iOS 13 adds support for face tracking while the world-facing (i.e., rear) camera is active. This means the user-facing (i.e., front) camera is used for face tracking, but the pass through video uses the world-facing camera. To enable this mode in ARFoundation, you must enable an `ARFaceManager`, set the `ARSession` tracking mode to "Position and Rotation" or "Don't Care", and set the `ARCameraManager`'s facing direction to "World". Tap the screen to toggle between the user-facing and world-facing cameras.

The sample code in `DisplayFaceInfo.OnEnable` shows how to detect support for these face tracking features.

When using the world-facing camera, a cube is displayed in front of the camera whose orientation is driven by the face in front of the user-facing camera.

This feature requires a device with a TrueDepth camera and an A12 bionic chip running iOS 13.

## HumanBodyTracking2D

This sample demonstrates 2D screen space body tracking. A 2D skeleton is generated when a person is detected. See the [`ScreenSpaceJointVisualizer.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/ScreenSpaceJointVisualizer.cs) script.

This sample requires a device with an A12 bionic chip running iOS 13.

## HumanBodyTracking3D

This sample demonstrates 3D world space body tracking. A 3D skeleton is generated when a person is detected. See the [`HumanBodyTracker.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/HumanBodyTracker.cs) script.

This sample requires a device with an A12 bionic chip running iOS 13.

## DepthImages

This sample demonstrates raw texture depth images from different methods.
* Environment depth (certain Android devices)
* Human stencil (Apple devices with an A12 bionic chip (or later) running iOS 13 or later)
* Human depth (Apple devices with an A12 bionic chip (or later) running iOS 13 or later)

## SimpleOcclusion

This sample demonstrates occlusion of virtual content by real world content through the use of environment depth images on supported Android devices.

## AllPointCloudPoints

This sample shows all feature points over time, not just the current frame's feature points as the "AR Default Point Cloud" prefab does. It does this by using a slightly modified version of the `ARPointCloudParticleVisualizer` component that stores all the feature points in a Dictionary. Since each feature point has a unique identifier, it can look up the stored point and update its position in the dictionary if it already exists. This can be a useful starting point for custom solutions that require the entire map of point cloud points, e.g., for custom mesh reconstruction techniques.

This sample has two UI components:
* A button in the lower left which allows you to switch between visualizing "All" the points and just those in the "Current Frame".
* Text in the upper right which displays the number of points in each point cloud (ARCore & ARKit will only ever have one).

## CameraGrain

This sample demonstrates the camera grain effect. Once a plane is detected, you can place a cube on it with a material that simulates the camera grain noise in the camera feed. See the `CameraGrain.cs` script. Also see  `CameraGrain.shader` which animates and applies the camera grain texture (through linear interpolation) in screenspace.

This sample requires a device running iOS 13 and Unity 2020.2 or later.

## Meshing

These meshing scenes use features of some devices to construct meshes from scanned data of real world surfaces. These meshing scenes will not work on all devices.

For ARKit, this functionality requires at least iPadOS 13.4 running on a device with a LiDAR scanner.

### ClassificationMeshes

This scene demonstrates mesh classification functionality. With mesh classification enabled, each triangle in the mesh surface is identified as one of several surface types. This sample scene creates submeshes for each classification type and renders each mesh type with a different color.

This scene only works on ARKit.

### NormalMeshes

This scene renders an overlay on top of the real world scanned geometry illustrating the normal of the surface.

### OcclusionMeshes

At first, this scene may appear to be doing nothing. However, it is rendering a depth texture on top of the scene based on the real world geometry. This allows for the real world to occlude virtual content. The scene has a script on it that fires a red ball into the scene when you tap. You will see the occlusion working by firing the red balls into a space which you can then move the iPad camera behind some other real world object to see that the virtual red balls are occluded by the real world object.

## Interaction

This sample scene demonstrates the functionality of the `XR Interaction Toolkit` package. In the scene, you are able to place a cube on a plane which you can translate, rotate and scale with gestures. See the [`XR Interaction Toolkit Documentation`](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@0.9/manual/index.html) for more details.

# Community and Feedback

The intention of this reposititory is to provide a means for getting started with the features in ARFoundation. The samples are intentionally simplistic with a focus on teaching basic scene setup and APIs. If you you have a question, find a bug, or would like to request a new feature concerning any of the ARFoundation packages or these samples please [submit a GitHub issue](https://github.com/Unity-Technologies/arfoundation-samples/issues). New issues are reviewed regularly.

## Contributions and Pull Requests

We are not accepting pull requests at this time. If you find an issue with the samples, our would like to request a new sample, please [submit a GitHub issue](https://github.com/Unity-Technologies/arfoundation-samples/issues).