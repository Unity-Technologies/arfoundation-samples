using System;
using System.Collections.Generic;
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
    [Serializable]
    public class RequiresXRLoader : ISceneRequirement
    {
        enum XRLoaderType { OpenXR, ARCore, ARKit, Simulation }

        [SerializeField]
        XRLoaderType m_LoaderType;

        public virtual void Evaluate(List<RequirementResult> results)
        {
            bool isSupported;
            switch (m_LoaderType)
            {
                case XRLoaderType.ARCore:
#if !UNITY_ANDROID
                    isSupported = false;
#else
                    isSupported = XRManagerUtility.IsLoaderActive<ARCoreLoader>();
#endif
                    break;
                case XRLoaderType.ARKit:
#if !UNITY_IOS
                    isSupported = false;
#else
                    isSupported = XRManagerUtility.IsLoaderActive<ARKitLoader>();
#endif
                    break;
                case XRLoaderType.OpenXR:
                    isSupported = XRManagerUtility.IsLoaderActive<OpenXRLoader>();
                    break;
                case XRLoaderType.Simulation:
#if !UNITY_EDITOR
                    isSupported = false;
#else
                    isSupported = XRManagerUtility.IsLoaderActive<SimulationLoader>();
#endif
                    break;
                default:
                    throw new NotSupportedException($"Unsupported XRLoader type: {m_LoaderType}");
            }

            results.Add(new RequirementResult(isSupported, $"{GetType().Name} ({m_LoaderType})"));
        }
    }
}
