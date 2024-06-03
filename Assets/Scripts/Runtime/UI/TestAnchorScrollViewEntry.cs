using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class TestAnchorScrollViewEntry : MonoBehaviour
    {
        [Header("Entry References")]
        [SerializeField]
        Button m_SaveButton;

        [SerializeField]
        Button m_LoadButton;

        [SerializeField]
        Button m_EraseButton;

        [SerializeField]
        TextMeshProUGUI m_AnchorDisplayLabel;
        public string AnchorDisplayText => m_AnchorDisplayLabel.text;

        [Header("Save Button References")]
        [SerializeField]
        GameObject m_SaveButtonText;

        [SerializeField]
        GameObject m_SaveButtonIcon;

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
        GameObject m_LoadButtonIcon;
        
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

        public SerializableGuid persistentAnchorGuid { get; set; }

        [SerializeField, Tooltip("The event raised when the action button is clicked.")]
        UnityEvent<TestAnchorScrollViewEntry> m_RequestSave = new();
        public UnityEvent<TestAnchorScrollViewEntry> requestSave => m_RequestSave;

        [SerializeField, Tooltip("The event raised when the action button is clicked.")]
        UnityEvent<TestAnchorScrollViewEntry> m_RequestLoad = new();
        public UnityEvent<TestAnchorScrollViewEntry> requestLoad => m_RequestLoad;
        
        [SerializeField, Tooltip("The event raised when the erase button is clicked.")]
        UnityEvent<TestAnchorScrollViewEntry> m_RrequestEraseAnchor = new();
        public UnityEvent<TestAnchorScrollViewEntry> requestEraseAnchor => m_RrequestEraseAnchor;

        CancellationTokenSource m_CancellationTokenSource = new();

        public void StartSaveLoadingAnimation()
        {
            m_SaveButtonText.SetActive(false);
            m_SaveButtonIcon.SetActive(false);
            m_SaveLoadingVisualizer.StartAnimating();
        }

        public void StopSaveLoadingAnimation()
        {
            m_SaveLoadingVisualizer.StopAnimating();
            m_SaveButtonIcon.SetActive(true);
            m_SaveButtonText.SetActive(true);
        }
        
        public void StartLoadLoadingAnimation()
        {
            m_LoadButtonText.SetActive(false);
            m_LoadButtonIcon.SetActive(false);
            m_LoadLoadingVisualizer.StartAnimating();
        }

        public void StopLoadLoadingAnimation()
        {
            m_LoadLoadingVisualizer.StopAnimating();
            m_LoadButtonIcon.SetActive(true);
            m_LoadButtonText.SetActive(true);
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

        public async Task ShowSaveResult(bool isSuccessful, float durationInSeconds)
        {
            var visualizer = isSuccessful ? m_SaveSuccessVisualizer : m_SaveErrorVisualizer;
            visualizer.SetActive(true);
            m_SaveButtonText.SetActive(false);
            m_SaveButtonIcon.SetActive(false);

            try
            {
                await Task.Delay((int)(durationInSeconds * 1000), m_CancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // do nothing
            }

            visualizer.SetActive(false);
            m_SaveButtonText.SetActive(true);
            m_SaveButtonIcon.SetActive(true);
        }
        
        public async Task ShowLoadResult(bool isSuccessful, float durationInSeconds)
        {
            var visualizer = isSuccessful ? m_LoadSuccessVisualizer : m_LoadErrorVisualizer;
            visualizer.SetActive(true);
            m_LoadButtonText.SetActive(false);
            m_LoadButtonIcon.SetActive(false);

            try
            {
                await Task.Delay((int)(durationInSeconds * 1000), m_CancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // do nothing
            }

            visualizer.SetActive(false);
            m_LoadButtonText.SetActive(true);
            m_LoadButtonIcon.SetActive(true);
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
            m_SaveButton.onClick.AddListener(OnRequestSaveButtonClicked);
            m_LoadButton.onClick.AddListener(OnRequestLoadButtonClicked);
            m_EraseButton.onClick.AddListener(OnEraseAnchorButtonClicked);
        }

        void OnDisable()
        {
            m_SaveButton.onClick.RemoveListener(OnRequestSaveButtonClicked);
            m_LoadButton.onClick.RemoveListener(OnRequestLoadButtonClicked);
            m_EraseButton.onClick.RemoveListener(OnEraseAnchorButtonClicked);
            m_CancellationTokenSource.Cancel();
        }

        void OnDestroy()
        {
            m_SaveButton.onClick.RemoveAllListeners();
            m_LoadButton.onClick.RemoveAllListeners();
            m_EraseButton.onClick.RemoveAllListeners();
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
            m_RrequestEraseAnchor?.Invoke(this);
        }
    }
}
