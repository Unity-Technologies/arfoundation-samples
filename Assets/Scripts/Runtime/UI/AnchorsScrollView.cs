using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using SerializableGuid = UnityEngine.XR.ARSubsystems.SerializableGuid;

#if UNITY_ANDROID && ARCORE_4_2_OR_NEWER && !UNITY_EDITOR
using UnityEngine.XR.ARCore;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class AnchorsScrollView : MonoBehaviour
    {
        static readonly Pool.ObjectPool<List<Awaitable>> s_VoidAwaitableLists = new(
            createFunc: () => new List<Awaitable>(),
            actionOnGet: null,
            actionOnRelease: null,
            actionOnDestroy: null,
            collectionCheck: false,
            defaultCapacity: 8,
            maxSize: 1024);

        static readonly Pool.ObjectPool<Dictionary<AsyncInstantiateOperation<AnchorScrollViewEntry>, ARSaveOrLoadAnchorResult>> s_AsyncInstantiateMaps = new(
            createFunc: () => new Dictionary<AsyncInstantiateOperation<AnchorScrollViewEntry>, ARSaveOrLoadAnchorResult>(),
            actionOnGet: null,
            actionOnRelease: null,
            actionOnDestroy: null,
            collectionCheck: false,
            defaultCapacity: 8,
            maxSize: 1024);

        static readonly Pool.ObjectPool<Dictionary<AsyncInstantiateOperation<AnchorScrollViewEntry>, SerializableGuid>> s_ErasedEntryAwaitablesMaps = new(
            createFunc: () => new Dictionary<AsyncInstantiateOperation<AnchorScrollViewEntry>, SerializableGuid>(),
            actionOnGet: null,
            actionOnRelease: null,
            actionOnDestroy: null,
            collectionCheck: false,
            defaultCapacity: 8,
            maxSize: 1024);

        static readonly Pool.ObjectPool<Dictionary<AsyncInstantiateOperation<AnchorScrollViewEntry>, ARAnchor>> s_AddNewEntryAwaitablesMaps = new(
            createFunc: () => new Dictionary<AsyncInstantiateOperation<AnchorScrollViewEntry>, ARAnchor>(),
            actionOnGet: null,
            actionOnRelease: null,
            actionOnDestroy: null,
            collectionCheck: false,
            defaultCapacity: 8,
            maxSize: 1024);

        [Header("References")]
        [SerializeField]
        ARAnchorManager m_AnchorManager;

        [SerializeField]
        Transform m_ContentTransform;

        [SerializeField]
        Transform m_NewAnchorsLabel;

        [SerializeField]
        Transform m_SavedAnchorsLabel;

        [SerializeField]
        Transform m_SavedAnchorsNotSupportedLabel;

        [SerializeField]
        Button m_SaveAllAnchorsButton;

        [SerializeField]
        Button m_LoadAllAnchorsButton;

        [SerializeField]
        Button m_EraseAllAnchorsButton;

        [Header("Prefabs")]
        [SerializeField]
        AnchorScrollViewEntry m_NewAnchorEntryPrefab;

        [SerializeField]
        AnchorScrollViewEntry m_SavedAnchorEntryPrefab;

        [Header("Settings")]
        [SerializeField]
        float m_ResultDurationInSeconds = 2;

        int m_EntryCount = 1;
        HashSet<TrackableId> m_LoadRequests = new();
        Dictionary<TrackableId, AnchorScrollViewEntry> m_NewAnchorEntriesByAnchorId = new();
        Dictionary<SerializableGuid, AnchorScrollViewEntry> m_SavedAnchorEntriesBySavedAnchorGuid = new();
        Dictionary<TrackableId, SerializableGuid> m_SavedAnchorGuidByAnchorId = new();

        List<ARSaveOrLoadAnchorResult> m_SaveAnchorResults = new();
        List<XREraseAnchorResult> m_OutputEraseAnchorResults = new();

        bool m_SupportsGetSavedAnchorIds;
        bool m_SupportsSaveAndLoadAnchors;
        bool m_SupportsEraseAnchors;

        SaveAndLoadAnchorDataToFile m_SaveAndLoadAnchorDataToFile;

        void Awake()
        {
            if (m_AnchorManager == null)
            {
                m_AnchorManager = FindAnyObjectByType<ARAnchorManager>();
                if (m_AnchorManager == null)
                {
                    Debug.LogError($"{nameof(AnchorsScrollView)} component requires an {nameof(ARAnchorManager)}.", this);
                    enabled = false;
                    return;
                }

                Debug.LogWarning($"Null serialized field. Set the {nameof(ARAnchorManager)} reference on the " +
                    $"{nameof(AnchorsScrollView)} component for better performance.", this);
            }

            var descriptor = m_AnchorManager.descriptor;
            m_SupportsSaveAndLoadAnchors = descriptor.supportsSaveAnchor && descriptor.supportsLoadAnchor;

            if (!m_SupportsSaveAndLoadAnchors)
            {
                m_SavedAnchorsNotSupportedLabel.gameObject.SetActive(true);
                m_NewAnchorsLabel.gameObject.SetActive(false);
                m_SavedAnchorsLabel.gameObject.SetActive(false);

                m_SaveAllAnchorsButton.interactable = false;
                m_LoadAllAnchorsButton.interactable = false;
                m_EraseAllAnchorsButton.interactable = false;
                return;
            }

            m_SupportsEraseAnchors = descriptor.supportsEraseAnchor;
            m_SupportsGetSavedAnchorIds = descriptor.supportsGetSavedAnchorIds;
            InitializeUI();
        }

        void OnDestroy()
        {
            if (m_SupportsSaveAndLoadAnchors)
                m_AnchorManager.trackablesChanged.RemoveListener(OnAnchorsChanged);
        }

        public async void SaveAllAnchors()
        {
            if (!m_SupportsSaveAndLoadAnchors)
                return;

            var anchorsToSave = new List<ARAnchor>();
            foreach (var newAnchorEntry in m_NewAnchorEntriesByAnchorId.Values)
            {
                anchorsToSave.Add(newAnchorEntry.representedAnchor);
                newAnchorEntry.EnableActionButton(false);
                newAnchorEntry.StartActionLoadingAnimation();
            }

            await m_AnchorManager.TrySaveAnchorsAsync(anchorsToSave, m_SaveAnchorResults);

            var saveToFileAwaitables = s_VoidAwaitableLists.Get();
            var createSavedEntryAwaitables = s_AsyncInstantiateMaps.Get();
            foreach (var result in m_SaveAnchorResults)
            {
                if (result.resultStatus.IsError())
                    continue;

                var saveToFileAwaitable = SavePersistentAnchorGuidToFile(
                    result.resultStatus,
                    result.savedAnchorGuid);

                if (saveToFileAwaitable != null)
                    saveToFileAwaitables.Add(saveToFileAwaitable);

                if (!m_SavedAnchorEntriesBySavedAnchorGuid.ContainsKey(result.savedAnchorGuid))
                {
                    var saveEntryAwaitable = InstantiateAsync(m_SavedAnchorEntryPrefab, m_ContentTransform);
                    createSavedEntryAwaitables.Add(saveEntryAwaitable, result);
                }
            }

            foreach (var (createSaveEntryAwaitable, result) in createSavedEntryAwaitables)
            {
                var saveAnchorEntry = await createSaveEntryAwaitable;
                var newAnchorEntry = m_NewAnchorEntriesByAnchorId[result.anchor.trackableId];
                var isAnchorActive = newAnchorEntry.representedAnchor != null;
                SetupSavedAnchorEntry(
                    saveAnchorEntry[0],
                    result.savedAnchorGuid,
                    isAnchorActive,
                    DateTime.Now,
                    newAnchorEntry.AnchorDisplayText);

                UpdateSavedAnchorEntry(saveAnchorEntry[0], result.savedAnchorGuid, newAnchorEntry.representedAnchor);
            }

            foreach (var awaitable in saveToFileAwaitables)
            {
                await awaitable;
            }

            var showResultAwaitables = s_VoidAwaitableLists.Get();
            foreach (var result in m_SaveAnchorResults)
            {
                var entry = m_NewAnchorEntriesByAnchorId[result.anchor.trackableId];
                var showResultAwaitable = ShowEntryActionResult(entry, result.resultStatus, result.savedAnchorGuid);
                showResultAwaitables.Add(showResultAwaitable);
            }

            foreach (var awaitable in showResultAwaitables)
            {
                await awaitable;
            }

            saveToFileAwaitables.Clear();
            s_VoidAwaitableLists.Release(saveToFileAwaitables);

            createSavedEntryAwaitables.Clear();
            s_AsyncInstantiateMaps.Release(createSavedEntryAwaitables);
        }

        public async void LoadAllAnchors()
        {
            if (!m_SupportsSaveAndLoadAnchors)
                return;

            var anchorGuidsToLoad = new List<SerializableGuid>();
            foreach (var (savedAnchorGuid, anchorEntry) in m_SavedAnchorEntriesBySavedAnchorGuid)
            {
                anchorGuidsToLoad.Add(savedAnchorGuid);
                anchorEntry.EnableActionButton(false);
                anchorEntry.StartActionLoadingAnimation();
            }

            var loadAnchorResults = new List<ARSaveOrLoadAnchorResult>();
            await m_AnchorManager.TryLoadAnchorsAsync(
                anchorGuidsToLoad,
                loadAnchorResults,
                OnIncrementalLoadResultsAvailable);

            var awaitables = new List<Awaitable>();
            foreach (var loadAnchorResult in loadAnchorResults)
            {
                var entry = m_SavedAnchorEntriesBySavedAnchorGuid[loadAnchorResult.savedAnchorGuid];
                if (loadAnchorResult.resultStatus.IsSuccess())
                {
                    continue;
                }

                entry.StopActionLoadingAnimation();
                awaitables.Add(entry.ShowActionResult(false, m_ResultDurationInSeconds));
                entry.EnableActionButton(true);
            }

            foreach (var awaitable in awaitables)
            {
                await awaitable;
            }
        }

        async void OnIncrementalLoadResultsAvailable(ReadOnlyListSpan<ARSaveOrLoadAnchorResult> loadAnchorResults)
        {
            var showResultAwaitables = s_VoidAwaitableLists.Get();
            foreach (var loadAnchorResult in loadAnchorResults)
            {
                var entry = m_SavedAnchorEntriesBySavedAnchorGuid[loadAnchorResult.savedAnchorGuid];
                entry.representedAnchor = loadAnchorResult.anchor;
                entry.EnableActionButton(false);

                m_SavedAnchorGuidByAnchorId.TryAdd(
                    loadAnchorResult.anchor.trackableId,
                    loadAnchorResult.savedAnchorGuid);

                // add anchor id to the load request list so when the added anchor change event
                // is raised from the anchor trackable manager, we can know if an entry for this anchor already exists
                m_LoadRequests.Add(loadAnchorResult.anchor.trackableId);

                entry.StopActionLoadingAnimation();
                showResultAwaitables.Add(entry.ShowActionResult(true, m_ResultDurationInSeconds));
                entry.EnableActionButton(false);
            }

            foreach (var awaitable in showResultAwaitables)
            {
                await awaitable;
            }

            showResultAwaitables.Clear();
            s_VoidAwaitableLists.Release(showResultAwaitables);
        }

        public async void EraseAllAnchors()
        {
            if (!m_SupportsEraseAnchors)
                return;

            var anchorsToErase = new List<SerializableGuid>();
            foreach (var (savedAnchorGuid, anchorEntry) in m_SavedAnchorEntriesBySavedAnchorGuid)
            {
                anchorsToErase.Add(savedAnchorGuid);
                anchorEntry.EnableActionButton(false);
                anchorEntry.EnableEraseButton(false);
                anchorEntry.StartEraseLoadingAnimation();
            }

            await m_AnchorManager.TryEraseAnchorsAsync(anchorsToErase, m_OutputEraseAnchorResults);

            var showResultAwaitables = s_VoidAwaitableLists.Get();
            for (var i = 0; i < m_OutputEraseAnchorResults.Count; i += 1)
            {
                var savedAnchorEntry = m_SavedAnchorEntriesBySavedAnchorGuid[m_OutputEraseAnchorResults[i].savedAnchorGuid];
                var isAnchorInScene = savedAnchorEntry.representedAnchor != null;

                savedAnchorEntry.StopEraseLoadingAnimation();
                if (m_OutputEraseAnchorResults[i].resultStatus.IsError())
                {
                    var showFailedResultAwaitable = savedAnchorEntry.ShowEraseResult(false, m_ResultDurationInSeconds);
                    showResultAwaitables.Add(showFailedResultAwaitable);
                    savedAnchorEntry.EnableEraseButton(true);
                    savedAnchorEntry.EnableActionButton(!isAnchorInScene);
                    continue;
                }

                if (!m_SupportsGetSavedAnchorIds)
                {
                    var eraseAnchorAwaitable = m_SaveAndLoadAnchorDataToFile.EraseAnchorIdAsync(savedAnchorEntry.savedAnchorGuid);
                    showResultAwaitables.Add(eraseAnchorAwaitable);
                }

                var showSuccessResultAwaitable = savedAnchorEntry.ShowEraseResult(true, m_ResultDurationInSeconds);
                showResultAwaitables.Add(showSuccessResultAwaitable);
            }

            foreach (var awaitable in showResultAwaitables)
            {
                await awaitable;
            }

            showResultAwaitables.Clear();
            s_VoidAwaitableLists.Release(showResultAwaitables);

            var erasedEntryAwaitables = s_ErasedEntryAwaitablesMaps.Get();
            foreach (var eraseAnchorResult in m_OutputEraseAnchorResults)
            {
                var savedAnchorEntry = m_SavedAnchorEntriesBySavedAnchorGuid[eraseAnchorResult.savedAnchorGuid];
                var isAnchorInScene = savedAnchorEntry.representedAnchor != null;
                if (isAnchorInScene)
                {
                    var addNewEntryAwaitable = InstantiateAsync(m_NewAnchorEntryPrefab, m_ContentTransform);
                    erasedEntryAwaitables.Add(addNewEntryAwaitable, eraseAnchorResult.savedAnchorGuid);
                }
                else
                {
                    RemoveSavedAnchorEntry(savedAnchorEntry);
                }
            }

            foreach (var (awaitable, savedAnchorGuid) in erasedEntryAwaitables)
            {
                var newAnchorEntry = await awaitable;
                var savedAnchorEntry = m_SavedAnchorEntriesBySavedAnchorGuid[savedAnchorGuid];
                SetupNewAnchorEntry(
                    newAnchorEntry[0],
                    savedAnchorEntry.representedAnchor,
                    savedAnchorEntry.AnchorDisplayText);

                RemoveSavedAnchorEntry(savedAnchorEntry);
            }

            erasedEntryAwaitables.Clear();
            s_ErasedEntryAwaitablesMaps.Release(erasedEntryAwaitables);
        }

        async void InitializeUI()
        {
            var addSavedEntryAwaitables = new Dictionary<AsyncInstantiateOperation<AnchorScrollViewEntry>, SerializableGuid>();
            Dictionary<SerializableGuid, DateTime> savedAnchorData = new();
            if (m_SupportsGetSavedAnchorIds)
            {
                var result = await m_AnchorManager.TryGetSavedAnchorIdsAsync(Allocator.Temp);
                if (result.status.IsSuccess())
                {
                    foreach (var savedAnchorGuid in result.value)
                    {
                        if (!m_SavedAnchorEntriesBySavedAnchorGuid.ContainsKey(savedAnchorGuid))
                        {
                            var saveEntryAwaitable = InstantiateAsync(m_SavedAnchorEntryPrefab, m_ContentTransform);
                            addSavedEntryAwaitables.Add(saveEntryAwaitable, savedAnchorGuid);
                        }
                    }
                }
            }
            else
            {
                m_SaveAndLoadAnchorDataToFile ??= new SaveAndLoadAnchorDataToFile();
                savedAnchorData = await m_SaveAndLoadAnchorDataToFile.GetSavedAnchorsDataAsync();
                foreach (var savedAnchorGuid in savedAnchorData.Keys)
                {
                    if (!m_SavedAnchorEntriesBySavedAnchorGuid.ContainsKey(savedAnchorGuid))
                    {
                        var saveEntryAwaitable = InstantiateAsync(m_SavedAnchorEntryPrefab, m_ContentTransform);
                        addSavedEntryAwaitables.Add(saveEntryAwaitable, savedAnchorGuid);
                    }
                }
            }

            foreach (var (saveEntryAwaitable, serializableGuid) in addSavedEntryAwaitables)
            {
                DateTime dateTime = default;
                if (!m_SupportsGetSavedAnchorIds)
                    dateTime = savedAnchorData[serializableGuid];

                var savedAnchorEntry = await saveEntryAwaitable;
                SetupSavedAnchorEntry(
                    savedAnchorEntry[0],
                    serializableGuid,
                    false,
                    dateTime,
                    $"Anchor {m_EntryCount}");

                m_EntryCount += 1;
            }

            addSavedEntryAwaitables.Clear();

            var addNewEntryAwaitables = s_AddNewEntryAwaitablesMaps.Get();
            foreach (var anchor in m_AnchorManager.trackables)
            {
                var addNewEntryAwaitable = InstantiateAsync(m_NewAnchorEntryPrefab, m_ContentTransform);
                addNewEntryAwaitables.Add(addNewEntryAwaitable, anchor);
            }

            m_AnchorManager.trackablesChanged.AddListener(OnAnchorsChanged);
            foreach (var (awaitable, anchor) in addNewEntryAwaitables)
            {
                var newAnchorEntry = await awaitable;
                SetupNewAnchorEntry(
                    newAnchorEntry[0],
                    anchor,
                    null);
            }

            addNewEntryAwaitables.Clear();
            s_AddNewEntryAwaitablesMaps.Release(addNewEntryAwaitables);
        }

        async void OnAnchorsChanged(ARTrackablesChangedEventArgs<ARAnchor> changes)
        {
            var addNewEntryAwaitables = s_AddNewEntryAwaitablesMaps.Get();
            foreach (var anchor in changes.added)
            {
                if (m_LoadRequests.Contains(anchor.trackableId))
                {
                    m_LoadRequests.Remove(anchor.trackableId);
                }
                else
                {
                    var addNewEntryAwaitable = InstantiateAsync(m_NewAnchorEntryPrefab, m_ContentTransform);
                    addNewEntryAwaitables.Add(addNewEntryAwaitable, anchor);
                }
            }

            foreach (var (awaitable, anchor) in addNewEntryAwaitables)
            {
                var newAnchorEntry = await awaitable;
                SetupNewAnchorEntry(
                    newAnchorEntry[0],
                    anchor,
                    null);
            }

            addNewEntryAwaitables.Clear();
            s_AddNewEntryAwaitablesMaps.Release(addNewEntryAwaitables);

            foreach (var (anchorId, _) in changes.removed)
            {
                var doesSavedEntryExist = m_SavedAnchorGuidByAnchorId.TryGetValue(anchorId, out var savedAnchorGuid);
                if (doesSavedEntryExist &&
                    m_SavedAnchorEntriesBySavedAnchorGuid.TryGetValue(savedAnchorGuid, out var savedAnchorEntry))
                {
                    savedAnchorEntry.EnableActionButton(true);
                }
                else if (m_NewAnchorEntriesByAnchorId.TryGetValue(anchorId, out var newAnchorEntry))
                {
                    RemoveNewAnchorEntry(newAnchorEntry);
                }
            }
        }

        void RemoveNewAnchorEntry(AnchorScrollViewEntry entry)
        {
            entry.requestAction.RemoveListener(RequestSaveAnchor);
            m_NewAnchorEntriesByAnchorId.Remove(entry.representedAnchor.trackableId);
            UnityObjectUtils.Destroy(entry.gameObject);
        }

        void SetupNewAnchorEntry(AnchorScrollViewEntry newAnchorEntry, ARAnchor anchor, string displayLabelText)
        {
            newAnchorEntry.transform.localRotation = Quaternion.identity;
            newAnchorEntry.transform.localPosition = Vector3.zero;
            var lastNewAnchorEntryIndex = m_SavedAnchorsLabel.GetSiblingIndex();

            newAnchorEntry.transform.SetSiblingIndex(lastNewAnchorEntryIndex);
            newAnchorEntry.SetDisplayedAnchorLabel(displayLabelText ?? $"Anchor {m_EntryCount}");
            newAnchorEntry.representedAnchor = anchor;
            newAnchorEntry.requestAction.AddListener(RequestSaveAnchor);

            m_NewAnchorEntriesByAnchorId.Add(anchor.trackableId, newAnchorEntry);
            if (displayLabelText == null)
                m_EntryCount += 1;
        }

        void SetupSavedAnchorEntry(
            AnchorScrollViewEntry savedAnchorEntry,
            SerializableGuid savedAnchorGuid,
            bool isAnchorActive,
            DateTime savedDateTime = default,
            string displayLabelText = null)
        {
            savedAnchorEntry.transform.localRotation = Quaternion.identity;
            savedAnchorEntry.transform.localPosition = Vector3.zero;
            savedAnchorEntry.transform.SetAsLastSibling();
            savedAnchorEntry.SetDisplayedAnchorLabel(displayLabelText ?? $"Anchor {m_EntryCount}");

            if (!m_SupportsGetSavedAnchorIds && savedDateTime != default)
                savedAnchorEntry.SetAnchorSavedDateTime(savedDateTime);

            savedAnchorEntry.savedAnchorGuid = savedAnchorGuid;
            savedAnchorEntry.requestAction.AddListener(RequestLoadAnchor);
            savedAnchorEntry.EnableActionButton(!isAnchorActive);

            if (m_SupportsEraseAnchors)
            {
                savedAnchorEntry.requestEraseAnchor.AddListener(RequestEraseAnchor);
                savedAnchorEntry.EnableEraseButton(true);
                savedAnchorEntry.ShowEraseButton(true);
            }

            if (displayLabelText == null)
                m_EntryCount += 1;

            m_SavedAnchorEntriesBySavedAnchorGuid.Add(savedAnchorGuid, savedAnchorEntry);
        }

        void RemoveSavedAnchorEntry(AnchorScrollViewEntry entry)
        {
            entry.requestAction.RemoveListener(RequestLoadAnchor);

            if (m_SupportsEraseAnchors)
                entry.requestEraseAnchor.RemoveListener(RequestEraseAnchor);

            m_SavedAnchorEntriesBySavedAnchorGuid.Remove(entry.savedAnchorGuid);

            if (entry.representedAnchor != null)
                m_SavedAnchorGuidByAnchorId.Remove(entry.representedAnchor.trackableId);

            Destroy(entry.gameObject);
        }

        async void RequestSaveAnchor(AnchorScrollViewEntry newAnchorEntry)
        {
            var representedAnchor = newAnchorEntry.representedAnchor;

#if UNITY_ANDROID && ARCORE_4_2_OR_NEWER && !UNITY_EDITOR
            if (m_AnchorManager.subsystem is ARCoreAnchorSubsystem arCoreAnchorSubsystem)
            {
                var quality = ArFeatureMapQuality.AR_FEATURE_MAP_QUALITY_INSUFFICIENT;
                var resultStatus = arCoreAnchorSubsystem.EstimateFeatureMapQualityForHosting(representedAnchor.trackableId, ref quality);

                if (!resultStatus.IsSuccess())
                {
                    Debug.LogError("An error occurred while attempting to check the feature map quality of the anchor.");
                    return;
                }

                if (quality == ArFeatureMapQuality.AR_FEATURE_MAP_QUALITY_INSUFFICIENT)
                {
                    Debug.LogWarning("Anchor map quality is insufficient. Save the anchor when the quality improves.");
                    return;
                }
            }
#endif

            newAnchorEntry.EnableActionButton(false);
            newAnchorEntry.StartActionLoadingAnimation();

            var result = await m_AnchorManager.TrySaveAnchorAsync(newAnchorEntry.representedAnchor, newAnchorEntry.cancellationTokenSource.Token);
            var savedAnchorGuid = result.value;
            var saveToFileAwaitable = SavePersistentAnchorGuidToFile(result.status, savedAnchorGuid);

            AsyncInstantiateOperation<AnchorScrollViewEntry> saveEntryAwaitable = null;
            if (result.status.IsSuccess() && !m_SavedAnchorEntriesBySavedAnchorGuid.ContainsKey(savedAnchorGuid))
            {
                saveEntryAwaitable = InstantiateAsync(m_SavedAnchorEntryPrefab, m_ContentTransform);
            }

            if (saveEntryAwaitable != null)
            {
                if (saveToFileAwaitable != null)
                    await saveToFileAwaitable;

                var savedAnchorEntry = await saveEntryAwaitable;
                var isAnchorActive = newAnchorEntry.representedAnchor != null;
                SetupSavedAnchorEntry(
                    savedAnchorEntry[0],
                    savedAnchorGuid,
                    isAnchorActive,
                    DateTime.Now,
                    newAnchorEntry.AnchorDisplayText);

                UpdateSavedAnchorEntry(savedAnchorEntry[0], savedAnchorGuid, newAnchorEntry.representedAnchor);
            }

            await ShowEntryActionResult(newAnchorEntry, result.status, result.value);
        }

        Awaitable SavePersistentAnchorGuidToFile(XRResultStatus resultStatus, TrackableId guid)
        {
            var wasSaveSuccessful = resultStatus.IsSuccess();
            if (m_SupportsGetSavedAnchorIds || !wasSaveSuccessful)
            {
                return null;
            }

            m_SaveAndLoadAnchorDataToFile ??= new SaveAndLoadAnchorDataToFile();
            return m_SaveAndLoadAnchorDataToFile.SaveAnchorIdAsync(guid, DateTime.Now);
        }

        void UpdateSavedAnchorEntry(
            AnchorScrollViewEntry savedAnchorEntry,
            SerializableGuid savedAnchorGuid,
            ARAnchor anchor)
        {
            m_SavedAnchorEntriesBySavedAnchorGuid.TryAdd(savedAnchorGuid, savedAnchorEntry);
            m_SavedAnchorGuidByAnchorId.TryAdd(anchor.trackableId, savedAnchorGuid);
            savedAnchorEntry.gameObject.SetActive(false);
            savedAnchorEntry.representedAnchor = anchor;
        }

        Awaitable ShowEntryActionResult(
            AnchorScrollViewEntry newAnchorEntry,
            XRResultStatus resultStatus,
            SerializableGuid savedAnchorGuid)
        {
            newAnchorEntry.StopActionLoadingAnimation();
            var wasSaveSuccessful = resultStatus.IsSuccess();
            var awaitable = newAnchorEntry.ShowActionResult(wasSaveSuccessful, m_ResultDurationInSeconds);
            newAnchorEntry.EnableActionButton(true);

            if (wasSaveSuccessful)
            {
                // we remove the entry in the new entry section of the scroll view
                // and enable the entry in the saved anchor section of the scroll view
                RemoveNewAnchorEntry(newAnchorEntry);
                if (m_SavedAnchorEntriesBySavedAnchorGuid.TryGetValue(savedAnchorGuid, out var savedAnchorEntry))
                {
                    savedAnchorEntry.gameObject.SetActive(true);
                }
            }

            return awaitable;
        }

        async void RequestLoadAnchor(AnchorScrollViewEntry entry)
        {
            entry.EnableActionButton(false);
            entry.StartActionLoadingAnimation();

            var result = await m_AnchorManager.TryLoadAnchorAsync(entry.savedAnchorGuid, entry.cancellationTokenSource.Token);
            var wasLoadSuccessful = result.status.IsSuccess();
            if (wasLoadSuccessful)
            {
                entry.representedAnchor = result.value;
                entry.EnableActionButton(false);
                // add anchor id to load request list so when the added anchor change event
                // is raised, we can know an entry for this anchor already exists
                m_LoadRequests.Add(entry.representedAnchor.trackableId);
                m_SavedAnchorGuidByAnchorId.TryAdd(entry.representedAnchor.trackableId, entry.savedAnchorGuid);
            }

            entry.StopActionLoadingAnimation();
            await entry.ShowActionResult(wasLoadSuccessful, m_ResultDurationInSeconds);
            entry.EnableActionButton(!wasLoadSuccessful);
        }

        async void RequestEraseAnchor(AnchorScrollViewEntry savedAnchorEntry)
        {
            savedAnchorEntry.EnableActionButton(false);
            savedAnchorEntry.EnableEraseButton(false);
            savedAnchorEntry.StartEraseLoadingAnimation();

            var result = await m_AnchorManager.TryEraseAnchorAsync(savedAnchorEntry.savedAnchorGuid);
            var isAnchorInScene = savedAnchorEntry.representedAnchor != null;

            if (!result.IsSuccess())
            {
                savedAnchorEntry.StopEraseLoadingAnimation();
                await savedAnchorEntry.ShowEraseResult(false, m_ResultDurationInSeconds);
                savedAnchorEntry.EnableEraseButton(true);
                savedAnchorEntry.EnableActionButton(!isAnchorInScene);
                return;
            }

            if (!m_SupportsGetSavedAnchorIds)
                await m_SaveAndLoadAnchorDataToFile.EraseAnchorIdAsync(savedAnchorEntry.savedAnchorGuid);

            savedAnchorEntry.StopEraseLoadingAnimation();
            await savedAnchorEntry.ShowEraseResult(true, m_ResultDurationInSeconds);

            if (isAnchorInScene)
            {
                var newAnchorEntry = await InstantiateAsync(m_NewAnchorEntryPrefab, m_ContentTransform);
                SetupNewAnchorEntry(
                    newAnchorEntry[0],
                    savedAnchorEntry.representedAnchor,
                    savedAnchorEntry.AnchorDisplayText);
            }

            RemoveSavedAnchorEntry(savedAnchorEntry);
        }
    }
}
