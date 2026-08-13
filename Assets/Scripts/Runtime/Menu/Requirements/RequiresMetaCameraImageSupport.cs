using System;
using System.Collections.Generic;
#if METAOPENXR_2_4_0_OR_NEWER && UNITY_ANDROID
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.Meta;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Requirement that the OpenXR Meta Quest Camera (Passthrough) feature is enabled and the
    /// "Camera Image Support" (passthrough camera) setting is enabled. Use this for sample scenes
    /// that need CPU or GPU camera images, e.g. MetaCpuImage and MetaGpuImage.
    /// </summary>
    [Serializable]
    public class RequiresMetaCameraImageSupport : ISceneRequirement
    {
        public virtual void Evaluate(List<RequirementResult> results)
        {
#if METAOPENXR_2_4_0_OR_NEWER && UNITY_ANDROID
            var feature = OpenXRSettings.Instance.GetFeature<ARCameraFeature>();
            var isSupported = feature != null && feature.enabled && feature.cameraImageSupportEnabled;
            results.Add(new RequirementResult(
                isSupported,
                "Meta Camera Image Support",
                "Ensure <b>Meta Quest: Camera (Passthrough)</b> is enabled in <b>Project Settings</b> > <b>XR Plug-in Management</b> > <b>OpenXR</b> and that <b>Camera Image Support</b> is enabled in its feature settings."));
#else
            results.Add(new RequirementResult(
                false,
                "Meta Camera Image Support",
                "Ensure <b>Meta Quest: Camera (Passthrough)</b> is enabled in <b>Project Settings</b> > <b>XR Plug-in Management</b> > <b>OpenXR</b> and that <b>Camera Image Support</b> is enabled in its feature settings."));
#endif
        }
    }
}
