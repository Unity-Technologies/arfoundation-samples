using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MetaBatchPersistentAnchorButtons : MonoBehaviour
    {
        [Header("Class References")]
        [SerializeField]
        ARAnchorManager m_ARAnchorManager;

        [SerializeField]
        MetaBatchPersistentAnchors m_MetaBatchPersistentAnchors;

        [Header("UI References")]
        [SerializeField]
        Button m_BatchSaveButton;

        [SerializeField]
        Button m_BatchEraseButton;

        [SerializeField]
        Button m_BatchLoadButton;

        [SerializeField]
        Button m_BatchRemoveButton;

        int m_SelectedAnchorsSavedCount;
        int m_SelectedAnchorsInSceneCount;

        void Awake()
        {
            if (m_ARAnchorManager == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_ARAnchorManager)} is null."), this);

            if (m_MetaBatchPersistentAnchors == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_MetaBatchPersistentAnchors)} is null."), this);

            if (m_BatchSaveButton == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_BatchSaveButton)} is null."), this);

            if (m_BatchEraseButton == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_BatchEraseButton)} is null."), this);

            if (m_BatchLoadButton == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_BatchLoadButton)} is null."), this);

            if (m_BatchRemoveButton == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_BatchRemoveButton)} is null."), this);

            m_MetaBatchPersistentAnchors.selectedEntriesChanged.AddListener(OnSelectedEntriesChanged);
        }

        void OnSelectedEntriesChanged(IReadOnlyCollection<MetaPersistentAnchorEntry> changedSelectedEntries)
        {
            foreach (var entry in changedSelectedEntries)
            {
                if (entry.toggle.isOn)
                {
                    m_SelectedAnchorsInSceneCount += entry.isInScene ? 1 : 0;
                    m_SelectedAnchorsSavedCount += entry.isSaved ? 1 : 0;

                    entry.inSceneStateStateChanged.AddListener(OnAnchorInSceneStateChanged);
                    entry.savedStateChanged.AddListener(OnAnchorSavedStateChanged);
                }
                else
                {
                    m_SelectedAnchorsInSceneCount += entry.isInScene ? -1 : 0;
                    m_SelectedAnchorsSavedCount += entry.isSaved ? -1 : 0;

                    entry.inSceneStateStateChanged.RemoveListener(OnAnchorInSceneStateChanged);
                    entry.savedStateChanged.RemoveListener(OnAnchorSavedStateChanged);
                }
            }

            UpdateInSceneStateButtons();
            UpdateSaveStateButtons();
        }

        void OnAnchorInSceneStateChanged(bool isInScene)
        {
            m_SelectedAnchorsInSceneCount += isInScene ? 1 : -1;
            UpdateInSceneStateButtons();
        }

        void OnAnchorSavedStateChanged(bool isSaved)
        {
            m_SelectedAnchorsSavedCount += isSaved ? 1 : -1;
            UpdateSaveStateButtons();
        }

        void UpdateInSceneStateButtons()
        {
            var isAtLeastOneSelectedAnchorInScene = m_SelectedAnchorsInSceneCount > 0;

            if (m_ARAnchorManager.subsystem.subsystemDescriptor.supportsSaveAnchor)
                m_BatchSaveButton.SetEnabled(isAtLeastOneSelectedAnchorInScene);

            m_BatchRemoveButton.SetEnabled(isAtLeastOneSelectedAnchorInScene);
        }

        void UpdateSaveStateButtons()
        {
            var isAtLeastOneSelectedAnchorSaved = m_SelectedAnchorsSavedCount > 0;
            var areAllSelectedEntriesSaved =
                m_MetaBatchPersistentAnchors.selectedEntries.Count != 0 &&
                m_SelectedAnchorsSavedCount == m_MetaBatchPersistentAnchors.selectedEntries.Count;

            if (m_ARAnchorManager.subsystem.subsystemDescriptor.supportsEraseAnchor)
            {
                m_BatchEraseButton.SetEnabled(isAtLeastOneSelectedAnchorSaved);
                m_BatchEraseButton.gameObject.SetActive(areAllSelectedEntriesSaved);
            }

            if (m_ARAnchorManager.subsystem.subsystemDescriptor.supportsLoadAnchor)
            {
                m_BatchLoadButton.SetEnabled(isAtLeastOneSelectedAnchorSaved);
                m_BatchSaveButton.gameObject.SetActive(!areAllSelectedEntriesSaved);
            }
        }
    }
}
