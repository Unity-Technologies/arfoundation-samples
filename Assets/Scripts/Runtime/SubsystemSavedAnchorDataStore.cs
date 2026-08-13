using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// An <see cref="ISavedAnchorDataStore"/> backed by the AR anchor subsystem.
    /// The subsystem owns persistence, so mutation methods are no-ops.
    /// </summary>
    class SubsystemSavedAnchorDataStore : ISavedAnchorDataStore
    {
        readonly ARAnchorManager m_AnchorManager;
        readonly Dictionary<SerializableGuid, DateTime> m_SavedAnchors = new();

        public SubsystemSavedAnchorDataStore(ARAnchorManager anchorManager)
        {
            m_AnchorManager = anchorManager;
        }

        public async Awaitable<IReadOnlyDictionary<SerializableGuid, DateTime>> GetSavedAnchorsDataAsync()
        {
            m_SavedAnchors.Clear();
            var result = await m_AnchorManager.TryGetSavedAnchorIdsAsync(Allocator.Temp);
            if (result.status.IsSuccess())
            {
                foreach (var guid in result.value)
                {
                    m_SavedAnchors[guid] = default;
                }
            }

            return m_SavedAnchors;
        }

#pragma warning disable CS1998 // We are intentionally doing sync over async on empty Awaitables
        // ReSharper disable AsyncMethodWithoutAwait

        // These methods do nothing -- the subsystem is responsible to track persisted anchors
        public async Awaitable SaveAnchorIdAsync(SerializableGuid savedAnchorId, DateTime dateTime) { }

        public async Awaitable SaveAnchorIdsAsync(
            ReadOnlyListSpan<ARSaveOrLoadAnchorResult> saveResults, DateTime dateTime) { }

        public async Awaitable EraseAnchorIdAsync(SerializableGuid savedAnchorId) { }

        public async Awaitable EraseAnchorIdsAsync(ReadOnlyListSpan<XREraseAnchorResult> eraseResults) { }
#pragma warning restore CS1998
    }
}
