using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif

public abstract class TCPConnection : MonoBehaviour
{
    [SerializeField]
    ARSession m_Session;

    public ARSession session
    {
        get { return m_Session; }
        set { m_Session = value; }
    }

    [SerializeField]
    int m_Port = 8502;

    public int port
    {
        get { return m_Port; }
        set { m_Port = value; }
    }

    public bool connected
    {
        get
        {
            return
                (m_TcpClient != null) &&
                (m_TcpClient.Connected);
        }
    }

    protected TcpClient m_TcpClient;

    protected virtual void OnEnable()
    {
#if UNITY_IOS
        if (ARKitSessionSubsystem.supportsCollaboration)
        {
            m_ExitRequested = false;
        }
        else
#endif
        {
            Logger.Log("Collaboration is not supported by this device.");
            enabled = false;
        }
    }

    protected virtual void OnDisable()
    {
#if UNITY_IOS
        // Shutdown running threads
        m_ExitRequested = true;

        if (m_ReadThread.IsAlive)
            m_ReadThread.Join();

        if (m_SendThread.IsAlive)
            m_SendThread.Join();
#endif

        // Close down TCP connection
        if (m_TcpClient != null)
        {
            m_TcpClient.Close();
            Logger.Log("Connection closed");
        }

        m_TcpClient = null;
    }

    protected virtual void Update()
    {
#if UNITY_IOS
        if (session == null)
            return;

        var subsystem = session.subsystem as ARKitSessionSubsystem;
        if (subsystem == null)
            return;

        // Disable collaboration if we aren't connected to anyone
        subsystem.collaborationEnabled = connected;

        if (connected)
        {
            // Make sure threads are running
            if (!m_ReadThread.IsAlive)
                m_ReadThread.Start();
            if (!m_SendThread.IsAlive)
                m_SendThread.Start();

            ProcessRemoteCollaborationData(subsystem);
            CheckForLocalCollaborationData(subsystem);
        }
#endif
    }

#if UNITY_IOS
    static readonly byte[] m_Buffer = new Byte[1024];

    Queue<ARCollaborationData> m_CollaborationDataSendQueue;

    Queue<ARCollaborationData> m_CollaborationDataReadQueue;

    Thread m_ReadThread;

    Thread m_SendThread;

    bool m_ExitRequested;

    void Awake()
    {
        m_CollaborationDataSendQueue = new Queue<ARCollaborationData>();
        m_CollaborationDataReadQueue = new Queue<ARCollaborationData>();
        m_ReadThread = new Thread(ReadThreadProc);
        m_SendThread = new Thread(SendThreadProc);
    }

    void SendThreadProc()
    {
        var stream = m_TcpClient.GetStream();
        while (!m_ExitRequested)
        {
            var collaborationData = new ARCollaborationData();
            int queueSize = 0;
            lock (m_CollaborationDataSendQueue)
            {
                if (m_CollaborationDataSendQueue.Count > 0)
                {
                    collaborationData = m_CollaborationDataSendQueue.Dequeue();
                }
                queueSize = m_CollaborationDataSendQueue.Count;
            }

            if (collaborationData.valid)
            {
                using (collaborationData)
                {
                    SendData(stream, collaborationData.bytes);
                }
            }

            if (queueSize == 0)
            {
                // If there's nothing else in the queue at the moment,
                // then go to sleep for a bit.
                // Otherwise, immediately try to send the next one.
                Thread.Sleep(1);
            }
        }
    }

    unsafe void ReadThreadProc()
    {
        var stream = m_TcpClient.GetStream();
        while (!m_ExitRequested)
        {
            if (stream.DataAvailable)
            {
                var lengthBytes = ReadBytes(stream, sizeof(int));
                int expectedLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(lengthBytes, 0));

                if (expectedLength <= 0)
                {
                    Logger.Log($"Warning: received data of length {expectedLength}. Ignoring.");
                }
                else
                {
                    // Read incomming stream into byte arrary.
                    var collaborationBytes = ReadBytes(stream, expectedLength);
                    var collaborationData = new ARCollaborationData(collaborationBytes);
                    if (collaborationData.valid)
                    {
                        lock (m_CollaborationDataReadQueue)
                        {
                            m_CollaborationDataReadQueue.Enqueue(collaborationData);
                        }
                    }
                    else
                    {
                        Logger.Log($"Received {expectedLength} bytes from remote host, but the collaboration data was not valid.");
                    }
                }
            }

            Thread.Sleep(1);
        }
    }

    void CheckForLocalCollaborationData(ARKitSessionSubsystem subsystem)
    {
        // Check for new data and queue it
        if (subsystem.collaborationDataCount > 0)
        {
            CollaborationNetworkingIndicator.NotifyHasCollaborationData();
            lock (m_CollaborationDataSendQueue)
            {
                while (subsystem.collaborationDataCount > 0)
                {
                    m_CollaborationDataSendQueue.Enqueue(subsystem.DequeueCollaborationData());
                }
            }
        }
    }


    unsafe void ProcessRemoteCollaborationData(ARKitSessionSubsystem subsystem)
    {
        // Check for remote data and apply it
        lock (m_CollaborationDataReadQueue)
        {
            while (m_CollaborationDataReadQueue.Count > 0)
            {
                using (var collaborationData = m_CollaborationDataReadQueue.Dequeue())
                {
                    // Only notify user concerning large data sizes
                    if (collaborationData.bytes.Length > 1024)
                    {
                        Logger.Log($"Received {collaborationData.bytes.Length} bytes from remote host. Updating session.");
                    }

                    CollaborationNetworkingIndicator.NotifyIncomingDataReceived();

                    // Assume we only put in valid collaboration data into the queue.
                    subsystem.UpdateWithCollaborationData(collaborationData);
                }
            }
        }
    }

    static byte[] ReadBytes(NetworkStream stream, int count)
    {
        var bytes = new byte[count];
        int bytesRemaining = count;
        int offset = 0;

        while (bytesRemaining > 0)
        {
            int bytesRead = stream.Read(bytes, offset, bytesRemaining);
            offset += bytesRead;
            bytesRemaining -= bytesRead;
        }

        return bytes;
    }

    /// <summary>
    /// Send message to other device using socket connection.
    /// </summary>
    static void SendData(NetworkStream stream, NativeArray<byte> bytes)
    {
        try
        {
            var byteArray = bytes.ToArray();
            int length = IPAddress.HostToNetworkOrder(byteArray.Length);
            var lengthBytes = BitConverter.GetBytes(length);
            stream.Write(lengthBytes, 0, lengthBytes.Length);
            stream.Write(byteArray, 0, byteArray.Length);
            CollaborationNetworkingIndicator.NotifyOutgoingDataSent();

            if (byteArray.Length > 1024)
            {
                Logger.Log($"Sent {byteArray.Length} bytes to remote.");
            }
        }
        catch (SocketException socketException)
        {
            Logger.Log("Socket exception: " + socketException);
        }
    }
#endif
}
