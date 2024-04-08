using System;
using UnityEngine;

#if METAOPENXR_0_2_OR_NEWER && UNITY_ANDROID
using UnityEngine.XR.OpenXR.Features.Meta;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MetaSceneCaptureSample : MonoBehaviour
    {
        [SerializeField]
        ARSession m_ARSession;

        void Reset() => m_ARSession = FindAnyObjectByType<ARSession>();

        void Awake()
        {
            if (m_ARSession == null)
                m_ARSession = FindAnyObjectByType<ARSession>();
        }

        public void RequestSceneCapture()
        {
            if (m_ARSession == null)
            {
                Debug.LogError("AR Session is null, so scene capture cannot be requested.", this);

                return;
            }

            if (m_ARSession.subsystem == null)
            {
                Debug.LogError("Session subsystem is null, so scene capture cannot be requested. Are your XR Plug-in Management settings correct?");

                return;
            }

#if METAOPENXR_0_2_OR_NEWER && UNITY_ANDROID
            var success = (m_ARSession.subsystem as MetaOpenXRSessionSubsystem)?.TryRequestSceneCapture() ?? false;
            Debug.Log($"Meta OpenXR scene capture completed request with result: {success}");
#else
            Debug.LogError("Meta-OpenXR is not installed. Please install the package \"Unity OpenXR: Meta\" version 0.2 or newer to use scene capture.");
#endif
        }
    }
}
