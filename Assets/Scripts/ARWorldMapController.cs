using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
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
    [Tooltip("The ARSession component controlling the session from which to generate ARWorldMaps.")]
    [SerializeField]
    ARSession m_ARSession;

    /// <summary>
    /// The ARSession component controlling the session from which to generate ARWorldMaps.
    /// </summary>
    public ARSession arSession
    {
        get { return m_ARSession; }
        set { m_ARSession = value; }
    }

    [Tooltip("UI Text component to display error messages")]
    [SerializeField]
    Text m_ErrorText;

    /// <summary>
    /// The UI Text component used to display error messages
    /// </summary>
    public Text errorText
    {
        get { return m_ErrorText; }
        set { m_ErrorText = value; }
    }

    [Tooltip("The UI Text element used to display log messages.")]
    [SerializeField]
    Text m_LogText;

    /// <summary>
    /// The UI Text element used to display log messages.
    /// </summary>
    public Text logText
    {
        get { return m_LogText; }
        set { m_LogText = value; }
    }

    [Tooltip("The UI Text element used to display the current AR world mapping status.")]
    [SerializeField]
    Text m_MappingStatusText;

    /// <summary>
    /// The UI Text element used to display the current AR world mapping status.
    /// </summary>
    public Text mappingStatusText
    {
        get { return m_MappingStatusText; }
        set { m_MappingStatusText = value; }
    }

    [Tooltip("A UI button component which will generate an ARWorldMap and save it to disk.")]
    [SerializeField]
    Button m_SaveButton;

    /// <summary>
    /// A UI button component which will generate an ARWorldMap and save it to disk.
    /// </summary>
    public Button saveButton
    {
        get { return m_SaveButton; }
        set { m_SaveButton = value; }
    }

    [Tooltip("A UI button component which will load a previously saved ARWorldMap from disk and apply it to the current session.")]
    [SerializeField]
    Button m_LoadButton;

    /// <summary>
    /// A UI button component which will load a previously saved ARWorldMap from disk and apply it to the current session.
    /// </summary>
    public Button loadButton
    {
        get { return m_LoadButton; }
        set { m_LoadButton = value; }
    }

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
        var sessionSubsystem = (ARKitSessionSubsystem)m_ARSession.subsystem;
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
        var sessionSubsystem = (ARKitSessionSubsystem)m_ARSession.subsystem;
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
            var sessionSubsystem = (ARKitSessionSubsystem)m_ARSession.subsystem;
            if (sessionSubsystem != null)
                return sessionSubsystem.worldMapSupported;
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

    static void SetActive(Button button, bool active)
    {
        if (button != null)
            button.gameObject.SetActive(active);
    }

    static void SetActive(Text text, bool active)
    {
        if (text != null)
            text.gameObject.SetActive(active);
    }

    static void SetText(Text text, string value)
    {
        if (text != null)
            text.text = value;
    }

    void Update()
    {
        if (supported)
        {
            SetActive(errorText, false);
            SetActive(saveButton, true);
            SetActive(loadButton, true);
            SetActive(mappingStatusText, true);
        }
        else
        {
            SetActive(errorText, true);
            SetActive(saveButton, false);
            SetActive(loadButton, false);
            SetActive(mappingStatusText, false);
        }

#if UNITY_IOS
        var sessionSubsystem = (ARKitSessionSubsystem)m_ARSession.subsystem;
#else
        XRSessionSubsystem sessionSubsystem = null;
#endif
        if (sessionSubsystem == null)
            return;

        var numLogsToShow = 20;
        string msg = "";
        for (int i = Mathf.Max(0, m_LogMessages.Count - numLogsToShow); i < m_LogMessages.Count; ++i)
        {
            msg += m_LogMessages[i];
            msg += "\n";
        }
        SetText(logText, msg);

#if UNITY_IOS
        SetText(mappingStatusText, string.Format("Mapping Status: {0}", sessionSubsystem.worldMappingStatus));
#endif
    }

    List<string> m_LogMessages;
}
