using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Saves and loads SerializableGuids and DateTimes to a file on the devices local storage.
    /// </summary>
    public class SaveAndLoadAnchorDataToFile
    {
        readonly string m_FilePath = Path.Combine(Application.persistentDataPath, "SavedAnchorIds.json");

        bool m_Initialized;
        Awaitable m_InitializeAwaitable;
        Dictionary<SerializableGuid, DateTime> m_SavedAnchorsData = new();

        public SaveAndLoadAnchorDataToFile()
        {
            m_InitializeAwaitable = PopulateSavedAnchorIdsFromFile();
        }

        /// <summary>
        /// Saves a `SerializableGuid` to a file asynchronously, appending to the list of ids already saved.
        /// If no file exists or the file is unreadable, a new file is created.
        /// </summary>
        public async Awaitable SaveAnchorIdAsync(SerializableGuid savedAnchorId, DateTime dateTime)
        {
            try
            {
                if (!m_Initialized)
                    await m_InitializeAwaitable;

                m_SavedAnchorsData[savedAnchorId] = dateTime;
                await WriteSavedAnchorIdsToFile();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public async Awaitable SaveAnchorIdsAsync(
            ReadOnlyListSpan<ARSaveOrLoadAnchorResult> saveResults, DateTime dateTime)
        {
            try
            {
                if (!m_Initialized)
                    await m_InitializeAwaitable;

                foreach (var result in saveResults)
                {
                    if (result.resultStatus.IsError())
                        continue;

                    m_SavedAnchorsData[result.savedAnchorGuid] = dateTime;
                }

                await WriteSavedAnchorIdsToFile();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public async Awaitable EraseAnchorIdAsync(SerializableGuid savedAnchorId)
        {
            try
            {
                if (!m_Initialized)
                    await m_InitializeAwaitable;

                m_SavedAnchorsData.Remove(savedAnchorId);
                await WriteSavedAnchorIdsToFile();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        public async Awaitable EraseAnchorIdsAsync(ReadOnlyListSpan<XREraseAnchorResult> eraseResults)
        {
            try
            {
                if (!m_Initialized)
                    await m_InitializeAwaitable;

                foreach (var result in eraseResults)
                {
                    if (result.resultStatus.IsError())
                        continue;

                    m_SavedAnchorsData.Remove(result.savedAnchorGuid);
                }

                await WriteSavedAnchorIdsToFile();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Returns the set of `SerializableGuid`s from the save file.
        /// </summary>
        /// <returns>The set of `SerializableGuid`s that were saved to the file.
        /// If no file exists or the file is unreadable, an an empty set is returned.</returns>
        public async Awaitable<Dictionary<SerializableGuid, DateTime>> GetSavedAnchorsDataAsync()
        {
            if (!m_Initialized)
                await m_InitializeAwaitable;

            return m_SavedAnchorsData;
        }

        async Awaitable PopulateSavedAnchorIdsFromFile()
        {
            try
            {
                m_SavedAnchorsData.Clear();
                if (!File.Exists(m_FilePath))
                    return;

                using var streamReader = File.OpenText(m_FilePath);
                using var jsonTextReader = new JsonTextReader(streamReader);

                var kvp = (JObject)await JToken.ReadFromAsync(jsonTextReader);
                foreach (var (idAsString, dateTime) in kvp)
                {
                    var tokens = idAsString.Split("-");
                    var low = Convert.ToUInt64(tokens[0], 16);
                    var high = Convert.ToUInt64(tokens[1], 16);
                    var serializableGuid = new SerializableGuid(low, high);
                    m_SavedAnchorsData.Add(serializableGuid, (DateTime)dateTime);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                m_Initialized = true;
            }
        }

        async Awaitable WriteSavedAnchorIdsToFile()
        {
            await using var fileStream = new FileStream(m_FilePath, FileMode.Create, FileAccess.Write);
            await using var writer = new StreamWriter(fileStream);
            using var jsonWriter = new JsonTextWriter(writer);
            var serializer = new JsonSerializer();
            serializer.Serialize(jsonWriter, m_SavedAnchorsData);
        }
    }
}
