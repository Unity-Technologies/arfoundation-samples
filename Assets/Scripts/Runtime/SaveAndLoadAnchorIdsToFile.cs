using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class SaveAndLoadAnchorIdsToFile
    {
        readonly string m_FilePath = Path.Combine(Application.persistentDataPath, "SavedAnchorIds.json");

        bool m_Initialized;
        Awaitable m_InitializeTask;
        HashSet<SerializableGuid> m_SavedAnchorIds = new();

        public SaveAndLoadAnchorIdsToFile()
        {
            m_InitializeTask = PopulateSavedAnchorIdsFromFile();
        }

        /// <summary>
        /// Saves a `SerializableGuid` to a file asynchronously, appending to the list of ids already saved.
        /// If no file exists or the file is unreadable, a new file is created.
        /// </summary>
        /// <param name="savedAnchorId">The `SerializableGuid` to save.</param>
        public async Awaitable SaveAnchorIdAsync(SerializableGuid savedAnchorId)
        {
            try
            {
                if (!m_Initialized)
                    await m_InitializeTask;

                m_SavedAnchorIds.Add(savedAnchorId);
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
                    await m_InitializeTask;

                m_SavedAnchorIds.Remove(savedAnchorId);
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
        public async Awaitable<HashSet<SerializableGuid>> LoadSavedAnchorIdsAsync()
        {
            if (!m_Initialized)
                await m_InitializeTask;

            return m_SavedAnchorIds;
        }

        private async Awaitable PopulateSavedAnchorIdsFromFile()
        {
            try
            {
                m_SavedAnchorIds.Clear();
                if (!File.Exists(m_FilePath))
                    return;

                using var streamReader = File.OpenText(m_FilePath);
                using var jsonTextReader = new JsonTextReader(streamReader);

                var jArray = (JArray)await JToken.ReadFromAsync(jsonTextReader);
                foreach (var jObject in jArray)
                {
                    var idAsJToken = jObject["guid"];
                    if (idAsJToken == null)
                        continue;

                    var idAsString = idAsJToken.Value<string>();
                    var guid = new Guid(idAsString);
                    var serializableGuid = new SerializableGuid(guid);
                    m_SavedAnchorIds.Add(serializableGuid);
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
            var jsonString = JsonConvert.SerializeObject(m_SavedAnchorIds, Formatting.Indented);
            await File.WriteAllTextAsync(m_FilePath, jsonString);
        }
    }
}
