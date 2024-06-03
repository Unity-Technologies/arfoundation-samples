using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARSubsystems;
using SerializableGuid = UnityEngine.XR.ARSubsystems.SerializableGuid;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class AnchorsScrollView : MonoBehaviour
    {
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
        Transform m_PersistentAnchorsNotSupportedLabel;

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
        Dictionary<TrackableId, AnchorScrollViewEntry> m_SavedAnchorEntriesByAnchorId = new();

        bool m_SupportsGetSavedAnchorIds;
        bool m_SupportsSaveAndLoadAnchors;
        bool m_SupportsEraseAnchors;

        SaveAndLoadAnchorIdsToFile m_SaveAndLoadAnchorIdsToFile;

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

                Debug.LogWarning($"Null serialized field. Set the {nameof(ARAnchorManager)} reference on the {nameof(AnchorsScrollView)} component for better performance.", this);
            }

            var descriptor = m_AnchorManager.descriptor;
            m_SupportsGetSavedAnchorIds = descriptor.supportsGetSavedAnchorIds;
            m_SupportsSaveAndLoadAnchors = descriptor.supportsSaveAnchor && descriptor.supportsLoadAnchor;

            if (!m_SupportsSaveAndLoadAnchors)
            {
                m_PersistentAnchorsNotSupportedLabel.gameObject.SetActive(true);
                m_NewAnchorsLabel.gameObject.SetActive(false);
                m_SavedAnchorsLabel.gameObject.SetActive(false);
            }

            m_SupportsEraseAnchors = descriptor.supportsEraseAnchor;

            InitializeUI();
        }

        void OnDestroy()
        {
            if (m_SupportsSaveAndLoadAnchors)
            {
                m_AnchorManager.trackablesChanged.RemoveListener(OnAnchorsChanged);
            }
        }

        async void InitializeUI()
        {
            if (m_SupportsGetSavedAnchorIds)
            {
                var result = await m_AnchorManager.TryGetSavedAnchorIdsAsync(Allocator.Temp);
                if (result.status.IsSuccess())
                {
                    foreach (var persistentAnchorGuid in result.value)
                    {
                        AddSavedAnchorEntry(persistentAnchorGuid, false);
                    }
                }
            }
            else
            {
                m_SaveAndLoadAnchorIdsToFile ??= new SaveAndLoadAnchorIdsToFile();
                var persistentAnchorGuids = await m_SaveAndLoadAnchorIdsToFile.LoadSavedAnchorIdsAsync();
                foreach (var persistentAnchorGuid in persistentAnchorGuids)
                {
                    AddSavedAnchorEntry(persistentAnchorGuid, false);
                }
            }

            foreach (var anchor in m_AnchorManager.trackables)
            {
                if (!m_SavedAnchorEntriesByAnchorId.ContainsKey(anchor.trackableId))
                {
                    AddNewAnchorEntry(anchor);
                }
            }

            if (m_SupportsSaveAndLoadAnchors)
            {
                m_AnchorManager.trackablesChanged.AddListener(OnAnchorsChanged);
            }
        }

        void OnAnchorsChanged(ARTrackablesChangedEventArgs<ARAnchor> changes)
        {
            foreach (var anchor in changes.added)
            {
                if (m_LoadRequests.Contains(anchor.trackableId))
                {
                    m_LoadRequests.Remove(anchor.trackableId);
                }
                else
                {
                    AddNewAnchorEntry(anchor);
                }
            }

            foreach ((TrackableId anchorId, _) in changes.removed)
            {
                if (m_SavedAnchorEntriesByAnchorId.TryGetValue(anchorId, out var savedAnchorEntry))
                {
                    savedAnchorEntry.EnableActionButton(true);
                }
                else if (m_NewAnchorEntriesByAnchorId.TryGetValue(anchorId, out var newAnchorEntry))
                {
                    RemoveNewAnchorEntry(newAnchorEntry);
                }
            }
        }

        AnchorScrollViewEntry AddNewAnchorEntry(ARAnchor anchor, string displayLabelText = null)
        {
            var newAnchorEntry = Instantiate(m_NewAnchorEntryPrefab, m_ContentTransform);
            var lastNewAnchorEntryIndex = m_SavedAnchorsLabel.GetSiblingIndex();

            newAnchorEntry.transform.SetSiblingIndex(lastNewAnchorEntryIndex);
            newAnchorEntry.SetDisplayedAnchorLabel(displayLabelText ?? $"Anchor {m_EntryCount}");
            newAnchorEntry.representedAnchor = anchor;
            newAnchorEntry.requestAction.AddListener(RequestSaveAnchor);

            m_NewAnchorEntriesByAnchorId.Add(anchor.trackableId, newAnchorEntry);
            if (displayLabelText == null)
                m_EntryCount += 1;

            return newAnchorEntry;
        }

        void RemoveNewAnchorEntry(AnchorScrollViewEntry entry)
        {
            entry.requestAction.RemoveListener(RequestSaveAnchor);
            UnityObjectUtils.Destroy(entry.gameObject);
            m_NewAnchorEntriesByAnchorId.Remove(entry.representedAnchor.trackableId);
        }

        AnchorScrollViewEntry AddSavedAnchorEntry(SerializableGuid persistentAnchorGuid, bool isAnchorActive, string displayLabelText = null)
        {
            var savedAnchorEntry = Instantiate(m_SavedAnchorEntryPrefab, m_ContentTransform);

            savedAnchorEntry.transform.SetAsLastSibling();
            savedAnchorEntry.SetDisplayedAnchorLabel(displayLabelText ?? $"Anchor {m_EntryCount}");
            savedAnchorEntry.persistentAnchorGuid = persistentAnchorGuid;
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

            return savedAnchorEntry;
        }

        void RemoveSavedAnchorEntry(AnchorScrollViewEntry entry)
        {
            entry.requestAction.RemoveListener(RequestLoadAnchor);

            if (m_SupportsEraseAnchors)
                entry.requestEraseAnchor.RemoveListener(RequestEraseAnchor);

            Destroy(entry.gameObject);
            m_SavedAnchorEntriesByAnchorId.Remove(entry.representedAnchor.trackableId);
        }

        async void RequestSaveAnchor(AnchorScrollViewEntry entry)
        {
            entry.EnableActionButton(false);
            entry.StartActionLoadingAnimation();

            // collect data from the entry in case it gets destroyed
            // during the save process from removing or destroying the anchor
            var representedAnchor = entry.representedAnchor;
            var representedAnchorTrackableId = entry.representedAnchor.trackableId;
            var displayLabelText = entry.AnchorDisplayText;
            var result = await m_AnchorManager.TrySaveAnchorAsync(representedAnchor);
            if (!m_SupportsGetSavedAnchorIds)
            {
                m_SaveAndLoadAnchorIdsToFile ??= new SaveAndLoadAnchorIdsToFile();
                await m_SaveAndLoadAnchorIdsToFile.SaveAnchorIdAsync(representedAnchorTrackableId);
            }

            var wasSaveSuccessful = result.status.IsSuccess();
            AnchorScrollViewEntry savedAnchorEntry = null;

            if (wasSaveSuccessful)
            {
                var savedAnchorId = result.value;
                var isAnchorActive = representedAnchor != null;
                savedAnchorEntry = AddSavedAnchorEntry(savedAnchorId, isAnchorActive, displayLabelText);
                // disabling to allow to show result before revealing the new entry
                // in the saved anchor section in the scroll view
                savedAnchorEntry.gameObject.SetActive(false);
                savedAnchorEntry.representedAnchor = representedAnchor;
                m_SavedAnchorEntriesByAnchorId.TryAdd(representedAnchorTrackableId, savedAnchorEntry);
            }

            entry.StopActionLoadingAnimation();
            await entry.ShowActionResult(wasSaveSuccessful, m_ResultDurationInSeconds);
            entry.EnableActionButton(true);

            if (wasSaveSuccessful)
            {
                // now we can remove the entry in the new entry section of the scroll view
                // and enable the entry in the saved anchor section of the scroll view
                RemoveNewAnchorEntry(entry);
                if (savedAnchorEntry != null)
                {
                    savedAnchorEntry.gameObject.SetActive(true);
                }
            }
        }

        async void RequestLoadAnchor(AnchorScrollViewEntry entry)
        {
            entry.EnableActionButton(false);
            entry.StartActionLoadingAnimation();

            var result = await m_AnchorManager.TryLoadAnchorAsync(entry.persistentAnchorGuid);
            var wasLoadSuccessful = result.status.IsSuccess();
            if (wasLoadSuccessful)
            {
                entry.representedAnchor = result.value;
                entry.EnableActionButton(false);
                // add anchor id to load request list so when the added anchor change event
                // is raised, we can know an entry for this anchor already exists
                m_LoadRequests.Add(entry.representedAnchor.trackableId);
                m_SavedAnchorEntriesByAnchorId.TryAdd(entry.representedAnchor.trackableId, entry);
            }

            entry.StopActionLoadingAnimation();
            await entry.ShowActionResult(wasLoadSuccessful, m_ResultDurationInSeconds);
            entry.EnableActionButton(!wasLoadSuccessful);
        }

        async void RequestEraseAnchor(AnchorScrollViewEntry entry)
        {
            entry.EnableActionButton(false);
            entry.EnableEraseButton(false);
            entry.StartEraseLoadingAnimation();

            var result = await m_AnchorManager.TryEraseAnchorAsync(entry.persistentAnchorGuid);
            var isAnchorInScene = entry.representedAnchor != null;

            if (!result.IsSuccess())
            {
                entry.StopEraseLoadingAnimation();
                await entry.ShowEraseResult(false, m_ResultDurationInSeconds);
                entry.EnableEraseButton(true);
                entry.EnableActionButton(!isAnchorInScene);
                return;
            }

            if (!m_SupportsGetSavedAnchorIds)
            {
                await m_SaveAndLoadAnchorIdsToFile.EraseAnchorIdAsync(entry.persistentAnchorGuid);
            }

            entry.StopEraseLoadingAnimation();
            await entry.ShowEraseResult(true, m_ResultDurationInSeconds);

            if (isAnchorInScene)
            {
                AddNewAnchorEntry(entry.representedAnchor, entry.AnchorDisplayText);
            }

            RemoveSavedAnchorEntry(entry);
        }
    }
}
