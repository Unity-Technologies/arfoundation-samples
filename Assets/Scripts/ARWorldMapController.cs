using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif

/// <summary>
/// Demonstrates the saving and loading of an
/// <a href="https://developer.apple.com/documentation/arkit/arworldmap">ARWorldMap</a>
/// </summary>
/// <remarks>
/// ARWorldMaps are only supported by ARKit, so this API is in the
/// <c>UntyEngine.XR.ARKit</c> namespace.
/// </remarks>
public class ARWorldMapController : MonoBehaviour
{
    [SerializeField]
    ARSession m_ARSession;

    [SerializeField]
    Text m_ErrorText;

    [SerializeField]
    Button m_SaveButton;

    [SerializeField]
    Button m_LoadButton;

    [SerializeField]
    Text m_LogText;

    [SerializeField]
    Text m_MappingStatus;

    /// <summary>
    /// Create an <c>ARWorldMap</c> and save it to disk.
    /// </summary>
    public void OnSaveButton()
    {
#if UNITY_IOS
        StartCoroutine(Save());
#endif
    }

    /// <summary>
    /// Load an <c>ARWorldMap</c> from disk and apply it
    /// to the current session.
    /// </summary>
    public void OnLoadButton()
    {
#if UNITY_IOS
        StartCoroutine(Load());
#endif
    }

    /// <summary>
    /// Reset the <c>ARSession</c>, destroying any existing trackables,
    /// such as planes. Upon loading a saved <c>ARWorldMap</c>, saved
    /// trackables will be restored.
    /// </summary>
    public void OnResetButton()
    {
        m_ARSession.Reset();
    }

#if UNITY_IOS
    IEnumerator Save()
    {
        var sessionSubsystem = ARSubsystemManager.sessionSubsystem;
        if (sessionSubsystem == null)
        {
            Log("No session subsystem available. Could not save.");
            yield break;
        }

        var request = sessionSubsystem.GetARWorldMapAsync();

        while (!request.status.IsDone())
            yield return null;

        if (request.status.IsError())
        {
            Log(string.Format("Session serialization failed with status {0}", request.status));
            yield break;
        }

        var worldMap = request.GetWorldMap();
        request.Dispose();

        SaveAndDisposeWorldMap(worldMap);
    }

    IEnumerator Load()
    {
        var sessionSubsystem = ARSubsystemManager.sessionSubsystem;
        if (sessionSubsystem == null)
        {
            Log("No session subsystem available. Could not load.");
            yield break;
        }

        var file = File.Open(path, FileMode.Open);
        if (file == null)
        {
            Log(string.Format("File {0} does not exist.", path));
            yield break;
        }

        Log(string.Format("Reading {0}...", path));

        int bytesPerFrame = 1024 * 10;
        var bytesRemaining = file.Length;
        var binaryReader = new BinaryReader(file);
        var allBytes = new List<byte>();
        while (bytesRemaining > 0)
        {
            var bytes = binaryReader.ReadBytes(bytesPerFrame);
            allBytes.AddRange(bytes);
            bytesRemaining -= bytesPerFrame;
            yield return null;
        }

        var data = new NativeArray<byte>(allBytes.Count, Allocator.Temp);
        data.CopyFrom(allBytes.ToArray());

        Log(string.Format("Deserializing to ARWorldMap...", path));
        ARWorldMap worldMap;
        if (ARWorldMap.TryDeserialize(data, out worldMap))
        data.Dispose();

        if (worldMap.valid)
        {
            Log("Deserialized successfully.");
        }
        else
        {
            Debug.LogError("Data is not a valid ARWorldMap.");
            yield break;
        }

        Log("Apply ARWorldMap to current session.");
        sessionSubsystem.ApplyWorldMap(worldMap);
    }

    void SaveAndDisposeWorldMap(ARWorldMap worldMap)
    {
        Log("Serializing ARWorldMap to byte array...");
        var data = worldMap.Serialize(Allocator.Temp);
        Log(string.Format("ARWorldMap has {0} bytes.", data.Length));

        var file = File.Open(path, FileMode.Create);
        var writer = new BinaryWriter(file);
        writer.Write(data.ToArray());
        writer.Close();
        data.Dispose();
        worldMap.Dispose();
        Log(string.Format("ARWorldMap written to {0}", path));
    }
#endif

    string path
    {
        get
        {
            return Path.Combine(Application.persistentDataPath, "my_session.worldmap");
        }
    }

    bool supported
    {
        get
        {
#if UNITY_IOS
            var sessionSubsystem = ARSubsystemManager.sessionSubsystem;
            if (sessionSubsystem != null)
                return sessionSubsystem.WorldMapSupported();
#endif
            return false;
        }
    }

    void Awake()
    {
        m_LogMessages = new List<string>();
    }

    void Log(string logMessage)
    {
        m_LogMessages.Add(logMessage);
    }

    void Update()
    {
        if (supported)
        {
            m_ErrorText.gameObject.SetActive(false);
            m_SaveButton.gameObject.SetActive(true);
            m_LoadButton.gameObject.SetActive(true);
            m_MappingStatus.gameObject.SetActive(true);
        }
        else
        {
            m_ErrorText.gameObject.SetActive(true);
            m_SaveButton.gameObject.SetActive(false);
            m_LoadButton.gameObject.SetActive(false);
            m_MappingStatus.gameObject.SetActive(false);
        }

        var sessionSubsystem = ARSubsystemManager.sessionSubsystem;
        if (sessionSubsystem == null)
            return;

        var numLogsToShow = 20;
        string msg = "";
        for (int i = Mathf.Max(0, m_LogMessages.Count - numLogsToShow); i < m_LogMessages.Count; ++i)
        {
            msg += m_LogMessages[i];
            msg += "\n";
        }
        m_LogText.text = msg;

#if UNITY_IOS
        m_MappingStatus.text = string.Format("Mapping Status: {0}", sessionSubsystem.GetWorldMappingStatus());
#endif
    }

    List<string> m_LogMessages;
}
