using System;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MetaPersistentAnchorEntry : MonoBehaviour
    {
        [Header("Script References")]
        [SerializeField]
        TextMeshProUGUI m_IdLabel;

        [SerializeField]
        UIAnchorInSceneVisualizer m_UIAnchorInSceneVisualizer;

        [SerializeField]
        UIAnchorLastSaveVisualizer m_UIAnchorLastSaveVisualizer;

        [SerializeField]
        AnchorToastNotification m_ToastNotification;

        [Header("Buttons")]
        [SerializeField]
        Toggle m_Toggle;
        public Toggle toggle => m_Toggle;

        [SerializeField]
        Button m_SaveButton;

        [SerializeField]
        Button m_EraseButton;

        [SerializeField]
        Button m_LoadButton;

        [SerializeField]
        Button m_RemoveButton;

        [Header("Events")]
        [SerializeField]
        UnityEvent<MetaPersistentAnchorEntry, bool> m_EntryToggled = new();
        public UnityEvent<MetaPersistentAnchorEntry, bool> entryToggled => m_EntryToggled;

        [SerializeField]
        UnityEvent<MetaPersistentAnchorEntry> m_SaveRequested = new();
        public UnityEvent<MetaPersistentAnchorEntry> saveRequested => m_SaveRequested;

        [SerializeField]
        UnityEvent<MetaPersistentAnchorEntry> m_LoadRequested = new();
        public UnityEvent<MetaPersistentAnchorEntry> loadRequested => m_LoadRequested;

        [SerializeField]
        UnityEvent<MetaPersistentAnchorEntry> m_EraseRequested = new();
        public UnityEvent<MetaPersistentAnchorEntry> eraseRequested => m_EraseRequested;

        [SerializeField]
        UnityEvent<MetaPersistentAnchorEntry> m_RemoveRequested = new();
        public UnityEvent<MetaPersistentAnchorEntry> removeRequested => m_RemoveRequested;

        [SerializeField]
        UnityEvent<bool> m_SavedStateChanged = new();
        public UnityEvent<bool> savedStateChanged => m_SavedStateChanged;

        [SerializeField]
        UnityEvent<bool> m_InSceneStateChanged = new();
        public UnityEvent<bool> inSceneStateStateChanged => m_InSceneStateChanged;

        public int entryId { get; private set; } = -1;

        public bool isInScene => anchor != null;

        public ARAnchor anchor { get; private set; }

        public TrackableId trackableId => anchor?.trackableId ?? TrackableId.invalidId;

        public bool isSaved { get; private set; }

        public SerializableGuid savedAnchorGuid { get; private set; }

        bool m_HasEntryIdBeenSet;
        bool m_SupportsSaveAndLoad = true;
        bool m_SupportsErase = true;

        /// <summary>
        /// Sets the entryId.
        /// </summary>
        /// <param name="id">The value to set the entryId to.</param>
        public void SetEntryId(int id)
        {
            if (m_HasEntryIdBeenSet)
                return;

            m_HasEntryIdBeenSet = true;
            entryId = id;
            m_IdLabel.text = id.ToString();
            SetEntryIdOnAnchor();
        }

        /// <summary>
        /// Show the in progress toast notification.
        /// </summary>
        public void ShowInProgress()
        {
            m_SaveButton.SetEnabled(false);
            m_EraseButton.SetEnabled(false);
            m_LoadButton.SetEnabled(false);
            m_RemoveButton.SetEnabled(false);
            m_ToastNotification.ShowInProgressNotification();
        }

        /// <summary>
        /// Stops an in progress toast notification and shows a success or failed toast notification.
        /// </summary>
        /// <param name="resultStatus">The result to display</param>
        public void ShowResult(XRResultStatus resultStatus)
        {
            m_ToastNotification.ShowResult(resultStatus.IsSuccess(), resultStatus.statusCode.ToString());
            m_SaveButton.SetEnabled(m_SupportsSaveAndLoad && isInScene && !isSaved);
            m_EraseButton.SetEnabled(m_SupportsErase && isSaved);
            m_LoadButton.SetEnabled(m_SupportsSaveAndLoad && isSaved);
            m_RemoveButton.SetEnabled(isInScene);
        }

        /// <summary>
        /// Stops an in progress toast notification and shows a success or failed toast notification.
        /// </summary>
        /// <param name="success">The success or failed result to display.</param>
        public void ShowResult(bool success)
        {
            m_ToastNotification.ShowResult(success, success ? "Success" : "Failed");
            m_SaveButton.SetEnabled(m_SupportsSaveAndLoad && isInScene && !isSaved);
            m_LoadButton.SetEnabled(m_SupportsSaveAndLoad && isSaved);
            m_EraseButton.SetEnabled(m_SupportsErase && isSaved);
            m_RemoveButton.SetEnabled(isInScene);
        }

        /// <summary>
        /// Updates the ARAnchor reference that this UIAnchorEntry represents when the anchor exists in the scene.
        /// </summary>
        /// <param name="anchor">The in anchor the UIAnchorEntry represents. Pass `null` if the anchor is not in the scene.</param>
        public void UpdateInSceneStatus(ARAnchor anchor)
        {
            if (this.anchor == anchor)
                return;

            this.anchor = anchor;
            m_UIAnchorInSceneVisualizer.UpdateVisualizer(isInScene);
            m_SaveButton.SetEnabled(m_SupportsSaveAndLoad && isInScene);
            m_RemoveButton.SetEnabled(isInScene);

            SetEntryIdOnAnchor();

            m_InSceneStateChanged?.Invoke(isInScene);
        }

        /// <summary>
        /// Updates the saved status of ARAnchor.
        /// </summary>
        /// <param name="isSaved">Pass true if the anchor is saved, otherwise false.</param>
        /// <param name="savedAnchorGuid">The saved anchor guid of the ARAnchor if the anchor is saved. Pass `default`
        /// if `isSaved` is false.</param>
        /// <param name="savedTime">The date and time when the anchor was saved. Pass `default` if `isSaved` is false.</param>
        public void UpdateSaveStatus(bool isSaved, SerializableGuid savedAnchorGuid, DateTime savedTime)
        {
            if (this.isSaved == isSaved)
                return;

            this.isSaved = isSaved;
            this.savedAnchorGuid = savedAnchorGuid;
            m_SaveButton.gameObject.SetActive(m_SupportsSaveAndLoad && m_SupportsErase && !isSaved);
            m_EraseButton.gameObject.SetActive(m_SupportsErase && isSaved);

            m_EraseButton.SetEnabled(m_SupportsErase && isSaved);
            m_LoadButton.SetEnabled(m_SupportsSaveAndLoad && isSaved);

            m_UIAnchorLastSaveVisualizer.UpdateVisualizer(m_SupportsSaveAndLoad && isSaved, savedTime);
            m_SavedStateChanged?.Invoke(m_SupportsSaveAndLoad && isSaved);
        }

        public void DisableSaveAndLoadButtons()
        {
            m_SupportsSaveAndLoad = false;
            m_SaveButton.SetEnabled(false);
            m_LoadButton.SetEnabled(false);
        }

        public void DisableEraseButton()
        {
            m_SupportsErase = false;
            m_EraseButton.SetEnabled(false);
        }

        void Reset()
        {
            m_Toggle = GetComponentInChildren<Toggle>();
            m_IdLabel = GetComponentInChildren<TextMeshProUGUI>();
            m_UIAnchorInSceneVisualizer = GetComponentInChildren<UIAnchorInSceneVisualizer>();
            m_UIAnchorLastSaveVisualizer = GetComponentInChildren<UIAnchorLastSaveVisualizer>();
            m_ToastNotification = GetComponentInChildren<AnchorToastNotification>();
        }

        void Awake()
        {
            if (m_IdLabel == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_IdLabel)} is null."), this);

            if (m_UIAnchorInSceneVisualizer == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_UIAnchorInSceneVisualizer)} is null."), this);

            if (m_UIAnchorLastSaveVisualizer == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_UIAnchorLastSaveVisualizer)} is null."), this);

            if (m_ToastNotification == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_ToastNotification)} is null."), this);

            if (m_Toggle == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_Toggle)} is null."), this);

            if (m_SaveButton == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_SaveButton)} is null."), this);

            if (m_EraseButton == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_EraseButton)} is null."), this);

            if (m_LoadButton == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_LoadButton)} is null."), this);

            if (m_RemoveButton == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_RemoveButton)} is null."), this);
        }

        void OnEnable()
        {
            m_Toggle.onValueChanged.AddListener(isOn =>
            {
                m_EntryToggled?.Invoke(this, isOn);
            });

            if (m_SupportsSaveAndLoad)
            {
                m_SaveButton.onClick.AddListener(() =>
                {
                    m_SaveRequested?.Invoke(this);
                });
            }

            if (m_SupportsErase)
            {
                m_EraseButton.onClick.AddListener(() =>
                {
                    m_EraseRequested?.Invoke(this);
                });
            }

            if (m_SupportsSaveAndLoad)
            {
                m_LoadButton.onClick.AddListener(() =>
                {
                    m_LoadRequested?.Invoke(this);
                });
            }

            m_RemoveButton.onClick.AddListener(() =>
            {
                m_RemoveRequested?.Invoke(this);
            });
        }

        void SetEntryIdOnAnchor()
        {
            if (anchor == null)
                return;

            var debugInfoDisplayController = anchor.GetComponent<ARAnchorDebugVisualizer>();
            if (debugInfoDisplayController == null)
                return;

            debugInfoDisplayController.showEntryId = true;
            debugInfoDisplayController.entryId = entryId;
        }
    }
}
