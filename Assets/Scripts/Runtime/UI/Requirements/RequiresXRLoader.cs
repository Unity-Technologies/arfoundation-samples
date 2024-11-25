using System;
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
    public class RequiresXRLoader : IBooleanExpression
    {
        enum XRLoaderType { OpenXR, ARCore, ARKit, Simulation }

        [SerializeField]
        XRLoaderType m_LoaderType;

        public bool Evaluate()
        {
            switch (m_LoaderType)
            {
                case XRLoaderType.ARCore:
#if !UNITY_ANDROID
                    return false;
#else
                    return XRManagerUtility.IsLoaderActive<ARCoreLoader>();
#endif
                case XRLoaderType.ARKit:
#if !UNITY_IOS
                    return false;
#else
                    return XRManagerUtility.IsLoaderActive<ARKitLoader>();
#endif
                case XRLoaderType.OpenXR:
                    return XRManagerUtility.IsLoaderActive<OpenXRLoader>();
                case XRLoaderType.Simulation:
#if !UNITY_EDITOR
                    return false;
#else
                    return XRManagerUtility.IsLoaderActive<SimulationLoader>();
#endif
                default:
                    throw new NotSupportedException($"Unsupported XRLoader type: {m_LoaderType}");
            }
        }
    }
}
