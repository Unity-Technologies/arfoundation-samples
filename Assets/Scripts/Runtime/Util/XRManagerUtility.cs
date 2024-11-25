using System.Linq;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.ARFoundation.Samples
{
    static class XRManagerUtility
    {
        /// <summary>
        /// Returns <see langword="true"/> if there is an active <see cref="XRLoader"/> of
        /// type <typeparamref name="TLoader"/>. Otherwise, <see langword="false"/>.
        /// </summary>
        /// <typeparam name="TLoader">The <see cref="XRLoader"/> type to check the active status of.</typeparam>
        /// <returns><see langword="true"/> if <c>TLoader</c> is active. Otherwise, <see langword="false"/>.</returns>
        /// <seealso href="https://docs.unity3d.com/Packages/com.unity.xr.management@4.3/manual/index.html"/>
        public static bool IsLoaderActive<TLoader>() where TLoader : XRLoader
        {
            var settingsInstance = XRGeneralSettings.Instance;
            if (settingsInstance == null)
                return false;

            var managerSettings = settingsInstance.Manager;
            if (managerSettings == null)
                return false;

            return managerSettings.activeLoaders?.OfType<TLoader>().Any() ?? false;
        }
    }
}
