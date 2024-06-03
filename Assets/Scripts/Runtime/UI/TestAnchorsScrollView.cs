using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class TestAnchorsScrollView : MonoBehaviour
    {
        [Header("References")]
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

        SaveAndLoadAnchorIdsToFile m_SaveAndLoadAnchorIdsToFile;

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
        }

        async void Start()
        {
            IEnumerable<SerializableGuid> persistentAnchorGuids = new List<SerializableGuid>();
            if (m_SupportsGetSavedAnchorIds)
            {
                var result = await m_AnchorManager.TryGetSavedAnchorIdsAsync(Allocator.Temp);
                if (result.status.IsSuccess())
                {
                    persistentAnchorGuids = result.value;
                }
            }
            else
            {
                m_SaveAndLoadAnchorIdsToFile ??= new SaveAndLoadAnchorIdsToFile();
                persistentAnchorGuids = await m_SaveAndLoadAnchorIdsToFile.LoadSavedAnchorIdsAsync();
            }

            foreach (var persistentAnchorGuid in persistentAnchorGuids)
            {
                AddAnchorEntry(null, persistentAnchorGuid);
            }
        }

        void OnEnable()
        {
            if (m_SupportsSaveAndLoadAnchors)
            {
                m_AnchorManager.trackablesChanged.AddListener(OnAnchorsChanged);
            }
        }

        void OnDisable()
        {
            if (m_SupportsSaveAndLoadAnchors)
            {
                m_AnchorManager.trackablesChanged.RemoveListener(OnAnchorsChanged);
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
                    AddAnchorEntry(anchor);
                }
            }
        }

        void AddAnchorEntry(ARAnchor anchor = null, SerializableGuid persistentAnchorGuid = default)
        {
            var anchorEntry = Instantiate(m_TestAnchorEntryPrefab, m_ContentTransform);
            
            anchorEntry.transform.SetAsLastSibling();
            anchorEntry.SetDisplayedAnchorLabel($"Anchor {m_EntryCount}");
            anchorEntry.representedAnchor = anchor;
            anchorEntry.persistentAnchorGuid = persistentAnchorGuid;
            anchorEntry.requestSave.AddListener(RequestSaveAnchor);
            anchorEntry.requestLoad.AddListener(RequestLoadAnchor);

            if (m_SupportsEraseAnchors)
                anchorEntry.requestEraseAnchor.AddListener(RequestEraseAnchor);

            m_EntryCount += 1;
        }

        async void RequestSaveAnchor(TestAnchorScrollViewEntry entry)
        {
            if (entry.representedAnchor == null)
            {
                await entry.ShowSaveResult(false, m_ResultDurationInSeconds);
                return;
            }

            entry.StartSaveLoadingAnimation();

            var result = await m_AnchorManager.TrySaveAnchorAsync(entry.representedAnchor);
            if (!m_SupportsGetSavedAnchorIds)
            {
                m_SaveAndLoadAnchorIdsToFile ??= new SaveAndLoadAnchorIdsToFile();
                await m_SaveAndLoadAnchorIdsToFile.SaveAnchorIdAsync(entry.representedAnchor.trackableId);
            }

            var wasSaveSuccessful = result.status.IsSuccess();
            if (wasSaveSuccessful)
            {
                entry.persistentAnchorGuid = result.value;
            }

            entry.StopSaveLoadingAnimation();
            await entry.ShowSaveResult(wasSaveSuccessful, m_ResultDurationInSeconds);
        }

        async void RequestLoadAnchor(TestAnchorScrollViewEntry entry)
        {
            entry.StartLoadLoadingAnimation();

            var result = await m_AnchorManager.TryLoadAnchorAsync(entry.persistentAnchorGuid);
            var wasLoadSuccessful = result.status.IsSuccess();
            if (wasLoadSuccessful)
            {
                entry.representedAnchor = result.value;
                // add anchor id to load request list so when the added anchor change event
                // is raised, we can know an entry for this anchor already exists
                m_LoadRequest.Add(entry.representedAnchor.trackableId);
            }

            entry.StopLoadLoadingAnimation();
            await entry.ShowLoadResult(wasLoadSuccessful, m_ResultDurationInSeconds);
        }

        async void RequestEraseAnchor(TestAnchorScrollViewEntry entry)
        {
            entry.StartEraseLoadingAnimation();

            var result = await m_AnchorManager.TryEraseAnchorAsync(entry.persistentAnchorGuid);

            if (!result.IsSuccess())
            {
                entry.StopEraseLoadingAnimation();
                await entry.ShowEraseResult(false, m_ResultDurationInSeconds);
                return;
            }

            entry.StopEraseLoadingAnimation();
            await entry.ShowEraseResult(true, m_ResultDurationInSeconds);
        }
    }
}
