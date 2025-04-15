using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MetaBatchSharedAnchors : MonoBehaviour
    {
        [SerializeField, ReadOnlyField]
        UIBatchToggle m_UIBatchToggle;

        [SerializeField]
        UnityEvent<IReadOnlyCollection<MetaSharedAnchorEntry>> m_SelectedEntriesChanged = new();
        public UnityEvent<IReadOnlyCollection<MetaSharedAnchorEntry>> selectedEntriesChanged => m_SelectedEntriesChanged;

        HashSet<MetaSharedAnchorEntry> m_SelectedEntries = new();
        public IReadOnlyCollection<MetaSharedAnchorEntry> selectedEntries => m_SelectedEntries;

        HashSet<MetaSharedAnchorEntry> m_ChangedSelectedEntries = new();
        Dictionary<int, MetaSharedAnchorEntry> m_AnchorEntryById = new();
        Dictionary<Toggle, int> m_AnchorEntryIdByToggle = new();

        int m_SelectedAnchorsSavedCount;
        int m_SelectedAnchorsInSceneCount;

        public void EntryAdded(MetaSharedAnchorEntry entry)
        {
            m_AnchorEntryById.Add(entry.entryId, entry);
            m_AnchorEntryIdByToggle.Add(entry.toggle, entry.entryId);
            m_UIBatchToggle.AddToggle(entry.toggle);
        }

        public void EntryRemoved(MetaSharedAnchorEntry entry)
        {
            m_UIBatchToggle.RemoveToggle(entry.toggle);
            m_AnchorEntryById.Remove(entry.entryId);
            m_AnchorEntryIdByToggle.Remove(entry.toggle);
        }

        void Reset()
        {
            m_UIBatchToggle = GetComponent<UIBatchToggle>();
        }

        void OnEnable()
        {
            m_UIBatchToggle.selectedTogglesChanged.AddListener(OnSelectedTogglesChanged);
        }

        void OnDisable()
        {
            m_UIBatchToggle.selectedTogglesChanged.RemoveListener(OnSelectedTogglesChanged);
        }

        void OnSelectedTogglesChanged(IEnumerable<Toggle> toggles)
        {
            foreach (var toggle in toggles)
            {
                var entryId = m_AnchorEntryIdByToggle[toggle];
                var entry = m_AnchorEntryById[entryId];

                if (toggle.isOn)
                    m_SelectedEntries.Add(entry);
                else
                    m_SelectedEntries.Remove(entry);

                m_ChangedSelectedEntries.Add(entry);
            }

            m_SelectedEntriesChanged?.Invoke(m_ChangedSelectedEntries);
            m_ChangedSelectedEntries.Clear();
        }
    }
}
