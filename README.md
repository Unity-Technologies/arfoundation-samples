# AR Foundation Samples
Example projects that use *AR Foundation 2.2* and demonstrate its functionality with sample assets and components.

This set of samples relies on four Unity packages:

* ARSubsystems
* ARCore XR Plugin
* ARKit XR Plugin
* ARFoundation

ARSubsystems defines an interface, and the platform-specific implementations are in the ARCore and ARKit packages. ARFoundation turns the AR data provided by ARSubsystems into Unity `GameObject`s and `MonoBehavour`s.

The `mater` branch is compatible with Unity 2019.1 and later. For 2018.4, see the [1.5-preview branch](https://github.com/Unity-Technologies/arfoundation-samples/tree/1.5-preview). ARFoundation 1.5 is functionality equivalent to 2.2. The only difference is the version of Unity on which it depends.

## :warning: ARKit 3 Support
The `master` branch includes support for ARKit 3, which is still in beta. While ARFoundation and ARSubsystems 2.2 _support_ ARKit 3, only Unity's ARKit XR Plugin 2.2 package contains _support_ for these features and _requires_ Xcode 11 beta and iOS 13 beta. Unity's ARKit XR Plugin 2.2 is not backwards compatible with previous versions of Xcode or iOS.

This table shows the latest version of the ARKit plugin and their Xcode and iOS compatibility:

|ARKit XR Plugin|Xcode Version|iOS Version|
|---------------|-----|---|
|2.2            |11 beta |13 beta|
|2.1            |9 or 10| 11 or 12|

This distinciton is temporary. Once iOS 13 is no longer in beta, the ARKit package is expected to work with all versions of Xcode 9+ and iOS 11+.

#### Which branch should I use?

If you want to checkout the latest and greatest features from Apple, use this `master` branch, Xcode 11 beta, and a device running iOS 13 beta. Otherwise, see the [2.1 branch](https://github.com/Unity-Technologies/arfoundation-samples/tree/2.1), which only lacks support for the new ARKit 3 features.

## Instructions for installing AR Foundation

1. Download the latest version of Unity 2019.1 or later.

2. Open Unity, and load the project at the root of the *arfoundation-samples* repository.

3. Open your choice of sample scene.

4. See the [AR Foundation Documentation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@latest?preview=1) for usage instructions and more information.
