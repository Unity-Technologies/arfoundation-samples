using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Abstracts the persistence mechanism for saved anchor GUIDs and their timestamps.
    /// </summary>
    interface ISavedAnchorDataStore
    {
        /// <summary>
        /// Returns the set of saved anchor GUIDs. The <see cref="DateTime"/> value is
        /// <c>default</c> when the backing store does not record timestamps.
        /// </summary>
        Awaitable<IReadOnlyDictionary<SerializableGuid, DateTime>> GetSavedAnchorsDataAsync();

        /// <summary>
        /// Records a single saved anchor GUID.
        /// </summary>
        Awaitable SaveAnchorIdAsync(SerializableGuid savedAnchorId, DateTime dateTime);

        /// <summary>
        /// Records multiple saved anchor GUIDs from a batch save result.
        /// </summary>
        Awaitable SaveAnchorIdsAsync(ReadOnlyListSpan<ARSaveOrLoadAnchorResult> saveResults, DateTime dateTime);

        /// <summary>
        /// Removes a single saved anchor GUID.
        /// </summary>
        Awaitable EraseAnchorIdAsync(SerializableGuid savedAnchorId);

        /// <summary>
        /// Removes multiple saved anchor GUIDs from a batch erase result.
        /// </summary>
        Awaitable EraseAnchorIdsAsync(ReadOnlyListSpan<XREraseAnchorResult> eraseResults);
    }
}
