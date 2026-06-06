using System;
using System.Collections.Generic;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresPlatform : ISceneRequirement
    {
        enum Platform { iOS, Android }

        [SerializeField]
        Platform m_RequiredPlatform;

        [SerializeField]
        VersionRequirement m_VersionRequirement;

        [SerializeField]
        bool m_AllowEditor;

        public virtual void Evaluate(List<RequirementResult> results)
        {
#if !UNITY_IOS
            if (m_RequiredPlatform == Platform.iOS)
            {
                results.Add(new RequirementResult(false, $"{GetType().Name} ({m_RequiredPlatform})"));
                return;
            }
#elif !UNITY_ANDROID
            if (m_RequiredPlatform == Platform.Android)
            {
                results.Add(new RequirementResult(false, $"{GetType().Name} ({m_RequiredPlatform})"));
                return;
            }
#endif

            if (Application.isEditor && !m_AllowEditor)
            {
                results.Add(new RequirementResult(false, $"{GetType().Name} ({m_RequiredPlatform})"));
                return;
            }

#if UNITY_IOS
            if (m_VersionRequirement.requiresMinimumVersion)
            {
                string version = iOS.Device.systemVersion;
                int major = int.Parse(version.Split(".")[0]);
                if (major < m_VersionRequirement.minimumVersion)
                {
                    results.Add(new RequirementResult(false, $"{GetType().Name} ({m_RequiredPlatform})"));
                    return;
                }
            }
#endif
            results.Add(new RequirementResult(true, $"{GetType().Name} ({m_RequiredPlatform})"));
        }

        [Serializable]
        public struct VersionRequirement
        {
            public bool requiresMinimumVersion;
            public int minimumVersion;
        }
    }
}
