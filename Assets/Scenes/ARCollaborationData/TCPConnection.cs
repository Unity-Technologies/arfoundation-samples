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
                // Serialize the collaboration data to a byte array
                SerializedARCollaborationData serializedData;
                using (collaborationData)
                {
                    // ARCollaborationData can be diposed after being serialized to bytes.
                    serializedData = collaborationData.ToSerialized();
                }

                using (serializedData)
                {
                    // Get the raw data as a NativeSlice
                    var collaborationBytes = serializedData.bytes;

                    // Construct the message header
                    var header = new MessageHeader
                    {
                        messageSize = collaborationBytes.Length,
                        messageType = MessageType.CollaborationData
                    };

                    // Send the header followed by the ARCollaborationData bytes
                    m_WriteBuffer.Send(stream, header);
                    m_WriteBuffer.Send(stream, collaborationBytes);
                    Logger.Log($"Sent {collaborationBytes.Length} bytes of collaboration data.");
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
            // Loop until there is data available
            if (!stream.DataAvailable)
            {
                Thread.Sleep(1);
                continue;
            }

            // Read the header
            var messageHeader = ReadMessageHeader(stream);

            // Handle the message
            switch (messageHeader.messageType)
            {
                case MessageType.CollaborationData:
                    var collaborationData = ReadCollaborationData(stream, messageHeader.messageSize);
                    if (collaborationData.valid)
                    {
                        // Only store critical data updates; optional updates can come every frame.
                        if (collaborationData.priority == ARCollaborationDataPriority.Critical)
                        {
                            lock (m_CollaborationDataReadQueue)
                            {
                                m_CollaborationDataReadQueue.Enqueue(collaborationData);
                            }
                            Logger.Log($"Received {messageHeader.messageSize} bytes of collaboration data.");
                        }
                    }
                    else
                    {
                        Logger.Log($"Received {messageHeader.messageSize} bytes from remote, but the collaboration data was not valid.");
                    }
                    break;

                default:
                    Logger.Log($"Unhandled message type '{messageHeader.messageType}'. Ignoring.");

                    // We don't understand this message, but read it out anyway
                    // so we can process the next message
                    int bytesRemaining = messageHeader.messageSize;
                    while (bytesRemaining > 0)
                    {
                        bytesRemaining -= m_ReadBuffer.Read(stream, 0, Mathf.Min(bytesRemaining, m_ReadBuffer.bufferSize));
                    }
                    break;
            }
        }
    }

    void CheckForLocalCollaborationData(ARKitSessionSubsystem subsystem)
    {
        // Exit if no new data is available
        if (subsystem.collaborationDataCount == 0)
            return;

        lock (m_CollaborationDataSendQueue)
        {
            // Enqueue all new collaboration data with critical priority
            while (subsystem.collaborationDataCount > 0)
            {
                var collaborationData = subsystem.DequeueCollaborationData();

                // As all data in this sample is sent over TCP, only send critical data
                if (collaborationData.priority == ARCollaborationDataPriority.Critical)
                {
                    m_CollaborationDataSendQueue.Enqueue(collaborationData);
                    CollaborationNetworkingIndicator.NotifyHasCollaborationData();
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
                    // Assume we only put in valid collaboration data into the queue.
                    subsystem.UpdateWithCollaborationData(collaborationData);
                }
            }
        }
    }

    const int k_BufferSize = 10240;

    NetworkBuffer m_ReadBuffer = new NetworkBuffer(k_BufferSize);

    NetworkBuffer m_WriteBuffer = new NetworkBuffer(k_BufferSize);

    MessageHeader ReadMessageHeader(NetworkStream stream)
    {
        int bytesRead = m_ReadBuffer.Read(stream, 0, MessageHeader.k_EncodedSize);
        return new MessageHeader(m_ReadBuffer.buffer, bytesRead);
    }

    ARCollaborationData ReadCollaborationData(NetworkStream stream, int size)
    {
        var builder = new ARCollaborationDataBuilder();
        try
        {
            int bytesRemaining = size;
            while (bytesRemaining > 0)
            {
                int bytesRead = m_ReadBuffer.Read(stream, 0, Mathf.Min(bytesRemaining, m_ReadBuffer.bufferSize));
                builder.Append(m_ReadBuffer.buffer, 0, bytesRead);
                bytesRemaining -= bytesRead;
            }

            return builder.ToCollaborationData();
        }
        finally
        {
            builder.Dispose();
        }
    }
#endif
}
