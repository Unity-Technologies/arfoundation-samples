using UnityEngine.XR.Management;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public static class SubsystemsUtility
    {
        public static bool TryGetLoadedSubsystem<TSubsystemBase, TSubsystemConcrete>(out TSubsystemConcrete subsystem)
            where TSubsystemBase : class, ISubsystem
            where TSubsystemConcrete : class, TSubsystemBase
        {
            if (XRGeneralSettings.Instance == null || XRGeneralSettings.Instance.Manager == null)
            {
                subsystem = null;
                return false;
            }

            var loader = XRGeneralSettings.Instance.Manager.activeLoader;
            var asBaseSubsystem = loader != null ? loader.GetLoadedSubsystem<TSubsystemBase>() : null;
            subsystem = asBaseSubsystem as TSubsystemConcrete;
            return subsystem != null;
        }
    }
}
