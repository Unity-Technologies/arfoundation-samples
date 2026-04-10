using System;

#if OPENXR_1_6_OR_NEWER
using UnityEngine.XR.OpenXR;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// Enumeration of supported XR Platforms for OpenXR.
    /// </summary>
    public enum XRPlatformType
    {
        /// <summary>
        /// Meta Quest devices supported through OpenXR.
        /// </summary>
        [InspectorName("OpenXR: Meta")]
        OpenXRMeta,

        /// <summary>
        /// Android XR devices supported through OpenXR.
        /// </summary>
        [InspectorName("OpenXR: Android XR")]
        OpenXRAndroidXR,

        /// <summary>
        /// Other OpenXR devices.
        /// </summary>
        [InspectorName("OpenXR: Other")]
        OpenXROther,

        /// <summary>
        /// Other device that does not support OpenXR or not running on an OpenXR runtime.
        /// </summary>
        Other,
    }

    /// <summary>
    /// Helper class that determines the current XR platform based on the active runtime.
    /// Currently, this only supports OpenXR Runtimes from Meta and Google.
    /// </summary>
    public static class XRPlatformUnderstanding
    {
        const string k_RuntimeNameMeta = "Oculus";
        const string k_RuntimeNameAndroidXR = "Android XR";

        /// <summary>
        /// The current platform based on the OpenXR Runtime name.
        /// </summary>
        public static XRPlatformType CurrentPlatform
        {
            get
            {
                if (!s_Initialized)
                {
                    s_CurrentPlatform = GetCurrentXRPlatform();
                    s_Initialized = true;
                }
                return s_CurrentPlatform;
            }
        }

        static XRPlatformType s_CurrentPlatform = XRPlatformType.Other;

        static bool s_Initialized;

        /// <summary>
        /// Returns the current platform based on the active OpenXR Runtime name.
        /// </summary>
        /// <returns>The current platform based on the active OpenXR Runtime name.</returns>
        static XRPlatformType GetCurrentXRPlatform()
        {
            // If we have already initialized, just return the current platform
            if (s_Initialized)
                return s_CurrentPlatform;

#if OPENXR_1_6_OR_NEWER
            try
            {
                var openXRRuntimeName = OpenXRRuntime.name;
                if (string.IsNullOrEmpty(openXRRuntimeName))
                {
                    s_CurrentPlatform = XRPlatformType.Other;
                }
                else
                {
                    switch (openXRRuntimeName)
                    {
                        case k_RuntimeNameMeta:
                            Debug.Log("Meta runtime detected.");
                            s_CurrentPlatform = XRPlatformType.OpenXRMeta;
                            break;
                        case k_RuntimeNameAndroidXR:
                            Debug.Log("Android XR runtime detected.");
                            s_CurrentPlatform = XRPlatformType.OpenXRAndroidXR;
                            break;
                        default:
                            Debug.Log($"Unknown OpenXR runtime detected: \"{openXRRuntimeName}\"");
                            s_CurrentPlatform = XRPlatformType.OpenXROther;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to get OpenXR runtime: {e.Message}");
                s_CurrentPlatform = XRPlatformType.Other;
            }
#else
            s_CurrentPlatform = XRPlatformType.Other;
#endif

            s_Initialized = true;
            return s_CurrentPlatform;
        }
    }
}
