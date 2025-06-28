using System;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MetaSharedAnchorEntry : MonoBehaviour
    {
        [Header("Script References")]
        [SerializeField]
        TextMeshProUGUI m_IdLabel;

        [SerializeField]
        TextMeshProUGUI m_StatusLabel;

        [SerializeField]
        AnchorToastNotification m_ToastNotification;

        [Header("Buttons")]
        [SerializeField]
        Toggle m_Toggle;
        public Toggle toggle => m_Toggle;

        [SerializeField]
        Button m_ShareButton;

        [SerializeField]
        Button m_RemoveButton;

        [Header("Events")]
        [SerializeField]
        UnityEvent<MetaSharedAnchorEntry, bool> m_EntryToggled = new();
        public UnityEvent<MetaSharedAnchorEntry, bool> entryToggled => m_EntryToggled;

        [SerializeField]
        UnityEvent<MetaSharedAnchorEntry> m_ShareRequested = new();
        public UnityEvent<MetaSharedAnchorEntry> shareRequested => m_ShareRequested;

        [SerializeField]
        UnityEvent<MetaSharedAnchorEntry> m_RemoveRequested = new();
        public UnityEvent<MetaSharedAnchorEntry> removeRequested => m_RemoveRequested;

        [SerializeField]
        UnityEvent<MetaSharedAnchorEntry, bool> m_InSceneStateChanged = new();
        public UnityEvent<MetaSharedAnchorEntry, bool> inSceneStateStateChanged => m_InSceneStateChanged;

        [SerializeField]
        UnityEvent m_AnchorShared = new();
        public UnityEvent anchorShared => m_AnchorShared;

        public int entryId { get; private set; } = -1;

        public ARAnchor anchor { get; set; }

        public TrackableId trackableId => anchor?.trackableId ?? TrackableId.invalidId;

        public bool isShared { get; private set; }

        public bool isSynced { get; private set; }

        bool m_HasEntryIdBeenSet;
        bool m_IsSharedAnchorsSupported;

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
            m_ShareButton.SetEnabled(false);
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
            m_ShareButton.SetEnabled(resultStatus.IsError() && m_IsSharedAnchorsSupported && !isSynced);
            m_RemoveButton.SetEnabled(true);
        }

        /// <summary>
        /// Stops an in progress toast notification and shows a success or failed toast notification.
        /// </summary>
        /// <param name="success">The success or failed result to display.</param>
        public void ShowResult(bool success)
        {
            m_ToastNotification.ShowResult(success, success ? "Success" : "Failed");
            m_ShareButton.SetEnabled(!success && m_IsSharedAnchorsSupported && !isSynced);
            m_RemoveButton.SetEnabled(true);
        }

        /// <summary>
        /// Sets the shared status to <see langword="true"/> and disables the share button.
        /// </summary>
        public void SetIsSharedStatus()
        {
            isShared = true;
            m_StatusLabel.text = "Shared";
            m_AnchorShared?.Invoke();
        }

        /// <summary>
        /// Sets the shared status to <see langword="true"/> and hides the share button.
        /// </summary>
        public void SetIsSyncedStatus()
        {
            isSynced = true;
            m_StatusLabel.text = "Synced";
            m_ShareButton.gameObject.SetActive(false);
        }

        void Reset()
        {
            m_Toggle = GetComponentInChildren<Toggle>();
            m_IdLabel = GetComponentInChildren<TextMeshProUGUI>();
            m_ToastNotification = GetComponentInChildren<AnchorToastNotification>();
        }

        void Awake()
        {
            if (m_IdLabel == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_IdLabel)} is null."), this);

            if (m_StatusLabel == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_StatusLabel)} is null."), this);

            if (m_ToastNotification == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_ToastNotification)} is null."), this);

            if (m_Toggle == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_Toggle)} is null."), this);

            if (m_ShareButton == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_ShareButton)} is null."), this);

            if (m_RemoveButton == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_RemoveButton)} is null."), this);
        }

        void OnEnable()
        {
            m_Toggle.onValueChanged.AddListener(isOn =>
            {
                m_EntryToggled?.Invoke(this, isOn);
            });

            m_ShareButton.onClick.AddListener(() =>
            {
                m_ShareRequested?.Invoke(this);
            });

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
