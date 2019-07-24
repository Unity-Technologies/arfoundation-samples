using UnityEngine;
using Unity.iOS.Multipeer;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARKit;

public class CollaborativeSession : MonoBehaviour
{
    [SerializeField]
    ARSession m_ARSession;

    public ARSession arSession
    {
        get { return m_ARSession; }
        set { m_ARSession = value; }
    }

    [SerializeField]
    string m_SessionName;

    public string sessionName
    {
        get { return m_SessionName; }
        set { m_SessionName = value; }
    }

    MCSession m_MCSession;

    ARKitSessionSubsystem GetSubsystem()
    {
        if (m_ARSession == null)
            return null;

        return m_ARSession.subsystem as ARKitSessionSubsystem;
    }

    void Awake()
    {
        m_MCSession = new MCSession(SystemInfo.deviceName, m_SessionName);
    }

    void OnEnable()
    {
        var subsystem = GetSubsystem();
        if (!ARKitSessionSubsystem.supportsCollaboration || subsystem == null)
        {
            enabled = false;
            return;
        }

        subsystem.collaborationEnabled = true;
        m_MCSession.Enabled = true;
    }

    void OnDisable()
    {
        m_MCSession.Enabled = false;

        var subsystem = GetSubsystem();
        if (subsystem != null)
            subsystem.collaborationEnabled = false;
    }

    void Update()
    {
        var subsystem = GetSubsystem();
        if (subsystem == null)
            return;

        // Check for new collaboration data
        while (subsystem.collaborationDataCount > 0)
        {
            using (var collaborationData = subsystem.DequeueCollaborationData())
            {
                if (collaborationData.priority == ARCollaborationDataPriority.Critical)
                {
                    CollaborationNetworkingIndicator.NotifyHasCollaborationData();

                    if (m_MCSession.ConnectedPeerCount == 0)
                        continue;

                    using (var serializedData = collaborationData.ToSerialized())
                    using (var data = new NSData(serializedData.bytes, false))
                    {
                        m_MCSession.SendToAllPeers(data, MCSessionSendDataMode.Reliable);
                        Logger.Log($"Sent {data.Length} bytes of collaboration data.");
                    }
                }
            }
        }

        // Check for incoming data
        while (m_MCSession.ReceivedDataQueueSize > 0)
        {
            CollaborationNetworkingIndicator.NotifyIncomingDataReceived();

            using (var data = m_MCSession.DequeueReceivedData())
            using (var collaborationData = new ARCollaborationData(data.Bytes))
            {
                if (collaborationData.valid)
                {
                    subsystem.UpdateWithCollaborationData(collaborationData);
                    Logger.Log($"Received {data.Bytes.Length} bytes of collaboration data.");
                }
                else
                {
                    Logger.Log($"Received {data.Bytes.Length} bytes from remote, but the collaboration data was not valid.");
                }
            }
        }
    }

    void OnDestroy()
    {
        m_MCSession.Dispose();
    }
}
