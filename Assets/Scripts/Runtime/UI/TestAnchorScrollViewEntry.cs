using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// This class provides functionality to test persistent anchors in valid and invalid
    /// contexts. For example, loading and erasing an anchor that was never saved or saving
    /// an anchor that has already been saved.
    /// </summary>
    public class TestAnchorScrollViewEntry : MonoBehaviour
    {
        [Header("Entry References")]
        [SerializeField]
        Button m_SaveAndLeaveButton;

        [SerializeField]
        Button m_LoadAndLeaveButton;

        [SerializeField]
        Button m_SaveButton;

        [SerializeField]
        Button m_LoadButton;

        [SerializeField]
        Button m_EraseButton;

        [SerializeField]
        Button m_SaveAndCancelButton;
        
        [SerializeField]
        Button m_LoadAndCancelButton;

        [SerializeField]
        TextMeshProUGUI m_AnchorDisplayLabel;
        public string AnchorDisplayText => m_AnchorDisplayLabel.text;

        [SerializeField]
        TextMeshProUGUI m_AnchorSavedDateLabel;

        [Header("Save Button References")]
        [SerializeField]
        GameObject m_SaveButtonText;

        [SerializeField]
        LoadingVisualizer m_SaveLoadingVisualizer;

        [SerializeField]
        GameObject m_SaveSuccessVisualizer;

        [SerializeField]
        GameObject m_SaveErrorVisualizer;

        [Header("Lopad Button References")]
        [SerializeField]
        GameObject m_LoadButtonText;

        [SerializeField]
        LoadingVisualizer m_LoadLoadingVisualizer;

        [SerializeField]
        GameObject m_LoadSuccessVisualizer;

        [SerializeField]
        GameObject m_LoadErrorVisualizer;

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

        public SerializableGuid savedAnchorGuid { get; set; }

        UnityEvent<TestAnchorScrollViewEntry> m_RequestSaveAndLeave = new();
        public UnityEvent<TestAnchorScrollViewEntry> requestSaveAndLeave => m_RequestSaveAndLeave;

        UnityEvent<TestAnchorScrollViewEntry> m_RequestLoadAndLeave = new();
        public UnityEvent<TestAnchorScrollViewEntry> requestLoadAndLeave => m_RequestLoadAndLeave;
        
        UnityEvent<TestAnchorScrollViewEntry> m_RequestSaveAndCancel = new();
        public UnityEvent<TestAnchorScrollViewEntry> requestSaveAndCancel => m_RequestSaveAndCancel;
        
        UnityEvent<TestAnchorScrollViewEntry> m_RequestLoadAndCancel = new();
        public UnityEvent<TestAnchorScrollViewEntry> requestLoadAndCancel => m_RequestLoadAndCancel;

        [SerializeField, Tooltip("The event raised when the action button is clicked.")]
        UnityEvent<TestAnchorScrollViewEntry> m_RequestSave = new();
        public UnityEvent<TestAnchorScrollViewEntry> requestSave => m_RequestSave;

        [SerializeField, Tooltip("The event raised when the action button is clicked.")]
        UnityEvent<TestAnchorScrollViewEntry> m_RequestLoad = new();
        public UnityEvent<TestAnchorScrollViewEntry> requestLoad => m_RequestLoad;

        [SerializeField, Tooltip("The event raised when the erase button is clicked.")]
        UnityEvent<TestAnchorScrollViewEntry> m_RequestEraseAnchor = new();
        public UnityEvent<TestAnchorScrollViewEntry> requestEraseAnchor => m_RequestEraseAnchor;

        CancellationTokenSource m_CancellationTokenSource = new();

        public void StartSaveInProgressAnimation()
        {
            m_SaveButtonText.SetActive(false);
            m_SaveLoadingVisualizer.StartAnimating();
        }

        public void StopSaveInProgressAnimation()
        {
            m_SaveLoadingVisualizer.StopAnimating();
            m_SaveButtonText.SetActive(true);
        }

        public void StartLoadInProgressAnimation()
        {
            m_LoadButtonText.SetActive(false);
            m_LoadLoadingVisualizer.StartAnimating();
        }

        public void StopLoadInProgressAnimation()
        {
            m_LoadLoadingVisualizer.StopAnimating();
            m_LoadButtonText.SetActive(true);
        }

        public void StartEraseInProgressAnimation()
        {
            m_EraseButtonIcon.SetActive(false);
            m_EraseLoadingVisualizer.StartAnimating();
        }

        public void StopEraseInProgressAnimation()
        {
            m_EraseLoadingVisualizer.StopAnimating();
            m_EraseButtonIcon.SetActive(true);
        }

        public async Awaitable ShowSaveResult(bool isSuccessful, float durationInSeconds)
        {
            var visualizer = isSuccessful ? m_SaveSuccessVisualizer : m_SaveErrorVisualizer;
            visualizer.SetActive(true);
            m_SaveButtonText.SetActive(false);

            try
            {
                await Awaitable.WaitForSecondsAsync(durationInSeconds, m_CancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // do nothing
            }

            visualizer.SetActive(false);
            m_SaveButtonText.SetActive(true);
        }

        public async Awaitable ShowLoadResult(bool isSuccessful, float durationInSeconds)
        {
            var visualizer = isSuccessful ? m_LoadSuccessVisualizer : m_LoadErrorVisualizer;
            visualizer.SetActive(true);
            m_LoadButtonText.SetActive(false);

            try
            {
                await Awaitable.WaitForSecondsAsync(durationInSeconds, m_CancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // do nothing
            }

            visualizer.SetActive(false);
            m_LoadButtonText.SetActive(true);
        }

        public async Awaitable ShowEraseResult(bool isSuccessful, float durationInSeconds)
        {
            var visualizer = isSuccessful ? m_EraseSuccessVisualizer : m_EraseErrorVisualizer;
            visualizer.SetActive(true);
            m_EraseButtonIcon.SetActive(false);

            try
            {
                await Awaitable.WaitForSecondsAsync(durationInSeconds, m_CancellationTokenSource.Token);
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

        public void SetAnchorSavedDateTime(DateTime dateTime)
        {
            var formattedDateTime = string.Format(
                "{0:d}\n<color=#bbb>{0:t}</color>",
                dateTime);

            m_AnchorSavedDateLabel.text = formattedDateTime;
        }

        void OnEnable()
        {
            m_CancellationTokenSource = new();
            m_SaveAndLeaveButton.onClick.AddListener(RequestSaveAndLeaveButtonClicked);
            m_LoadAndLeaveButton.onClick.AddListener(RequestLoadAndLeaveButtonClicked);
            m_SaveButton.onClick.AddListener(OnRequestSaveButtonClicked);
            m_LoadButton.onClick.AddListener(OnRequestLoadButtonClicked);
            m_EraseButton.onClick.AddListener(OnEraseAnchorButtonClicked);
            m_SaveAndCancelButton.onClick.AddListener(OnSaveAndCancelButtonClicked);
            m_LoadAndCancelButton.onClick.AddListener(OnLoadAndCancelButtonClicked);
        }

        void OnDisable()
        {
            m_SaveAndLeaveButton.onClick.RemoveListener(RequestSaveAndLeaveButtonClicked);
            m_LoadAndLeaveButton.onClick.AddListener(RequestLoadAndLeaveButtonClicked);
            m_SaveButton.onClick.RemoveListener(OnRequestSaveButtonClicked);
            m_LoadButton.onClick.RemoveListener(OnRequestLoadButtonClicked);
            m_EraseButton.onClick.RemoveListener(OnEraseAnchorButtonClicked);
            m_SaveAndCancelButton.onClick.RemoveListener(OnSaveAndCancelButtonClicked);
            m_LoadAndCancelButton.onClick.RemoveListener(OnLoadAndCancelButtonClicked);
            m_CancellationTokenSource.Cancel();
        }

        void OnDestroy()
        {
            m_SaveButton.onClick.RemoveAllListeners();
            m_LoadButton.onClick.RemoveAllListeners();
            m_EraseButton.onClick.RemoveAllListeners();
            m_SaveAndCancelButton.onClick.RemoveAllListeners();
            m_LoadAndCancelButton.onClick.RemoveAllListeners();
        }

        void RequestSaveAndLeaveButtonClicked()
        {
            m_RequestSaveAndLeave?.Invoke(this);
        }

        void RequestLoadAndLeaveButtonClicked()
        {
            m_RequestLoadAndLeave?.Invoke(this);
        }

        void OnRequestSaveButtonClicked()
        {
            m_RequestSave?.Invoke(this);
        }

        void OnRequestLoadButtonClicked()
        {
            m_RequestLoad?.Invoke(this);
        }

        void OnEraseAnchorButtonClicked()
        {
            m_RequestEraseAnchor?.Invoke(this);
        }

        void OnSaveAndCancelButtonClicked()
        {
            m_RequestSaveAndCancel?.Invoke(this);
        }

        void OnLoadAndCancelButtonClicked()
        {
            m_RequestLoadAndCancel?.Invoke(this);
        }
    }
}
