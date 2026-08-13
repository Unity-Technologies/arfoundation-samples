using System;
using System.Collections.Generic;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public abstract class RequiresARSubsystem<TSubsystem, TSubsystemDescriptor> : ISceneRequirement
        where TSubsystem : SubsystemWithProvider
        where TSubsystemDescriptor : ISubsystemDescriptor
    {
        protected const string k_SubRequirementRemediationText =
            "Verify this feature is supported on your device, enabled in <b>Project Settings</b>, and has the required permissions.";

        // ReSharper disable once StaticMemberInGenericType
        static bool s_Initialized;
        static List<TSubsystemDescriptor> s_Descriptors = new();

        protected static TSubsystem s_LoadedSubsystem;

        public virtual void Evaluate(List<RequirementResult> results)
        {
            if (!s_Initialized)
            {
                s_Initialized = true;
                SubsystemManager.GetSubsystemDescriptors(s_Descriptors);
                s_LoadedSubsystem = LoaderUtility.GetActiveLoader()?.GetLoadedSubsystem<TSubsystem>();
            }

            results.Add(new RequirementResult(
                s_LoadedSubsystem != null && s_Descriptors.Count != 0,
                typeof(TSubsystem).Name,
                "The subsystem was not loaded."));
        }
    }
}
