using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.XR.ARSubsystems;
#if METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
using UnityEngine.XR.OpenXR.Features.Meta;
using UnityEngine.XR.OpenXR.NativeTypes;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MetaSharedAnchorsMenu : MonoBehaviour
    {
        [SerializeField]
        ARAnchorManager m_AnchorManager;

        [SerializeField]
        MetaBatchSharedAnchors m_MetaBatchSharedAnchors;

        [SerializeField]
        MetaSharedAnchorEntry m_MetaSharedAnchorEntryPrefab;

        [SerializeField]
        Transform m_ContentTransform;

        [SerializeField]
        UnityEvent m_SyncAnchorRequested = new();
        public UnityEvent syncAnchorRequested => m_SyncAnchorRequested;

        [SerializeField]
        UnityEvent<XRResultStatus, int> m_SyncAnchorCompleted = new();
        public UnityEvent<XRResultStatus, int> syncAnchorCompleted => m_SyncAnchorCompleted;

        [Header("Debug Options")]
        [SerializeField]
        bool m_VerboseLogging;

        Dictionary<int, MetaSharedAnchorEntry> m_AnchorEntryByEntryId = new();
        Dictionary<TrackableId, int> m_EntryIdByTrackableId = new();
        Dictionary<TrackableId, XRAnchor> m_LoadedSharedAnchors = new();

        int m_NextEntryId = 1;
        bool m_IsSharedAnchorsSupported;

#if METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
        /// <summary>
        /// Shares the ARAnchors associated with the <see cref="MetaSharedAnchorEntry"/> from
        /// <see cref="MetaBatchSharedAnchors.selectedEntries"/>.
        /// </summary>
        public async void ShareAnchors()
        {
            try
            {
                if (!m_IsSharedAnchorsSupported)
                    return;

                var entries = m_MetaBatchSharedAnchors.selectedEntries;
                var anchorsToShare = new List<ARAnchor>();
                foreach (var entry in entries)
                {
                    if (entry.isSynced)
                        continue;

                    entry.ShowInProgress();
                    anchorsToShare.Add(entry.anchor);
                }

                var subsystem = m_AnchorManager.subsystem as MetaOpenXRAnchorSubsystem;
                var results = new List<XRShareAnchorResult>();

                if (m_VerboseLogging)
                    Debug.Log($"Sharing anchors to group {subsystem!.sharedAnchorsGroupId}:\n{anchorsToShare}", this);

                await m_AnchorManager.TryShareAnchorsAsync(anchorsToShare, results);

                if (m_VerboseLogging)
                    Debug.Log($"Anchor share results:\n{results}", this);

                foreach (var result in results)
                {
                    var entryId = m_EntryIdByTrackableId[result.anchorId];
                    var entry = m_AnchorEntryByEntryId[entryId];

                    if (result.resultStatus.IsSuccess())
                        entry.SetIsSharedStatus();

                    entry.ShowResult(result.resultStatus);
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
        }

        /// <summary>
        /// Loads all anchors that were shared with group <see cref="MetaOpenXRAnchorSubsystem.sharedAnchorsGroupId"/>>.
        /// </summary>
        public async void LoadAllSharedAnchorsFromGroup()
        {
            if (!m_IsSharedAnchorsSupported)
                return;

            try
            {
                m_SyncAnchorRequested?.Invoke();
                var subsystem = m_AnchorManager.subsystem as MetaOpenXRAnchorSubsystem;
                var outputLoadedAnchors = new List<XRAnchor>();

                if (m_VerboseLogging)
                    Debug.Log($"Loading anchors from group {subsystem!.sharedAnchorsGroupId}", this);

                var resultStatus = await m_AnchorManager.TryLoadAllSharedAnchorsAsync(
                    outputLoadedAnchors,
                    loadedAnchors =>
                    {
                        foreach (var xrAnchor in loadedAnchors)
                        {
                            m_LoadedSharedAnchors[xrAnchor.trackableId] = xrAnchor;
                        }
                    });

                if (m_VerboseLogging)
                    Debug.Log($"Loaded {outputLoadedAnchors.Count} anchors:\n{outputLoadedAnchors}", this);

                m_SyncAnchorCompleted?.Invoke(resultStatus, outputLoadedAnchors.Count);
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
        }
#endif

        /// <summary>
        /// Removes the ARAnchors associated with the <see cref="MetaSharedAnchorEntry"/> from
        /// <see cref="MetaBatchSharedAnchors.selectedEntries"/>.
        /// </summary>
        public void RemoveAnchors()
        {
            var entries = m_MetaBatchSharedAnchors.selectedEntries;
            foreach (var entry in entries)
            {
                var success = m_AnchorManager.TryRemoveAnchor(entry.anchor);
                entry.ShowResult(success);
            }
        }

        void Reset()
        {
            m_AnchorManager = FindAnyObjectByType<ARAnchorManager>();
            m_MetaBatchSharedAnchors = FindAnyObjectByType<MetaBatchSharedAnchors>();
        }

        void Awake()
        {
            if (m_AnchorManager == null)
            {
                Debug.LogError($"{nameof(m_AnchorManager)} is null.", this);
                return;
            }

            if (m_MetaBatchSharedAnchors == null)
            {
                Debug.LogError($"{nameof(m_MetaBatchSharedAnchors)} is null.", this);
                return;
            }

            if (m_MetaSharedAnchorEntryPrefab == null)
            {
                Debug.LogError($"{nameof(m_MetaSharedAnchorEntryPrefab)} is null.", this);
                return;
            }

            if (m_ContentTransform == null)
            {
                Debug.LogError($"{nameof(m_ContentTransform)} is null.", this);
                return;
            }

            m_AnchorManager.trackablesChanged.AddListener(OnTrackablesChanged);

#if METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
            if (m_AnchorManager.subsystem is MetaOpenXRAnchorSubsystem metaAnchorSubsystem)
                m_IsSharedAnchorsSupported = metaAnchorSubsystem.isSharedAnchorsSupported == Supported.Supported;
#endif
        }

        void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARAnchor> eventArgs)
        {
            foreach (var anchor in eventArgs.added)
            {
                if (m_LoadedSharedAnchors.ContainsKey(anchor.trackableId))
                {
                    AddLoadedSharedAnchorEntry(anchor);
                    m_LoadedSharedAnchors.Remove(anchor.trackableId);
                }
                else
                {
                    AddLocallyCreatedAnchorEntry(anchor);
                }
            }

            foreach (var (trackableId, _) in eventArgs.removed)
            {
                var entryIntId = m_EntryIdByTrackableId[trackableId];
                CheckToRemoveEntry(entryIntId);
            }
        }

        void AddLoadedSharedAnchorEntry(ARAnchor anchor)
        {
            if (m_EntryIdByTrackableId.ContainsKey(anchor.trackableId))
                return;

            var entry = Instantiate(m_MetaSharedAnchorEntryPrefab, m_ContentTransform);

            entry.removeRequested.AddListener(RemoveAnchor);
            entry.SetIsSyncedStatus();

            var entryId = m_NextEntryId++;
            entry.anchor = anchor;
            entry.SetEntryId(entryId);
            m_AnchorEntryByEntryId.Add(entryId, entry);
            m_EntryIdByTrackableId.Add(anchor.trackableId, entryId);
            m_MetaBatchSharedAnchors.EntryAdded(entry);
        }

        void AddLocallyCreatedAnchorEntry(ARAnchor anchor)
        {
            if (m_EntryIdByTrackableId.ContainsKey(anchor.trackableId))
                return;

            var entry = Instantiate(m_MetaSharedAnchorEntryPrefab, m_ContentTransform);

            entry.removeRequested.AddListener(RemoveAnchor);
            entry.shareRequested.AddListener(ShareAnchor);

            var entryId = m_NextEntryId++;
            entry.anchor = anchor;
            entry.SetEntryId(entryId);

            m_AnchorEntryByEntryId.Add(entryId, entry);
            m_EntryIdByTrackableId.Add(anchor.trackableId, entryId);
            m_MetaBatchSharedAnchors.EntryAdded(entry);
        }

        void CheckToRemoveEntry(int entryId)
        {
            var found = m_AnchorEntryByEntryId.TryGetValue(entryId, out var entry);
            if (!found)
                return;

            entry.removeRequested.RemoveListener(RemoveAnchor);
            entry.shareRequested.RemoveListener(ShareAnchor);

            m_EntryIdByTrackableId.Remove(entry.trackableId);
            m_AnchorEntryByEntryId.Remove(entryId);
            m_MetaBatchSharedAnchors.EntryRemoved(entry);
            Destroy(entry.gameObject);
        }

#if METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
        async void ShareAnchor(MetaSharedAnchorEntry entry)
        {
            if (!m_IsSharedAnchorsSupported || entry == null)
                return;

            try
            {
                entry.ShowInProgress();
                var subsystem = m_AnchorManager.subsystem as MetaOpenXRAnchorSubsystem;

                if (m_VerboseLogging)
                    Debug.Log($"Sharing anchor to group {subsystem!.sharedAnchorsGroupId}:\n{entry.anchor}", this);

                var resultStatus = await m_AnchorManager.TryShareAnchorAsync(entry.anchor);

                if (m_VerboseLogging)
                    Debug.Log($"Share result: ({resultStatus.statusCode}, {(XrResult)resultStatus.nativeStatusCode}", this);

                entry.ShowResult(resultStatus);

                if (resultStatus.IsSuccess())
                    entry.SetIsSharedStatus();
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
        }
#else
        void ShareAnchor(MetaSharedAnchorEntry entry)
        {
            // unsupported, do nothing
        }
#endif

        void RemoveAnchor(MetaSharedAnchorEntry entry)
        {
            if (entry == null)
                return;

            var success = m_AnchorManager.TryRemoveAnchor(entry.anchor);
            entry.ShowResult(success);
        }
    }
}
