using Unity.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARFoundation.Samples.Runtime;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Meshing;
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif
#if OPENXR_1_18_0_1_OR_NEWER && XR_HANDS_1_9_0_1_OR_NEWER
using UnityEngine.XR.OpenXR.Features.Meta;
#endif
#if ANDROIDOPENXR_1_0_0_3_OR_NEWER && UNITY_ANDROID
using UnityEngine.XR.OpenXR.Features.Android;
#endif

namespace UnityEngine.XR.ARFoundation.Samples.Hands
{
    public class ARHandsRenderer : MonoBehaviour
    {
        [SerializeField]
        Material m_HandsMaterial;

        [SerializeField]
        ARShaderOcclusion m_ARShaderOcclusion;

        [SerializeField]
        XROrigin m_XROrigin;

        IARHandView m_LeftHand;
        IARHandView m_RightHand;

        XRHandSubsystem m_HandsSubsystem;

        async void Awake()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
#if UNITY_ANDROID_XR
            await RequestHandTrackingPermissionIfNeeded("android.permission.HAND_TRACKING");
#elif UNITY_META_QUEST
            await RequestHandTrackingPermissionIfNeeded("com.oculus.permission.HAND_TRACKING");
#endif

            if (this == null)
                return;
#endif

            SubsystemsUtility.TryGetLoadedSubsystem<XRSessionSubsystem, XRSessionSubsystem>(out var sessionSubsystem);

            if (sessionSubsystem == null)
            {
                Debug.LogError("Can't get XRSessionSubsystem. Hand views won't be created.");
                return;
            }

            var handViewFactory = GetHandsFactory(sessionSubsystem);

            if (handViewFactory == null)
            {
                return;
            }

            if (m_XROrigin == null)
                m_XROrigin = FindAnyObjectByType<XROrigin>();

            if (m_XROrigin == null)
            {
                Debug.LogError("XROrigin not found. Hand views won't be created.");
                return;
            }

            var xrOriginTransform = m_XROrigin.transform;
            m_LeftHand = handViewFactory.CreateHand("LeftHand", m_HandsMaterial, m_ARShaderOcclusion, xrOriginTransform);
            m_RightHand = handViewFactory.CreateHand("RightHand", m_HandsMaterial, m_ARShaderOcclusion, xrOriginTransform);

            if (m_LeftHand == null || m_RightHand == null)
            {
                Debug.LogError("Failed to create hand views. Hands won't be rendered.");
                return;
            }

            m_HandsSubsystem = await SubsystemsUtility.GetRunningSubsystem<XRHandSubsystem, XRHandSubsystem>();

            if (this == null)
                return;

            if (m_HandsSubsystem != null)
            {
                m_HandsSubsystem.updatedHands += OnHandsUpdated;
            }
        }

        void OnDestroy()
        {
            if (m_HandsSubsystem != null)
            {
                m_HandsSubsystem.updatedHands -= OnHandsUpdated;
            }

            m_LeftHand?.Dispose();
            m_RightHand?.Dispose();
        }

        static IARHandViewFactory GetHandsFactory(XRSessionSubsystem sessionSubsystem)
        {
            switch (sessionSubsystem.subsystemDescriptor.id)
            {
                case "Android-Session":
                    return GetAndroidXRHandsFactory();
                case "Meta-Session":
#if XR_HANDS_1_9_0_1_OR_NEWER
                    return new MetaHandViewFactory();
#else
                    Debug.LogWarning("Meta mesh support requires XR Hands 1.9.0-pre.1 or newer to use.");
                    return null;
#endif
                default:
                    Debug.LogError($"Hands renderer is not yet implemented for {sessionSubsystem.subsystemDescriptor.id}.");
                    return null;
            }
        }

        static IARHandViewFactory GetAndroidXRHandsFactory()
        {
#if OPENXR_1_18_0_1_OR_NEWER && XR_HANDS_1_9_0_1_OR_NEWER
            if (OpenXRUtility.IsOpenXRFeatureEnabled<MetaOpenXRHandMeshData>())
                return new MetaHandViewFactory();
#endif

#if ANDROIDOPENXR_1_0_0_3_OR_NEWER && UNITY_ANDROID
#pragma warning disable CS0618 // Type or member is obsolete
            if (OpenXRUtility.IsOpenXRFeatureEnabled<AndroidXRHandMeshData>())
                return new AndroidARHandViewFactory();
#pragma warning restore CS0618
#endif

            Debug.LogError("No hand mesh data feature is enabled. Enable MetaOpenXRHandMeshData or AndroidXRHandMeshData in OpenXR settings.");
            return null;
        }

        static void UpdateHand(
            XRHandSubsystem subsystem,
            XRHandSubsystem.UpdateSuccessFlags successFlags,
            XRHandSubsystem.UpdateSuccessFlags mask,
            IARHandView handView,
            in XRHandMeshData handMeshData)
        {
            if ((successFlags & mask) != 0)
            {
                handView.renderer.enabled = true;
                handView.Update(subsystem, handMeshData);
            }
            else
            {
                handView.renderer.enabled = false;
            }
        }

        void OnHandsUpdated(
            XRHandSubsystem subsystem,
            XRHandSubsystem.UpdateSuccessFlags successFlags,
            XRHandSubsystem.UpdateType type)
        {
            // XR_FB_hand_tracking_mesh data is immutable for the lifetime of the XrInstance.
            // Once both hands have their mesh data, skip the native call and just update poses.
            if (m_LeftHand.isInitialized && m_RightHand.isInitialized)
            {
                UpdateHandPoses(subsystem, successFlags,
                    XRHandSubsystem.UpdateSuccessFlags.LeftHandJoints | XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose,
                    m_LeftHand);
                UpdateHandPoses(subsystem, successFlags,
                    XRHandSubsystem.UpdateSuccessFlags.RightHandJoints | XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose,
                    m_RightHand);
                return;
            }

            var queryParams = new XRHandMeshDataQueryParams
            {
                allocator = Allocator.Temp
            };

            if (!subsystem.TryGetMeshData(out var meshDataQueryResult, ref queryParams))
            {
                return;
            }

            using (meshDataQueryResult)
            {
                var leftHandSuccessMask = XRHandSubsystem.UpdateSuccessFlags.LeftHandJoints |
                                           XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose;

                UpdateHand(subsystem, successFlags, leftHandSuccessMask, m_LeftHand, meshDataQueryResult.leftHand);

                var rightHandSuccessMask = XRHandSubsystem.UpdateSuccessFlags.RightHandJoints |
                                            XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose;

                UpdateHand(subsystem, successFlags, rightHandSuccessMask, m_RightHand, meshDataQueryResult.rightHand);
            }
        }

        static void UpdateHandPoses(
            XRHandSubsystem subsystem,
            XRHandSubsystem.UpdateSuccessFlags successFlags,
            XRHandSubsystem.UpdateSuccessFlags mask,
            IARHandView handView)
        {
            if ((successFlags & mask) != 0)
            {
                handView.renderer.enabled = true;
                handView.UpdatePoses(subsystem);
            }
            else
            {
                handView.renderer.enabled = false;
            }
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        static async Awaitable RequestHandTrackingPermissionIfNeeded(string permission)
        {
            if (Permission.HasUserAuthorizedPermission(permission))
                return;

            var callbacks = new PermissionCallbacks();
            bool responded = false;
            callbacks.PermissionGranted += _ => responded = true;
            callbacks.PermissionDenied += _ => responded = true;
            Permission.RequestUserPermission(permission, callbacks);

            while (!responded)
                await Awaitable.NextFrameAsync();
        }
#endif
    }
}
