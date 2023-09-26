using System.Collections;
using TMPro;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(ARCameraManager))]
    public class ImageStabilization : MonoBehaviour
    {
        [SerializeField]
        TMP_Text m_Information;

        ARCameraManager m_CameraManager;
        string m_SupportStatus;

        void OnEnable()
        {
            m_CameraManager = GetComponent<ARCameraManager>();
            StartCoroutine(UpdateSupportStatus());
        }

        IEnumerator UpdateSupportStatus()
        {
            yield return null;
            m_SupportStatus = m_CameraManager.subsystem.subsystemDescriptor.supportsImageStabilization.ToString();
        }

        public void ToggleImageStabilization()
        {
            var supportStatus = m_CameraManager.subsystem.subsystemDescriptor.supportsImageStabilization;
            m_SupportStatus = supportStatus.ToString();

            if (supportStatus == Supported.Supported)
            {
                m_CameraManager.imageStabilizationRequested = !m_CameraManager.imageStabilizationRequested;
            }
            else
            {
                m_CameraManager.imageStabilizationRequested = false;
            }
        }

        void Update()
        {
            if (m_Information == null)
                return;

            var stabilizationEnabled = m_CameraManager.imageStabilizationEnabled ? "On" : "Off";
            m_Information.text = $"Support: {m_SupportStatus}\n" +
                $"Stabilization: {stabilizationEnabled}";
        }
    }
}
