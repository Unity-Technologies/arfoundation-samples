using System;
using System.Collections.Generic;
using Unity.Scripting.LifecycleManagement;
using Unity.XR.CoreUtils.Collections;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif // UNITY_ANDROID

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public partial class RequiresPermission : ISceneRequirement
    {
        [Serializable]
        public struct PlatformPermission
        {
            public DevicePlatform platform;
            public string permissionId;
        }

        [SerializeField, Tooltip("Each entry is only evaluated when running on the specified platform. Entries for other platforms are ignored.")]
        List<PlatformPermission> m_PermissionsPerPlatform = new();

#if UNITY_ANDROID
        [AutoStaticsCleanup]
        static readonly HashSet<string> s_RequestedPermissions = new();
#endif // UNITY_ANDROID

        public virtual void Evaluate(List<RequirementResult> results)
        {
            var activePlatform = DevicePlatformUtility.GetActiveDevicePlatform();

            foreach (var entry in m_PermissionsPerPlatform)
            {
                var requirementName = $"Permission ({entry.permissionId})";

                if (activePlatform != entry.platform)
                {
                    results.Add(new RequirementResult(true, requirementName));
                    continue;
                }

#if UNITY_ANDROID
                var remediationText =
                    $"The <b>{entry.permissionId}</b> permission has not been granted. Grant this permission in your " +
                    "device settings or reinstall the app and accept the permission dialog.";

                var isGranted = Permission.HasUserAuthorizedPermission(entry.permissionId);
                results.Add(new RequirementResult(isGranted, requirementName, remediationText));
#else
                results.Add(new RequirementResult(true, requirementName));
#endif // UNITY_ANDROID
            }
        }

        public static void RequestAllPending(ReadOnlyListSpan<SampleSceneDescriptor> descriptors, Action onComplete)
        {
#if UNITY_ANDROID
            var activePlatform = DevicePlatformUtility.GetActiveDevicePlatform();
            var permissionsToRequest = new List<string>();
            var nonPermissionResults = new List<RequirementResult>();

            foreach (var descriptor in descriptors)
            {
                var isOtherwiseSupported = true;
                foreach (var requirement in descriptor.requirements)
                {
                    if (requirement is RequiresPermission)
                        continue;

                    nonPermissionResults.Clear();
                    requirement.Evaluate(nonPermissionResults);
                    foreach (var result in nonPermissionResults)
                    {
                        if (!result.isSupported)
                        {
                            isOtherwiseSupported = false;
                            break;
                        }
                    }

                    if (!isOtherwiseSupported)
                        break;
                }

                if (!isOtherwiseSupported)
                    continue;

                foreach (var requirement in descriptor.requirements)
                {
                    if (requirement is not RequiresPermission permissionRequest)
                        continue;

                    foreach (var entry in permissionRequest.m_PermissionsPerPlatform)
                    {
                        if (entry.platform != activePlatform)
                            continue;

                        if (Permission.HasUserAuthorizedPermission(entry.permissionId))
                            continue;

                        if (!s_RequestedPermissions.Add(entry.permissionId))
                            continue;

                        permissionsToRequest.Add(entry.permissionId);
                    }
                }
            }

            if (permissionsToRequest.Count == 0)
            {
                onComplete?.Invoke();
                return;
            }

            var pendingCount = permissionsToRequest.Count;
            var callbacks = new PermissionCallbacks();
            callbacks.PermissionGranted += _ => OnRequestComplete(ref pendingCount, onComplete);
            callbacks.PermissionDenied += _ => OnRequestComplete(ref pendingCount, onComplete);
            Permission.RequestUserPermissions(permissionsToRequest.ToArray(), callbacks);
#else
            onComplete?.Invoke();
#endif // UNITY_ANDROID
        }

#if UNITY_ANDROID
        static void OnRequestComplete(ref int pendingCount, Action onComplete)
        {
            pendingCount--;
            if (pendingCount <= 0)
                onComplete?.Invoke();
        }
#endif // UNITY_ANDROID
    }
}
