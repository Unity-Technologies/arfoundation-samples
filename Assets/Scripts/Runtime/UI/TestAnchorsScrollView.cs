using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class TestAnchorsScrollView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        BackButton m_BackButton;

        [SerializeField]
        ARAnchorManager m_AnchorManager;

        [SerializeField]
        Transform m_ContentTransform;

        [SerializeField]
        Transform m_PersistentAnchorsNotSupportedLabel;

        [Header("Prefabs")]
        [SerializeField]
        TestAnchorScrollViewEntry m_TestAnchorEntryPrefab;

        [Header("Settings")]
        [SerializeField]
        float m_ResultDurationInSeconds = 2;

        int m_EntryCount = 1;
        HashSet<TrackableId> m_LoadRequest = new();
        bool m_SupportsGetSavedAnchorIds;
        bool m_SupportsSaveAndLoadAnchors;
        bool m_SupportsEraseAnchors;

        SaveAndLoadAnchorDataToFile m_SaveAndLoadAnchorDataToFile;
        Dictionary<TrackableId, TestAnchorScrollViewEntry> m_ActiveTestAnchorEntriesByAnchorId = new();
        SerializableGuid m_InvalidAnchorGuid = default;

        List<ARSaveOrLoadAnchorResult> m_OutputSavedAnchorResults = new();

        void Awake()
        {
            m_SupportsGetSavedAnchorIds = m_AnchorManager.subsystem.subsystemDescriptor.supportsGetSavedAnchorIds;
            m_SupportsSaveAndLoadAnchors =
                m_AnchorManager.subsystem.subsystemDescriptor.supportsSaveAnchor &&
                m_AnchorManager.subsystem.subsystemDescriptor.supportsLoadAnchor;

            if (!m_SupportsSaveAndLoadAnchors)
            {
                m_PersistentAnchorsNotSupportedLabel.gameObject.SetActive(true);
            }

            m_SupportsEraseAnchors = m_AnchorManager.subsystem.subsystemDescriptor.supportsEraseAnchor;

            if (m_SupportsSaveAndLoadAnchors)
            {
                m_AnchorManager.trackablesChanged.AddListener(OnAnchorsChanged);
            }
        }

        async void Start()
        {
            IEnumerable<SerializableGuid> savedAnchorGuids = new List<SerializableGuid>();
            IEnumerable<DateTime> anchorSavedDateTimes = new List<DateTime>();

            if (m_SupportsGetSavedAnchorIds)
            {
                var result = await m_AnchorManager.TryGetSavedAnchorIdsAsync(Allocator.Temp);
                if (result.status.IsSuccess())
                {
                    savedAnchorGuids = result.value;
                    anchorSavedDateTimes = new List<DateTime>(savedAnchorGuids.Count());
                }
            }
            else
            {
                m_SaveAndLoadAnchorDataToFile ??= new SaveAndLoadAnchorDataToFile();
                var savedAnchorData = await m_SaveAndLoadAnchorDataToFile.GetSavedAnchorsDataAsync();
                savedAnchorGuids = savedAnchorData.Keys;
                anchorSavedDateTimes = savedAnchorData.Values;
            }

            using var savedDateTimesItr = anchorSavedDateTimes.GetEnumerator();
            foreach (var savedAnchorGuid in savedAnchorGuids)
            {
                savedDateTimesItr.MoveNext();
                var entry = AddAnchorEntry(null, savedAnchorGuid);
                if (savedDateTimesItr.Current != default)
                {
                    entry.SetAnchorSavedDateTime(savedDateTimesItr.Current);
                }
            }
        }

        void OnDestroy()
        {
            if (m_SupportsSaveAndLoadAnchors)
            {
                m_AnchorManager.trackablesChanged.RemoveListener(OnAnchorsChanged);
            }
        }

        public async void SaveInvalidAnchor()
        {
            if (!m_SupportsSaveAndLoadAnchors)
            {
                return;
            }

            var result = await m_AnchorManager.subsystem.TrySaveAnchorAsync(m_InvalidAnchorGuid);
            Debug.Log($"ARF_Anchors_TestAnchorsScrollView.SaveInvalidAnchor: {result.status.statusCode}");
        }

        public void SaveSingleConcurrentRequests()
        {
            if (!m_SupportsSaveAndLoadAnchors)
            {
                return;
            }

            if (m_ActiveTestAnchorEntriesByAnchorId.Count < 2)
            {
                Debug.Log("ARF_Anchors_TestAnchorsScrollView.SaveSingleCurrentRequests: Could not run test. " +
                    "Not enough active anchors.");
                return;
            }

            TestAnchorScrollViewEntry entry1 = null;
            TestAnchorScrollViewEntry entry2 = null;
            var count = 1;
            foreach (var (_, scrollViewEntry) in m_ActiveTestAnchorEntriesByAnchorId)
            {
                if (count == 1)
                {
                    entry1 = scrollViewEntry;
                }
                else if (count == 2)
                {
                    entry2 = scrollViewEntry;
                }

                if (count == 2)
                    break;

                count += 1;
            }

            RequestSaveAnchor(entry1);
            RequestSaveAnchor(entry2);
        }

        public void SaveBatchConcurrentRequests()
        {
            if (!m_SupportsSaveAndLoadAnchors)
            {
                return;
            }

            var anchors = new List<ARAnchor>();
            var anchorEntries = new List<TestAnchorScrollViewEntry>();
            foreach (var anchorEntry in m_ActiveTestAnchorEntriesByAnchorId.Values)
            {
                anchors.Add(anchorEntry.representedAnchor);
                anchorEntries.Add(anchorEntry);
                anchorEntry.StartSaveInProgressAnimation();
            }

            var splitIndex = anchors.Count / 2;
            var anchorsFirstHalf = anchors.Take(splitIndex).ToList();
            var anchorEntriesFirstHalf = anchorEntries.Take(splitIndex).ToList();

            var anchorsSecondHalf = anchors.Skip(splitIndex).ToList();
            var anchorEntriesSecondHalf = anchorEntries.Skip(splitIndex).ToList();

            SaveAnchorsAsync(anchorsFirstHalf, anchorEntriesFirstHalf);
            SaveAnchorsAsync(anchorsSecondHalf, anchorEntriesSecondHalf);
        }

        async void SaveAnchorsAsync(IList<ARAnchor> anchorsToSave, List<TestAnchorScrollViewEntry> anchorEntries)
        {
            if (!m_SupportsSaveAndLoadAnchors)
            {
                return;
            }

            foreach (var anchorEntry in anchorEntries)
            {
                anchorEntry.StartSaveInProgressAnimation();
            }

            await m_AnchorManager.TrySaveAnchorsAsync(anchorsToSave, m_OutputSavedAnchorResults);

            for (var i = 0; i < anchorsToSave.Count; i += 1)
            {
                var wasSaveSuccessful = m_OutputSavedAnchorResults[i].resultStatus.IsSuccess();
                Debug.Log("ARF_Anchors_TestAnchorsScrollView.SaveAnchorBatch: " +
                    $"{m_OutputSavedAnchorResults[i].resultStatus.statusCode}");

                if (!m_SupportsGetSavedAnchorIds && wasSaveSuccessful)
                {
                    m_SaveAndLoadAnchorDataToFile ??= new SaveAndLoadAnchorDataToFile();
                    await m_SaveAndLoadAnchorDataToFile.SaveAnchorIdAsync(
                        m_OutputSavedAnchorResults[i].savedAnchorGuid,
                        DateTime.Now);
                }

                if (wasSaveSuccessful)
                {
                    anchorEntries[i].savedAnchorGuid = m_OutputSavedAnchorResults[i].savedAnchorGuid;
                    anchorEntries[i].SetAnchorSavedDateTime(DateTime.Now);
                }

                anchorEntries[i].StopSaveInProgressAnimation();
#pragma warning disable CS4014
                anchorEntries[i].ShowSaveResult(wasSaveSuccessful, m_ResultDurationInSeconds);
#pragma warning restore CS4014
            }
        }

        public async void SaveAllAnchorsWithInvalidAnchor()
        {
            if (!m_SupportsSaveAndLoadAnchors)
            {
                return;
            }

            var anchorIdsToSave = new NativeArray<TrackableId>(
                m_ActiveTestAnchorEntriesByAnchorId.Count + 1,
                Allocator.Persistent);

            var index = 0;
            foreach (var anchorEntry in m_ActiveTestAnchorEntriesByAnchorId.Values)
            {
                anchorIdsToSave[index] = anchorEntry.representedAnchor.trackableId;
                index += 1;
            }

            anchorIdsToSave[^1] = m_InvalidAnchorGuid;
            var saveAnchorResults = await m_AnchorManager.subsystem.TrySaveAnchorsAsync(
                anchorIdsToSave,
                Allocator.Temp);

            foreach (var saveAnchorResult in saveAnchorResults)
            {
                Debug.Log("ARF_Anchors_TestAnchorsScrollView.SaveAllAnchorsWithInvalidAnchor: " +
                    $"anchor: {saveAnchorResult.trackableId} " +
                    $"result status: {saveAnchorResult.resultStatus.statusCode}");
            }
        }

        public void SaveSameAnchorTwiceConcurrently()
        {
            if (!m_SupportsSaveAndLoadAnchors)
            {
                return;
            }

            if (m_ActiveTestAnchorEntriesByAnchorId.Count == 0)
            {
                Debug.Log("ARF_Anchors_TestAnchorsScrollView.SaveSameAnchorTwiceConcurrently: Could not run test. " +
                    "There are no active anchors.");
                return;
            }

            var entry = m_ActiveTestAnchorEntriesByAnchorId.First().Value;
            RequestSaveAnchor(entry);
            RequestSaveAnchor(entry);
        }

        public void SaveBatchConcurrentWithSameAnchor()
        {
            if (!m_SupportsSaveAndLoadAnchors)
            {
                return;
            }

            var anchors = new List<ARAnchor>();
            var anchorEntries = new List<TestAnchorScrollViewEntry>();
            foreach (var anchorEntry in m_ActiveTestAnchorEntriesByAnchorId.Values)
            {
                anchors.Add(anchorEntry.representedAnchor);
                anchorEntries.Add(anchorEntry);
                anchorEntry.StartSaveInProgressAnimation();
            }

            var splitIndex = anchors.Count / 2;
            var anchorsFirstHalf = anchors.Take(splitIndex).ToList();
            var anchorEntriesFirstHalf = anchorEntries.Take(splitIndex).ToList();

            var anchorsSecondHalf = anchors.Skip(splitIndex).ToList();
            anchorsSecondHalf.Add(anchorsFirstHalf[0]);
            var anchorEntriesSecondHalf = anchorEntries.Skip(splitIndex).ToList();
            anchorEntriesSecondHalf.Add(anchorEntriesFirstHalf[0]);

            SaveAnchorsAsync(anchorsFirstHalf, anchorEntriesFirstHalf);
            SaveAnchorsAsync(anchorsSecondHalf, anchorEntriesSecondHalf);
        }

        public async void LoadInvalidAnchor()
        {
            if (!m_SupportsSaveAndLoadAnchors)
            {
                return;
            }

            var result = await m_AnchorManager.TryLoadAnchorAsync(m_InvalidAnchorGuid);
            Debug.Log($"ARF_Anchors_TestAnchorsScrollView.LoadInvalidAnchor: {result.status.statusCode}");
        }

        public async void LoadAllAnchorsWithInvalidAnchor()
        {
            if (!m_SupportsSaveAndLoadAnchors)
            {
                return;
            }

            var savedAnchorGuids = new List<SerializableGuid>();
            var savedAnchorData = await m_SaveAndLoadAnchorDataToFile.GetSavedAnchorsDataAsync();

            foreach (var savedAnchorGuid in savedAnchorData.Keys)
            {
                savedAnchorGuids.Add(savedAnchorGuid);
            }

            savedAnchorGuids.Add(m_InvalidAnchorGuid);
            LoadAnchorsAsync(savedAnchorGuids);
        }

        public async void LoadSingleConcurrentRequests()
        {
            if (!m_SupportsSaveAndLoadAnchors)
            {
                return;
            }

            var savedAnchorData = await m_SaveAndLoadAnchorDataToFile.GetSavedAnchorsDataAsync();
            if (savedAnchorData.Count < 2)
            {
                Debug.Log("ARF_Anchors_TestAnchorsScrollView.LoadSingleConcurrentRequests: Could not run test. " +
                    "Not enough saved anchors.");
                return;
            }

            SerializableGuid savedAnchorGuid1 = default;
            SerializableGuid savedAnchorGuid2 = default;
            var count = 1;
            foreach (var savedAnchorGuid in savedAnchorData.Keys)
            {
                if (count == 1)
                {
                    savedAnchorGuid1 = savedAnchorGuid;
                }
                else if (count == 2)
                {
                    savedAnchorGuid2 = savedAnchorGuid;
                }

                if (count == 2)
                    break;

                count += 1;
            }

            LoadAnchorAsync(savedAnchorGuid1);
            LoadAnchorAsync(savedAnchorGuid2);
        }

        public async void LoadBatchConcurrentRequests()
        {
            if (!m_SupportsSaveAndLoadAnchors)
            {
                return;
            }

            var savedAnchorData = await m_SaveAndLoadAnchorDataToFile.GetSavedAnchorsDataAsync();
            var savedAnchorGuids = new List<SerializableGuid>();

            foreach (var savedAnchorGuid in savedAnchorData.Keys)
            {
                savedAnchorGuids.Add(savedAnchorGuid);
            }

            var splitIndex = savedAnchorGuids.Count / 2;
            var anchorsFirstHalf = savedAnchorGuids.Take(splitIndex).ToList();
            var anchorsSecondHalf = savedAnchorGuids.Skip(splitIndex).ToList();

            LoadAnchorsAsync(anchorsFirstHalf);
            LoadAnchorsAsync(anchorsSecondHalf);
        }

        public async void LoadSameAnchorTwiceConcurrently()
        {
            if (!m_SupportsSaveAndLoadAnchors)
            {
                return;
            }

            var savedAnchorData = await m_SaveAndLoadAnchorDataToFile.GetSavedAnchorsDataAsync();
            if (savedAnchorData.Count == 0)
            {
                Debug.Log("ARF_Anchors_TestAnchorsScrollView.LoadSameAnchorTwiceConcurrently: Could not run test. " +
                    "There are no saved anchors.");
                return;
            }

            var savedAnchorGuid = savedAnchorData.First().Key;

            LoadAnchorAsync(savedAnchorGuid);
            LoadAnchorAsync(savedAnchorGuid);
        }

        public async void LoadBatchConcurrentWithSameAnchor()
        {
            if (!m_SupportsSaveAndLoadAnchors)
            {
                return;
            }

            var savedAnchorData = await m_SaveAndLoadAnchorDataToFile.GetSavedAnchorsDataAsync();
            var savedAnchorGuids = new List<SerializableGuid>();

            foreach (var savedAnchorGuid in savedAnchorData.Keys)
            {
                savedAnchorGuids.Add(savedAnchorGuid);
            }

            var splitIndex = savedAnchorGuids.Count / 2;
            var anchorsFirstHalf = savedAnchorGuids.Take(splitIndex).ToList();
            var anchorsSecondHalf = savedAnchorGuids.Skip(splitIndex).ToList();
            anchorsSecondHalf.Add(anchorsFirstHalf[0]);

            LoadAnchorsAsync(anchorsFirstHalf);
            LoadAnchorsAsync(anchorsSecondHalf);
        }

        async void LoadAnchorAsync(SerializableGuid savedAnchorGuid)
        {
            if (!m_SupportsSaveAndLoadAnchors)
            {
                return;
            }

            var result = await m_AnchorManager.TryLoadAnchorAsync(savedAnchorGuid);
            if (result.status.IsSuccess())
            {
                m_LoadRequest.Add(result.value.trackableId);
            }

            Debug.Log($"ARF_Anchors_TestAnchorsScrollView.LoadAnchorAsync: {result.status.statusCode}");
        }

        async void LoadAnchorsAsync(List<SerializableGuid> savedAnchorGuids)
        {
            var loadAnchorResults = new List<ARSaveOrLoadAnchorResult>();
            await m_AnchorManager.TryLoadAnchorsAsync(
                savedAnchorGuids,
                loadAnchorResults,
                incrementalResults =>
                {
                    foreach (var loadAnchorResult in incrementalResults)
                    {
                        m_LoadRequest.Add(loadAnchorResult.anchor.trackableId);
                        Debug.Log(
                            $"ARF_Anchors_TestAnchorsScrollView.LoadAnchorsAsync:  " +
                            $"serializableGuid: {loadAnchorResult.savedAnchorGuid} " +
                            $"result status: {loadAnchorResult.resultStatus.statusCode}");
                    }
                });

            foreach (var loadAnchorResult in loadAnchorResults)
            {
                if (loadAnchorResult.resultStatus.IsError())
                {
                    Debug.Log(
                        $"ARF_Anchors_TestAnchorsScrollView.LoadAnchorsAsync:  " +
                        $"serializableGuid: {loadAnchorResult.savedAnchorGuid} " +
                        $"result status: {loadAnchorResult.resultStatus.statusCode}");
                }
            }
        }

        public async void EraseInvalidAnchor()
        {
            if (!m_SupportsEraseAnchors)
            {
                return;
            }

            var resultStatus = await m_AnchorManager.TryEraseAnchorAsync(m_InvalidAnchorGuid);
            Debug.Log($"ARF_Anchors_TestAnchorsScrollView.EraseInvalidAnchor: {resultStatus.statusCode}");
        }

        public async void EraseAllAnchorsWithInvalidAnchor()
        {
            if (!m_SupportsEraseAnchors)
            {
                return;
            }

            var savedAnchorData = await m_SaveAndLoadAnchorDataToFile.GetSavedAnchorsDataAsync();

            if (savedAnchorData.Count == 0)
            {
                Debug.Log("ARF_Anchors_TestAnchorsScrollView.EraseAllAnchorsWithInvalidAnchor: Could not run test. " +
                    "There are no saved anchors.");
                return;
            }

            var savedAnchorGuids = new List<SerializableGuid>();

            foreach (var savedAnchorGuid in savedAnchorData.Keys)
            {
                savedAnchorGuids.Add(savedAnchorGuid);
            }

            savedAnchorGuids.Add(m_InvalidAnchorGuid);
            EraseAnchorsAsync(savedAnchorGuids);
        }

        public async void EraseSingleConcurrentRequests()
        {
            if (!m_SupportsEraseAnchors)
            {
                return;
            }

            var savedAnchorData = await m_SaveAndLoadAnchorDataToFile.GetSavedAnchorsDataAsync();
            if (savedAnchorData.Count < 2)
            {
                Debug.Log("ARF_Anchors_TestAnchorsScrollView.EraseSingleConcurrentRequests: Could not run test. " +
                    "Not enough saved anchors.");
                return;
            }

            SerializableGuid savedAnchorGuid1 = default;
            SerializableGuid savedAnchorGuid2 = default;
            var count = 1;
            foreach (var savedAnchorGuid in savedAnchorData.Keys)
            {
                if (count == 1)
                {
                    savedAnchorGuid1 = savedAnchorGuid;
                }
                else if (count == 2)
                {
                    savedAnchorGuid2 = savedAnchorGuid;
                }

                if (count == 2)
                    break;

                count += 1;
            }

            EraseAnchorAsync(savedAnchorGuid1);
            EraseAnchorAsync(savedAnchorGuid2);
        }

        public async void EraseBatchConcurrentRequests()
        {
            if (!m_SupportsEraseAnchors)
            {
                return;
            }

            var savedAnchorData = await m_SaveAndLoadAnchorDataToFile.GetSavedAnchorsDataAsync();

            if (savedAnchorData.Count == 0)
            {
                Debug.Log("ARF_Anchors_TestAnchorsScrollView.EraseBatchConcurrentRequests: Could not run test. " +
                    "There are no saved anchors.");
                return;
            }

            var savedAnchorGuids = new List<SerializableGuid>();
            foreach (var savedAnchorGuid in savedAnchorData.Keys)
            {
                savedAnchorGuids.Add(savedAnchorGuid);
            }

            var splitIndex = savedAnchorGuids.Count / 2;
            var anchorsFirstHalf = savedAnchorGuids.Take(splitIndex).ToList();
            var anchorsSecondHalf = savedAnchorGuids.Skip(splitIndex).ToList();

            EraseAnchorsAsync(anchorsFirstHalf);
            EraseAnchorsAsync(anchorsSecondHalf);
        }

        public async void EraseSameAnchorTwiceConcurrently()
        {
            if (!m_SupportsEraseAnchors)
            {
                return;
            }

            var savedAnchorData = await m_SaveAndLoadAnchorDataToFile.GetSavedAnchorsDataAsync();
            if (savedAnchorData.Count == 0)
            {
                Debug.Log("ARF_Anchors_TestAnchorsScrollView.EraseSameAnchorTwiceConcurrently: " +
                    "Could not run test. There are no saved anchors.");
                return;
            }

            var savedAnchorGuid = savedAnchorData.First().Key;

            EraseAnchorAsync(savedAnchorGuid);
            EraseAnchorAsync(savedAnchorGuid);
        }

        public async void EraseBatchConcurrentWithSameAnchor()
        {
            if (!m_SupportsEraseAnchors)
            {
                return;
            }

            var savedAnchorData = await m_SaveAndLoadAnchorDataToFile.GetSavedAnchorsDataAsync();

            if (savedAnchorData.Count == 0)
            {
                Debug.Log("ARF_Anchors_TestAnchorsScrollView.EraseBatchConcurrentWithSameAnchor: " +
                    "Could not run test. There are no saved anchors.");
                return;
            }

            var savedAnchorGuids = new List<SerializableGuid>();

            foreach (var savedAnchorGuid in savedAnchorData.Keys)
            {
                savedAnchorGuids.Add(savedAnchorGuid);
            }

            var splitIndex = savedAnchorGuids.Count / 2;
            var anchorsFirstHalf = savedAnchorGuids.Take(splitIndex).ToList();
            var anchorsSecondHalf = savedAnchorGuids.Skip(splitIndex).ToList();
            anchorsSecondHalf.Add(anchorsFirstHalf[0]);

            EraseAnchorsAsync(anchorsFirstHalf);
            EraseAnchorsAsync(anchorsSecondHalf);
        }

        async void EraseAnchorAsync(SerializableGuid savedAnchorGuid)
        {
            var resultStats = await m_AnchorManager.TryEraseAnchorAsync(savedAnchorGuid);
            Debug.Log($"ARF_Anchors_TestAnchorsScrollView.EraseAnchorAsync: {resultStats.statusCode}");

            if (resultStats.IsSuccess())
            {
                await m_SaveAndLoadAnchorDataToFile.EraseAnchorIdAsync(savedAnchorGuid);
            }
        }

        async void EraseAnchorsAsync(List<SerializableGuid> savedAnchorGuids)
        {
            var eraseAnchorResults = new List<XREraseAnchorResult>();
            await m_AnchorManager.TryEraseAnchorsAsync(
                savedAnchorGuids,
                eraseAnchorResults);

            foreach (var eraseAnchorResult in eraseAnchorResults)
            {
                Debug.Log(
                    $"ARF_Anchors_TestAnchorsScrollView.EraseAnchorsAsync:  " +
                    $"serializableGuid: {eraseAnchorResult.savedAnchorGuid} " +
                    $"result status: {eraseAnchorResult.resultStatus.statusCode}, " +
                    $"native code: {eraseAnchorResult.resultStatus.nativeStatusCode}");

                if (eraseAnchorResult.resultStatus.IsSuccess())
                {
                    await m_SaveAndLoadAnchorDataToFile.EraseAnchorIdAsync(eraseAnchorResult.savedAnchorGuid);
                }
            }
        }

        void OnAnchorsChanged(ARTrackablesChangedEventArgs<ARAnchor> changes)
        {
            foreach (var anchor in changes.added)
            {
                // check if the added anchor was a result of a load request
                if (m_LoadRequest.Contains(anchor.trackableId))
                {
                    m_LoadRequest.Remove(anchor.trackableId);
                }
                else
                {
                    var entry = AddAnchorEntry(anchor);
                    m_ActiveTestAnchorEntriesByAnchorId.Add(anchor.trackableId, entry);
                }
            }

            foreach (var (trackableId, _) in changes.removed)
            {
                m_ActiveTestAnchorEntriesByAnchorId.Remove(trackableId);
            }
        }

        TestAnchorScrollViewEntry AddAnchorEntry(ARAnchor anchor = null, SerializableGuid savedAnchorGuid = default)
        {
            var anchorEntry = Instantiate(m_TestAnchorEntryPrefab, m_ContentTransform);

            anchorEntry.transform.SetAsLastSibling();
            anchorEntry.SetDisplayedAnchorLabel($"Anchor {m_EntryCount}");
            anchorEntry.representedAnchor = anchor;
            anchorEntry.savedAnchorGuid = savedAnchorGuid;
            anchorEntry.requestSave.AddListener(RequestSaveAnchor);
            anchorEntry.requestLoad.AddListener(RequestLoadAnchor);

            anchorEntry.requestSaveAndLeave.AddListener(RequestSaveAnchorAndLeave);
            anchorEntry.requestLoadAndLeave.AddListener(RequestLoadAnchorAndLeave);
            anchorEntry.requestSaveAndCancel.AddListener(RequestSaveAndCancel);
            anchorEntry.requestLoadAndCancel.AddListener(RequestLoadAndCancel);

            if (m_SupportsEraseAnchors)
                anchorEntry.requestEraseAnchor.AddListener(RequestEraseAnchor);

            m_EntryCount += 1;
            return anchorEntry;
        }

        void RequestSaveAnchor(TestAnchorScrollViewEntry entry)
        {
            RequestSaveAnchor(entry, CancellationToken.None);
        }

        async void RequestSaveAnchor(TestAnchorScrollViewEntry entry, CancellationToken cancellationToken)
        {
            if (entry.representedAnchor == null)
            {
                await entry.ShowSaveResult(false, m_ResultDurationInSeconds);
                return;
            }

            entry.StartSaveInProgressAnimation();

            var result = await m_AnchorManager.TrySaveAnchorAsync(entry.representedAnchor, cancellationToken);
            Debug.Log($"ARF_Anchors_TestAnchorsScrollView.RequestSaveAnchor: {result.status.statusCode}");

            var wasSaveSuccessful = result.status.IsSuccess();
            if (!m_SupportsGetSavedAnchorIds && wasSaveSuccessful)
            {
                m_SaveAndLoadAnchorDataToFile ??= new SaveAndLoadAnchorDataToFile();
                await m_SaveAndLoadAnchorDataToFile.SaveAnchorIdAsync(result.value, DateTime.Now);
            }

            if (wasSaveSuccessful)
            {
                entry.savedAnchorGuid = result.value;
                entry.SetAnchorSavedDateTime(DateTime.Now);
            }

            entry.StopSaveInProgressAnimation();
            await entry.ShowSaveResult(wasSaveSuccessful, m_ResultDurationInSeconds);
        }

        void RequestSaveAnchorAndLeave(TestAnchorScrollViewEntry entry)
        {
            var result = m_AnchorManager.TrySaveAnchorAsync(entry.representedAnchor);
            m_BackButton.BackButtonPressed();
        }

        void RequestLoadAnchorAndLeave(TestAnchorScrollViewEntry entry)
        {
            var result = m_AnchorManager.TryLoadAnchorAsync(entry.savedAnchorGuid);
            m_BackButton.BackButtonPressed();
        }

        void RequestSaveAndCancel(TestAnchorScrollViewEntry entry)
        {
            if (!m_SupportsSaveAndLoadAnchors)
            {
                return;
            }

            var cts = new CancellationTokenSource();
            RequestSaveAnchor(entry, cts.Token);
            cts.Cancel();
        }

        void RequestLoadAndCancel(TestAnchorScrollViewEntry entry)
        {
            if (!m_SupportsSaveAndLoadAnchors)
            {
                return;
            }

            var cts = new CancellationTokenSource();
            RequestLoadAnchor(entry, cts.Token);
            cts.Cancel();
        }

        void RequestLoadAnchor(TestAnchorScrollViewEntry entry)
        {
            RequestLoadAnchor(entry, CancellationToken.None);
        }

        async void RequestLoadAnchor(TestAnchorScrollViewEntry entry, CancellationToken cancellationToken)
        {
            entry.StartLoadInProgressAnimation();

            var result = await m_AnchorManager.TryLoadAnchorAsync(entry.savedAnchorGuid, cancellationToken);
            var wasLoadSuccessful = result.status.IsSuccess();
            if (wasLoadSuccessful)
            {
                entry.representedAnchor = result.value;
                // add anchor id to load request list so when the added anchor change event
                // is raised, we can know an entry for this anchor already exists
                m_LoadRequest.Add(entry.representedAnchor.trackableId);
                m_ActiveTestAnchorEntriesByAnchorId.TryAdd(entry.representedAnchor.trackableId, entry);
            }

            entry.StopLoadInProgressAnimation();
            await entry.ShowLoadResult(wasLoadSuccessful, m_ResultDurationInSeconds);
        }

        async void RequestEraseAnchor(TestAnchorScrollViewEntry entry)
        {
            entry.StartEraseInProgressAnimation();

            var result = await m_AnchorManager.TryEraseAnchorAsync(entry.savedAnchorGuid);

            if (!result.IsSuccess())
            {
                entry.StopEraseInProgressAnimation();
                await entry.ShowEraseResult(false, m_ResultDurationInSeconds);
                return;
            }

            var eraseFromFileAwaitable = m_SaveAndLoadAnchorDataToFile.EraseAnchorIdAsync(entry.savedAnchorGuid);

            entry.StopEraseInProgressAnimation();
            var showResultAwaitable = entry.ShowEraseResult(true, m_ResultDurationInSeconds);

            await eraseFromFileAwaitable;
            await showResultAwaitable;
        }
    }
}
