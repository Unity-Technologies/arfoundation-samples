using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if METAOPENXR_2_2_OR_NEWER && UNITY_ANDROID
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.OpenXR.Features.Meta;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MetaBatchSharedAnchorButtons : MonoBehaviour
    {
        [Header("Class References")]
        [SerializeField]
        ARAnchorManager m_ARAnchorManager;

        [SerializeField]
        MetaBatchSharedAnchors m_MetaBatchSharedAnchors;

        [Header("UI References")]

        [SerializeField]
        Button m_BatchRemoveButton;

        [SerializeField]
        Button m_BatchShareButton;

        int m_SelectedCount;
        int m_SelectedLocallyCreatedAnchorCount;
        bool m_IsSharedAnchorsSupported;

        void Awake()
        {
            if (m_ARAnchorManager == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_ARAnchorManager)} is null."), this);

            if (m_MetaBatchSharedAnchors == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_MetaBatchSharedAnchors)} is null."), this);

            if (m_BatchRemoveButton == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_BatchRemoveButton)} is null."), this);

            if (m_BatchShareButton == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_BatchShareButton)} is null."), this);

            m_MetaBatchSharedAnchors.selectedEntriesChanged.AddListener(OnSelectedEntriesChanged);

#if METAOPENXR_2_2_OR_NEWER && UNITY_ANDROID
            if (m_ARAnchorManager.subsystem is MetaOpenXRAnchorSubsystem metaAnchorSubsystem)
                m_IsSharedAnchorsSupported = metaAnchorSubsystem.isSharedAnchorsSupported == Supported.Supported;
#endif
        }

        void OnSelectedEntriesChanged(IReadOnlyCollection<MetaSharedAnchorEntry> changedSelectedEntries)
        {
            foreach (var entry in changedSelectedEntries)
            {
                if (entry.toggle.isOn)
                {
                    m_SelectedCount += 1;
                    m_SelectedLocallyCreatedAnchorCount += !entry.isSynced ? 1 : 0;
                    entry.inSceneStateStateChanged.AddListener(OnAnchorInSceneStateChanged);
                    entry.anchorShared.AddListener(OnAnchorShared);
                }
                else
                {
                    m_SelectedCount -= 1;
                    m_SelectedLocallyCreatedAnchorCount += !entry.isSynced ? -1 : 0;
                    entry.inSceneStateStateChanged.RemoveListener(OnAnchorInSceneStateChanged);
                    entry.anchorShared.RemoveListener(OnAnchorShared);
                }
            }

            UpdateInSceneStateButtons();
            UpdateShareStateButtons();
        }

        void OnAnchorInSceneStateChanged(MetaSharedAnchorEntry entry, bool isInScene)
        {
            m_SelectedCount -= 1;
            m_SelectedLocallyCreatedAnchorCount += isInScene && !entry.isSynced ? -1 : 0;
            UpdateInSceneStateButtons();
            UpdateShareStateButtons();
        }

        void OnAnchorShared()
        {
            UpdateShareStateButtons();
        }

        void UpdateInSceneStateButtons()
        {
            m_BatchRemoveButton.SetEnabled(m_SelectedCount > 0);
        }

        void UpdateShareStateButtons()
        {
            if (!m_IsSharedAnchorsSupported)
                return;

            m_BatchShareButton.SetEnabled(m_SelectedLocallyCreatedAnchorCount > 0);
        }
    }
}
