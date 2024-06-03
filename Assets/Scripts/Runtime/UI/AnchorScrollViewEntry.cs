using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class AnchorScrollViewEntry : MonoBehaviour
    {
        [Header("Entry References")]
        [SerializeField]
        Button m_ActionButton;

        [SerializeField]
        Button m_EraseButton;

        [SerializeField]
        TextMeshProUGUI m_AnchorDisplayLabel;
        public string AnchorDisplayText => m_AnchorDisplayLabel.text;

        [Header("Action Button References")]
        [SerializeField]
        GameObject m_ActionButtonText;

        [SerializeField]
        GameObject m_ActionButtonIcon;
        
        [SerializeField]
        LoadingVisualizer m_ActionLoadingVisualizer;

        [SerializeField]
        GameObject m_ActionSuccessVisualizer;

        [SerializeField]
        GameObject m_ActionErrorVisualizer;
        
        [Header("Erase Button References")]
        [SerializeField]
        GameObject m_EraseButtonIcon;

        [SerializeField]
        LoadingVisualizer m_EraseLoadingVisualizer;
        
        [SerializeField]
        GameObject m_EraseSuccessVisualizer;

        [SerializeField]
        GameObject m_EraseErrorVisualizer;

        public ARAnchor representedAnchor { get; set; }

        public SerializableGuid persistentAnchorGuid { get; set; }

        [SerializeField, Tooltip("The event raised when the action button is clicked.")]
        UnityEvent<AnchorScrollViewEntry> m_RequestAction = new();
        public UnityEvent<AnchorScrollViewEntry> requestAction => m_RequestAction;

        [SerializeField, Tooltip("The event raised when the erase button is clicked.")]
        UnityEvent<AnchorScrollViewEntry> m_RequestEraseAnchor = new();
        public UnityEvent<AnchorScrollViewEntry> requestEraseAnchor => m_RequestEraseAnchor;

        CancellationTokenSource m_CancellationTokenSource = new();

        public void StartActionLoadingAnimation()
        {
            m_ActionButtonText.SetActive(false);
            m_ActionButtonIcon.SetActive(false);
            m_ActionLoadingVisualizer.StartAnimating();
        }

        public void StopActionLoadingAnimation()
        {
            m_ActionLoadingVisualizer.StopAnimating();
            m_ActionButtonIcon.SetActive(true);
            m_ActionButtonText.SetActive(true);
        }
        
        public void StartEraseLoadingAnimation()
        {
            m_EraseButtonIcon.SetActive(false);
            m_EraseLoadingVisualizer.StartAnimating();
        }

        public void StopEraseLoadingAnimation()
        {
            m_EraseLoadingVisualizer.StopAnimating();
            m_EraseButtonIcon.SetActive(true);
        }

        public void EnableActionButton(bool isOn)
        {
            m_ActionButton.SetEnabled(isOn);
        }

        public void EnableEraseButton(bool isOn)
        {
            m_EraseButton.interactable = isOn;
        }

        public void ShowEraseButton(bool isOn)
        {
            m_EraseButton.gameObject.SetActive(isOn);
        }

        public async Task ShowActionResult(bool isSuccessful, float durationInSeconds)
        {
            var visualizer = isSuccessful ? m_ActionSuccessVisualizer : m_ActionErrorVisualizer;
            visualizer.SetActive(true);
            m_ActionButtonText.SetActive(false);
            m_ActionButtonIcon.SetActive(false);

            try
            {
                await Task.Delay((int)(durationInSeconds * 1000), m_CancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // do nothing
            }

            visualizer.SetActive(false);
            m_ActionButtonText.SetActive(true);
            m_ActionButtonIcon.SetActive(true);
        }

        public async Task ShowEraseResult(bool isSuccessful, float durationInSeconds)
        {
            var visualizer = isSuccessful ? m_EraseSuccessVisualizer : m_EraseErrorVisualizer;
            visualizer.SetActive(true);
            m_EraseButtonIcon.SetActive(false);

            try
            {
                await Task.Delay((int)(durationInSeconds * 1000), m_CancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // do nothing
            }

            visualizer.SetActive(false);
            m_EraseButtonIcon.SetActive(true);
        }

        public void SetDisplayedAnchorLabel(string label)
        {
            m_AnchorDisplayLabel.text = label;
        }

        void OnEnable()
        {
            m_CancellationTokenSource = new();
            m_ActionButton.onClick.AddListener(OnRequestActionButtonClicked);
            m_EraseButton.onClick.AddListener(OnEraseAnchorButtonClicked);
        }

        void OnDisable()
        {
            m_ActionButton.onClick.RemoveListener(OnRequestActionButtonClicked);
            m_EraseButton.onClick.RemoveListener(OnEraseAnchorButtonClicked);
            m_CancellationTokenSource.Cancel();
        }

        void OnDestroy()
        {
            m_ActionButton.onClick.RemoveAllListeners();
            m_EraseButton.onClick.RemoveAllListeners();
        }

        void OnRequestActionButtonClicked()
        {
            m_RequestAction?.Invoke(this);
        }

        void OnEraseAnchorButtonClicked()
        {
            m_RequestEraseAnchor?.Invoke(this);
        }
    }
}
