using System;
using System.Collections.Generic;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public abstract class RequiresARSubsystem<TSubsystem, TSubsystemDescriptor> : IBooleanExpression
        where TSubsystem : SubsystemWithProvider
        where TSubsystemDescriptor : ISubsystemDescriptor
    {
        // ReSharper disable once StaticMemberInGenericType
        static bool s_Initialized;
        static List<TSubsystemDescriptor> s_Descriptors = new();

        protected static TSubsystem s_LoadedSubsystem;

        public virtual bool Evaluate()
        {
            if (!s_Initialized)
            {
                s_Initialized = true;
                SubsystemManager.GetSubsystemDescriptors(s_Descriptors);
                s_LoadedSubsystem = LoaderUtility.GetActiveLoader()?.GetLoadedSubsystem<TSubsystem>();
            }

            return s_LoadedSubsystem != null && s_Descriptors.Count != 0;
        }
    }
}
