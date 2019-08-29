# AR Foundation Samples
Example projects that use [*AR Foundation 3.0*](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@3.0/manual/index.html) and demonstrate its functionality with sample assets and components.

This set of samples relies on five Unity packages:

* ARSubsystems
* ARCore XR Plugin
* ARKit XR Plugin
* ARKit Face Tracking
* ARFoundation

ARSubsystems defines an interface, and the platform-specific implementations are in the ARCore and ARKit packages. ARFoundation turns the AR data provided by ARSubsystems into Unity `GameObject`s and `MonoBehavour`s.

The `master` branch is compatible with Unity 2019.2 and later. For 2018.4, see the [1.5-preview branch](https://github.com/Unity-Technologies/arfoundation-samples/tree/1.5-preview). ARFoundation 1.5 is functionality equivalent to 2.2. The only difference is the version of Unity on which it depends.

## ARKit 3 Support

The ARKit XR Plugin and ARKit Face Tacking packages support both ARKit 2 and ARKit 3 simultaneously. We supply separate libraries and select the appropriate one based on the version of Xcode selected in your Build Settings. This should eliminate the confusion over which package version is compatible with which Xcode version.

The ARKit 3 features require Xcode 11 beta 7 and iOS/iPadOS 13.1 beta.

## Instructions for installing AR Foundation

1. Download the latest version of Unity 2019.2 or later.

2. Open Unity, and load the project at the root of the *arfoundation-samples* repository.

3. Open your choice of sample scene.

4. See the [AR Foundation Documentation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@latest?preview=1) for usage instructions and more information.
