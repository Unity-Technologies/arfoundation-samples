# AR Foundation Samples
Example projects that utilize *AR Foundation* and demonstrates its functionality with sample assets and components.

Requires Unity version 2018.1+

Requires the AR Foundation package

## Instructions for installing AR Foundation

1. Download the latest version of Unity 2018.1 or 2018.2

2. Open Unity and load the project at the root of the *arfoundation-samples* repository

3. Open your choice of sample scenes.

4. See the documentation for the AR Foundation for usage instructions and more information
See the [AR Foundation Documentation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@1.0/manual/index.html) to learn how to get started with the core components.

## Sample Scenes

Find some sample scenes in the "Scenes" folder. They demonstrate different ways you can use ARFoundation.

### SampleScene
Demonstrates the basic ARFoudation functionality.

This sample will:
- Display the AR point cloud
- Find and create planes
- When a plane is tapped it will place a simple cube on the plane

### ARCamerabackgroundLuma
Demonstrates how to capture the AR camera background to a RenderTexture and then extract the pixels from that texture.

In this sample:
- The RenderTexture is displayed on a UI with a green tint
- Two different lumas calculated and displayed on the UI
