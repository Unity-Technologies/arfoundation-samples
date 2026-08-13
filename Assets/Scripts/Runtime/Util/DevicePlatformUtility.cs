#if UNITY_ANDROID
using UnityEngine.XR.ARCore;
#endif
#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif
using UnityEngine.XR.OpenXR;
#if UNITY_EDITOR
using UnityEngine.XR.Simulation;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    public enum DevicePlatform
    {
        MetaQuest,
        AndroidXR,
        ARCore,
        iOS,
        XRSimulation,
    }

    public static class DevicePlatformUtility
    {
        static DevicePlatform? s_CachedOpenXRPlatform;

        public static DevicePlatform? GetActiveDevicePlatform()
        {
            var loader = LoaderUtility.GetActiveLoader();
            if (loader == null)
                return null;

            if (loader is OpenXRLoader)
                return GetOpenXRPlatform();

#if UNITY_ANDROID
            if (loader is ARCoreLoader)
                return DevicePlatform.ARCore;
#endif

#if UNITY_IOS
            if (loader is ARKitLoader)
                return DevicePlatform.iOS;
#endif

#if UNITY_EDITOR
            if (loader is SimulationLoader)
                return DevicePlatform.XRSimulation;
#endif

            return null;
        }

        static DevicePlatform? GetOpenXRPlatform()
        {
#if UNITY_META_QUEST
            return DevicePlatform.MetaQuest;
#elif UNITY_ANDROID_XR
            return DevicePlatform.AndroidXR;
#else
            if (s_CachedOpenXRPlatform.HasValue)
                return s_CachedOpenXRPlatform.Value;

            var runtime = OpenXRRuntime.name?.ToLower() ?? string.Empty;

            if (runtime.Contains("android xr"))
                s_CachedOpenXRPlatform = DevicePlatform.AndroidXR;
            else if (runtime.Contains("oculus") || runtime.Contains("meta"))
                s_CachedOpenXRPlatform = DevicePlatform.MetaQuest;
            else
                return null;

            return s_CachedOpenXRPlatform.Value;
#endif
        }
    }
}
