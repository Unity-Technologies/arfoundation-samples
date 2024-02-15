using System;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresPlatform : IBooleanExpression
    {
        enum Platform { iOS, Android }

        [SerializeField]
        Platform m_RequiredPlatform;

        [SerializeField]
        VersionRequirement m_VersionRequirement;

        [SerializeField]
        bool m_AllowEditor;

        public bool Evaluate()
        {
#if !UNITY_IOS
            if (m_RequiredPlatform == Platform.iOS)
                return false;

#elif !UNITY_ANDROID
            if (m_RequiredPlatform == Platform.Android)
                return false;
#endif

            if (Application.isEditor && !m_AllowEditor)
                return false;

#if UNITY_IOS
            if (m_VersionRequirement.requiresMinimumVersion)
            {
                string version = iOS.Device.systemVersion;
                int major = int.Parse(version.Split(".")[0]);
                if (major < m_VersionRequirement.minimumVersion)
                    return false;
            }
#endif
            return true;
        }

        [Serializable]
        public struct VersionRequirement
        {
            public bool requiresMinimumVersion;
            public int minimumVersion;
        }
    }
}
