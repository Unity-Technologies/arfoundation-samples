using System;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;
#if METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
using UnityEngine.XR.OpenXR.NativeTypes;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MetaPersistentAnchorsMenu : MonoBehaviour
    {
        [SerializeField]
        ARAnchorManager m_AnchorManager;

        [SerializeField]
        MetaBatchPersistentAnchors m_MetaBatchPersistentAnchors;

        [SerializeField]
        MetaPersistentAnchorEntry m_MetaPersistentAnchorEntryPrefab;

        [SerializeField]
        Transform m_ContentTransform;

        [Header("Debug Options")]
        [SerializeField]
        bool m_VerboseLogging;

        SaveAndLoadAnchorDataToFile m_SaveAndLoadAnchorDataToFile;

        Dictionary<int, MetaPersistentAnchorEntry> m_AnchorEntryByEntryId = new();
        Dictionary<TrackableId, int> m_EntryIdByTrackableId = new();
        Dictionary<SerializableGuid, int> m_EntryIdBySavedAnchorGuid = new();

        int m_NextEntryId = 1;

        /// <summary>
        /// Saves the ARAnchors associated with the <see cref="MetaPersistentAnchorEntry"/> from <see cref="MetaBatchPersistentAnchors.selectedEntries"/>.
        /// </summary>
        public async void SaveAnchors()
        {
            if (!m_AnchorManager.subsystem.subsystemDescriptor.supportsSaveAnchor)
                return;

            var anchorsToSave = new List<ARAnchor>();
            var entries = m_MetaBatchPersistentAnchors.selectedEntries;
            foreach (var entry in entries)
            {
                if (!entry.isInScene)
                    continue;

                entry.ShowInProgress();
                anchorsToSave.Add(entry.anchor);
            }

            var results = new List<ARSaveOrLoadAnchorResult>();

            if (m_VerboseLogging)
                Debug.Log($"Saving anchors:\n{anchorsToSave}", this);

            await m_AnchorManager.TrySaveAnchorsAsync(anchorsToSave, results);

            if (m_VerboseLogging)
                Debug.Log($"Save results:\n{results}", this);

            var dateTime = DateTime.Now;
            foreach (var result in results)
            {
                var entryId = m_EntryIdByTrackableId[result.anchor.trackableId];
                var entry = m_AnchorEntryByEntryId[entryId];

                if (result.resultStatus.IsSuccess())
                {
                    entry.UpdateSaveStatus(true, result.savedAnchorGuid, dateTime);
                    m_EntryIdBySavedAnchorGuid.TryAdd(result.savedAnchorGuid, entryId);

                    await m_SaveAndLoadAnchorDataToFile.SaveAnchorIdAsync(
                        result.savedAnchorGuid,
                        dateTime);
                }

                entry.ShowResult(result.resultStatus);
            }
        }

        /// <summary>
        /// Loads the ARAnchors associated with the <see cref="MetaPersistentAnchorEntry"/> from <see cref="MetaBatchPersistentAnchors.selectedEntries"/>.
        /// </summary>
        public async void LoadAnchors()
        {
            if (!m_AnchorManager.subsystem.subsystemDescriptor.supportsLoadAnchor)
                return;

            var savedAnchorGuidsToLoad = new List<SerializableGuid>();
            var entries = m_MetaBatchPersistentAnchors.selectedEntries;
            foreach (var entry in entries)
            {
                if (!entry.isSaved)
                    continue;

                entry.ShowInProgress();
                savedAnchorGuidsToLoad.Add(entry.savedAnchorGuid);
            }

            var results = new List<ARSaveOrLoadAnchorResult>();

            if (m_VerboseLogging)
                Debug.Log($"Loading anchors:\n{savedAnchorGuidsToLoad}", this);

            await m_AnchorManager.TryLoadAnchorsAsync(
                savedAnchorGuidsToLoad,
                results,
                incrementalResults =>
                {
                    foreach (var result in incrementalResults)
                    {
                        var entryId = m_EntryIdBySavedAnchorGuid[result.savedAnchorGuid];
                        var entry = m_AnchorEntryByEntryId[entryId];

                        entry.ShowResult(result.resultStatus);
                        entry.UpdateInSceneStatus(result.anchor);
                        m_EntryIdByTrackableId.TryAdd(result.anchor.trackableId, entryId);
                    }
                });

            if (m_VerboseLogging)
                Debug.Log($"Load results:\n{results}", this);
        }

        /// <summary>
        /// Erases the ARAnchors associated with the <see cref="MetaPersistentAnchorEntry"/> from <see cref="MetaBatchPersistentAnchors.selectedEntries"/>.
        /// </summary>
        public async void EraseAnchors()
        {
            if (!m_AnchorManager.subsystem.subsystemDescriptor.supportsEraseAnchor)
                return;

            var savedAnchorGuidsToErase = new List<SerializableGuid>();
            var entries = m_MetaBatchPersistentAnchors.selectedEntries;
            foreach (var entry in entries)
            {
                if (!entry.isSaved)
                    continue;

                entry.ShowInProgress();
                savedAnchorGuidsToErase.Add(entry.savedAnchorGuid);
            }

            var results = new List<XREraseAnchorResult>();

            if (m_VerboseLogging)
                Debug.Log($"Erasing anchors:\n{savedAnchorGuidsToErase}");

            await m_AnchorManager.TryEraseAnchorsAsync(savedAnchorGuidsToErase, results);

            if (m_VerboseLogging)
                Debug.Log($"Erase results:\n{results}");

            var removedAnchorsEntryIds = new List<int>();
            foreach (var result in results)
            {
                var entryId = m_EntryIdBySavedAnchorGuid[result.savedAnchorGuid];
                var entry = m_AnchorEntryByEntryId[entryId];

                if (result.resultStatus.IsSuccess())
                {
                    await m_SaveAndLoadAnchorDataToFile.EraseAnchorIdAsync(result.savedAnchorGuid);
                    entry.UpdateSaveStatus(false, TrackableId.invalidId, default);
                    m_EntryIdBySavedAnchorGuid.Remove(result.savedAnchorGuid);
                    removedAnchorsEntryIds.Add(entry.entryId);
                }

                entry.ShowResult(result.resultStatus);
            }

            foreach (var entryId in removedAnchorsEntryIds)
            {
                CheckToRemoveEntry(entryId);
            }
        }

        /// <summary>
        /// Removes the ARAnchors associated with the <see cref="MetaPersistentAnchorEntry"/> from <see cref="MetaBatchPersistentAnchors.selectedEntries"/>.
        /// </summary>
        public void RemoveAnchors()
        {
            var entries = m_MetaBatchPersistentAnchors.selectedEntries;
            foreach (var entry in entries)
            {
                if (!entry.isInScene)
                    continue;

                if (m_VerboseLogging)
                    Debug.Log($"Removing anchor:\n{entry.anchor}", this);

                var success = m_AnchorManager.TryRemoveAnchor(entry.anchor);

                if (m_VerboseLogging)
                    Debug.Log($"Remove result: {success.ToString()}", this);

                entry.ShowResult(success);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static async void InitializeDateTime()
        {
            try
            {
                // Initiate time zone look up so the system can cache it for faster calls.
                // Run on background thread so no delay startup.
                await Awaitable.BackgroundThreadAsync();
                _ = DateTime.Now;
                await Awaitable.MainThreadAsync();
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
        }

        void Reset()
        {
            m_AnchorManager = FindAnyObjectByType<ARAnchorManager>();
            m_MetaBatchPersistentAnchors = FindAnyObjectByType<MetaBatchPersistentAnchors>();
        }

        void Awake()
        {
            if (m_AnchorManager == null)
            {
                Debug.LogError($"{nameof(m_AnchorManager)} is null.", this);
                return;
            }

            if (m_MetaBatchPersistentAnchors == null)
            {
                Debug.LogError($"{nameof(m_MetaBatchPersistentAnchors)} is null.", this);
                return;
            }

            if (m_MetaPersistentAnchorEntryPrefab == null)
            {
                Debug.LogError($"{nameof(m_MetaPersistentAnchorEntryPrefab)} is null.", this);
                return;
            }

            if (m_ContentTransform == null)
            {
                Debug.LogError($"{nameof(m_ContentTransform)} is null.", this);
                return;
            }

            m_AnchorManager.trackablesChanged.AddListener(OnTrackablesChanged);
        }

        void Start()
        {
            PopulateMenuEntriesFromFile();
        }

        async void PopulateMenuEntriesFromFile()
        {
            try
            {
                m_SaveAndLoadAnchorDataToFile ??= new SaveAndLoadAnchorDataToFile();
                var savedAnchorData = await m_SaveAndLoadAnchorDataToFile.GetSavedAnchorsDataAsync();
                foreach (var (savedAnchorGuid, dateTime) in savedAnchorData)
                {
                    AddSavedEntry(savedAnchorGuid, dateTime);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARAnchor> eventArgs)
        {
            foreach (var anchor in eventArgs.added)
                AddNewEntry(anchor);

            foreach (var (trackableId, _) in eventArgs.removed)
            {
                var entryIntId = m_EntryIdByTrackableId[trackableId];
                var entry = m_AnchorEntryByEntryId[entryIntId];
                entry.UpdateInSceneStatus(null);
                CheckToRemoveEntry(entryIntId);
            }
        }

        void AddNewEntry(ARAnchor anchor)
        {
            if (m_EntryIdByTrackableId.ContainsKey(anchor.trackableId))
                return;

            var entry = Instantiate(m_MetaPersistentAnchorEntryPrefab, m_ContentTransform);
            entry.UpdateInSceneStatus(anchor);

            entry.saveRequested.AddListener(SaveAnchor);
            entry.loadRequested.AddListener(LoadAnchor);
            entry.eraseRequested.AddListener(EraseAnchor);
            entry.removeRequested.AddListener(RemoveAnchor);

            var entryId = m_NextEntryId++;
            entry.SetEntryId(entryId);

            if (!m_AnchorManager.subsystem.subsystemDescriptor.supportsSaveAnchor ||
                !m_AnchorManager.subsystem.subsystemDescriptor.supportsLoadAnchor)
                entry.DisableSaveAndLoadButtons();

            if (!m_AnchorManager.subsystem.subsystemDescriptor.supportsEraseAnchor)
                entry.DisableEraseButton();

            m_AnchorEntryByEntryId.Add(entryId, entry);
            m_EntryIdByTrackableId.Add(anchor.trackableId, entryId);
            m_MetaBatchPersistentAnchors.EntryAdded(entry);
        }

        void AddSavedEntry(SerializableGuid savedAnchorGuid, DateTime dateTime)
        {
            if (m_EntryIdBySavedAnchorGuid.ContainsKey(savedAnchorGuid))
                return;

            var entry = Instantiate(m_MetaPersistentAnchorEntryPrefab, m_ContentTransform);
            entry.UpdateSaveStatus(true, savedAnchorGuid, dateTime);

            entry.saveRequested.AddListener(SaveAnchor);
            entry.loadRequested.AddListener(LoadAnchor);
            entry.eraseRequested.AddListener(EraseAnchor);
            entry.removeRequested.AddListener(RemoveAnchor);

            var entryId = m_NextEntryId++;
            entry.SetEntryId(entryId);

            if (!m_AnchorManager.subsystem.subsystemDescriptor.supportsSaveAnchor ||
                !m_AnchorManager.subsystem.subsystemDescriptor.supportsLoadAnchor)
                entry.DisableSaveAndLoadButtons();

            if (!m_AnchorManager.subsystem.subsystemDescriptor.supportsEraseAnchor)
                entry.DisableEraseButton();

            m_AnchorEntryByEntryId.Add(entryId, entry);
            m_EntryIdBySavedAnchorGuid.Add(savedAnchorGuid, entryId);
            m_MetaBatchPersistentAnchors.EntryAdded(entry);
        }

        void CheckToRemoveEntry(int entryId)
        {
            var found = m_AnchorEntryByEntryId.TryGetValue(entryId, out var entry);
            if (!found)
                return;

            if (entry.isInScene || entry.isSaved)
                return;

            entry.saveRequested.RemoveListener(SaveAnchor);
            entry.loadRequested.RemoveListener(LoadAnchor);
            entry.eraseRequested.RemoveListener(EraseAnchor);
            entry.removeRequested.RemoveListener(RemoveAnchor);

            m_EntryIdByTrackableId.Remove(entry.trackableId);
            m_EntryIdBySavedAnchorGuid.Remove(entry.savedAnchorGuid);
            m_AnchorEntryByEntryId.Remove(entryId);
            m_MetaBatchPersistentAnchors.EntryRemoved(entry);
            Destroy(entry.gameObject);
        }

        async void SaveAnchor(MetaPersistentAnchorEntry entry)
        {
            if (!m_AnchorManager.subsystem.subsystemDescriptor.supportsSaveAnchor)
                return;

            try
            {
                if (entry == null || !entry.isInScene)
                    return;

                entry.ShowInProgress();

                if (m_VerboseLogging)
                    Debug.Log($"Saving anchor:\n{entry.anchor}", this);

                var result = await m_AnchorManager.TrySaveAnchorAsync(entry.anchor);

#if METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
                if (m_VerboseLogging)
                    Debug.Log($"Save result: ({result.status.statusCode}, {(XrResult)result.status.nativeStatusCode})", this);
#endif

                if (result.status.IsSuccess())
                {
                    entry.UpdateSaveStatus(true, result.value, DateTime.Now);
                    m_EntryIdBySavedAnchorGuid.TryAdd(result.value, entry.entryId);
                    await m_SaveAndLoadAnchorDataToFile.SaveAnchorIdAsync(result.value, DateTime.Now);
                }

                entry.ShowResult(result.status);
            }
            catch (OperationCanceledException)
            {
                // todo: need a UI state for canceled
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        async void LoadAnchor(MetaPersistentAnchorEntry entry)
        {
            if (!m_AnchorManager.subsystem.subsystemDescriptor.supportsLoadAnchor)
                return;

            try
            {
                if (entry == null || !entry.isSaved)
                    return;

                entry.ShowInProgress();

                if (m_VerboseLogging)
                    Debug.Log($"Loading anchor: {entry.savedAnchorGuid}", this);

                var result = await m_AnchorManager.TryLoadAnchorAsync(entry.savedAnchorGuid);

#if METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
                if (m_VerboseLogging)
                    Debug.Log($"Load result: ({result.status.statusCode}, {(XrResult)result.status.nativeStatusCode})", this);
#endif

                entry.ShowResult(result.status);

                if (result.status.IsSuccess())
                {
                    entry.UpdateInSceneStatus(result.value);
                    m_EntryIdByTrackableId.TryAdd(entry.anchor.trackableId, entry.entryId);
                }
            }
            catch (OperationCanceledException)
            {
                // todo: need a UI state for canceled
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        async void EraseAnchor(MetaPersistentAnchorEntry entry)
        {
            if (!m_AnchorManager.subsystem.subsystemDescriptor.supportsEraseAnchor)
                return;

            try
            {
                if (entry == null || !entry.isSaved)
                    return;

                entry.ShowInProgress();

                if (m_VerboseLogging)
                    Debug.Log($"Erasing anchor: {entry.savedAnchorGuid}", this);

                var resultStatus = await m_AnchorManager.TryEraseAnchorAsync(entry.savedAnchorGuid);

#if METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
                if (m_VerboseLogging)
                    Debug.Log($"Save result: ({resultStatus.statusCode}, {(XrResult)resultStatus.nativeStatusCode})", this);
#endif

                if (resultStatus.IsSuccess())
                {
                    await m_SaveAndLoadAnchorDataToFile.EraseAnchorIdAsync(entry.savedAnchorGuid);
                    entry.UpdateSaveStatus(false, TrackableId.invalidId, default);
                    m_EntryIdBySavedAnchorGuid.Remove(entry.savedAnchorGuid);
                    CheckToRemoveEntry(entry.entryId);
                }

                entry.ShowResult(resultStatus);
            }
            catch (OperationCanceledException)
            {
                // todo: need a UI state for canceled
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        void RemoveAnchor(MetaPersistentAnchorEntry entry)
        {
            if (entry == null || !entry.isInScene)
                return;

            if (m_VerboseLogging)
                Debug.Log($"Removing anchor:\n{entry.anchor}", this);

            var success = m_AnchorManager.TryRemoveAnchor(entry.anchor);

            if (m_VerboseLogging)
                Debug.Log($"Remove result: {success.ToString()}", this);

            entry.ShowResult(success);
        }
    }
}
