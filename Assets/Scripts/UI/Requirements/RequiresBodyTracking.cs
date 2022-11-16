using System.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RequiresBodyTracking : RequiresARSubsystem<XRHumanBodySubsystem, XRHumanBodySubsystemDescriptor>
    {
        [SerializeField]
        bool m_Requires2DTracking;

        [SerializeField]
        bool m_Requires3DTracking;

        protected override IEnumerator Start()
        {
            yield return base.Start();

            if (m_Button.interactable == false)
                yield break;

            var descriptor = s_LoadedSubsystem.subsystemDescriptor;

            if (m_Requires2DTracking && !descriptor.supportsHumanBody2D)
            {
                ARSceneSelectUI.DisableButton(m_Button);
                yield break;
            }

            if (m_Requires3DTracking && !descriptor.supportsHumanBody3D)
            {
                ARSceneSelectUI.DisableButton(m_Button);
            }
        }
    }
}
