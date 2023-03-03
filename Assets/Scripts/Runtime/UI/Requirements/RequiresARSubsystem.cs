using System.Collections;
using System.Collections.Generic;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(Button))]
    public abstract class RequiresARSubsystem<TSubsystem, TSubsystemDescriptor> : MonoBehaviour
        where TSubsystem : SubsystemWithProvider
        where TSubsystemDescriptor : ISubsystemDescriptor
    {
        // ReSharper disable once StaticMemberInGenericType
        static bool s_Initialized;
        static List<TSubsystemDescriptor> s_Descriptors = new();

        protected static TSubsystem s_LoadedSubsystem;

        protected Button m_Button;

        protected virtual IEnumerator Start()
        {
            m_Button = GetComponent<Button>();
            yield return null;

            if (!s_Initialized)
            {
                s_Initialized = true;
                SubsystemManager.GetSubsystemDescriptors(s_Descriptors);
                s_LoadedSubsystem = LoaderUtility.GetActiveLoader()?.GetLoadedSubsystem<TSubsystem>();
            }

            if (s_LoadedSubsystem == null || s_Descriptors.Count == 0)
                ARSceneSelectUI.DisableButton(m_Button);
        }
    }
}
