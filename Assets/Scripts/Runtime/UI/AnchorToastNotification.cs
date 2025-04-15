using System;
using TMPro;
using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class AnchorToastNotification : MonoBehaviour
    {
        [Header("In Progress Settings")]
        [SerializeField]
        GameObject m_InProgressNotification;

        [SerializeField]
        LoadingVisualizer m_LoadingVisualizer;

        [Header("Success Settings")]
        [SerializeField]
        FadeAfterDuration m_SuccessNotification;

        [SerializeField]
        TextMeshProUGUI m_SuccessCodeLabel;

        [Header("Error Settings")]
        [SerializeField]
        FadeAfterDuration m_ErrorNotification;

        [SerializeField]
        TextMeshProUGUI m_ErrorCodeLabel;

        public void ShowInProgressNotification()
        {
            m_SuccessNotification.gameObject.SetActive(false);
            m_ErrorNotification.gameObject.SetActive(false);

            m_InProgressNotification.SetActive(true);
            m_LoadingVisualizer.StartAnimating();
        }

        public void ShowResult(bool isSuccess, string resultMessage)
        {
            m_InProgressNotification.SetActive(false);
            m_LoadingVisualizer.StopAnimating();

            var resultObj = isSuccess ? m_SuccessCodeLabel : m_ErrorCodeLabel;
            resultObj.text = resultMessage;

            m_SuccessNotification.ResetFade();
            m_SuccessNotification.gameObject.SetActive(isSuccess);

            m_ErrorNotification.ResetFade();
            m_ErrorNotification.gameObject.SetActive(!isSuccess);
        }
    }
}
