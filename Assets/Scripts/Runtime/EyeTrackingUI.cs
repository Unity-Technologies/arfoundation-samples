using UnityEngine;
using Text = UnityEngine.UI.Text;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(Text))]
    public class EyeTrackingUI : MonoBehaviour
    {
        [SerializeField]
        ARFaceManager m_Manager;

        void OnEnable()
        {
            if (m_Manager == null)
            {
                m_Manager = FindObjectsUtility.FindAnyObjectByType<ARFaceManager>();
            }
            if (m_Manager != null && m_Manager.subsystem != null && m_Manager.descriptor.supportsEyeTracking)
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
}
