using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Text = UnityEngine.UI.Text;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(Text))]
public class EyeTrackingUI : MonoBehaviour
{
    [SerializeField]
    ARFaceManager m_Manager;

    void OnEnable()
    {
        if (m_Manager == null)
        {
            m_Manager = FindObjectOfType<ARFaceManager>();
        }
        if (m_Manager != null && m_Manager.subsystem != null &&
#if UNITY_2020_2_OR_NEWER
            m_Manager.subsystem.subsystemDescriptor.supportsEyeTracking)
#else
            m_Manager.subsystem.SubsystemDescriptor.supportsEyeTracking)
#endif
        {
            var infoGO = GetComponent<Text>();
            infoGO.text = "This device supports eye tracking.";
        }
        else
        {
            var infoGO = GetComponent<Text>();
            infoGO.text = "This device does not support eye tracking.";
        }
    }
}
