namespace UnityEngine.XR.ARFoundation.Samples
{
    public static class SubsystemsUtility
    {
        const float k_DefaultTimeout = 5f;

        public static bool TryGetLoadedSubsystem<TSubsystemBase, TSubsystemConcrete>(out TSubsystemConcrete subsystem)
            where TSubsystemBase : class, ISubsystem
            where TSubsystemConcrete : class, TSubsystemBase
        {
            var loader = LoaderUtility.GetActiveLoader();
            var asBaseSubsystem = loader != null ? loader.GetLoadedSubsystem<TSubsystemBase>() : null;
            subsystem = asBaseSubsystem as TSubsystemConcrete;
            return subsystem != null;
        }

        public static async Awaitable<TSubsystemConcrete> GetRunningSubsystem<TSubsystemBase, TSubsystemConcrete>
            (float timeout = k_DefaultTimeout)
            where TSubsystemBase : class, ISubsystem
            where TSubsystemConcrete : class, TSubsystemBase
        {
            if (!TryGetLoadedSubsystem<TSubsystemBase, TSubsystemConcrete>(out var subsystem))
            {
                Debug.LogError($"Can't get active {typeof(TSubsystemConcrete)}");
                return null;
            }

            var awaitTime = 0f;

            while (!subsystem.running)
            {
                if (awaitTime > timeout)
                {
                    Debug.LogError($"Subsystem {typeof(TSubsystemConcrete)} is not running");
                    return null;
                }

                await Awaitable.NextFrameAsync();
                awaitTime += Time.deltaTime;
            }

            return subsystem;
        }
    }
}
