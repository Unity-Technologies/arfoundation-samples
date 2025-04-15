using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public static class DisplayRefreshRateUtilityFactory
    {
        internal static bool TryCreate(out IDisplayRefreshRateUtility utility)
        {
            var loader = LoaderUtility.GetActiveLoader();
            var sessionSubsystem = loader != null ? loader.GetLoadedSubsystem<XRSessionSubsystem>() : null;

            if (sessionSubsystem == null)
            {
                // Could be null if user does not have "Initialize XR on Startup" enabled in XR Plug-in Management
                utility = default;
                return false;
            }

            // We switch on Session Descriptor id because we can't guarantee with current preprocessor directives whether
            // a provider package (and its types) will be present. For example, UNITY_ANDROID could signal that either
            // ARCore or OpenXR loader is present. Because we don't know for sure, we are unable to switch on the loader
            // type without introducing a build-time error in case that package was stripped.
            utility = sessionSubsystem.subsystemDescriptor.id switch
            {
                "Meta-Session" => new MetaDisplayRefreshRateUtility(),
                "Android-Session" => new AndroidXRDisplayRefreshRateUtility(),
                _ => null,
            };

            return true;
        }

    }
}
