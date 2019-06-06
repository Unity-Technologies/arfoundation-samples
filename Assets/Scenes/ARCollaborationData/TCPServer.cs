using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Unity.Collections;

public class TCPServer : TCPConnection
{
    TcpListener m_TcpListener;

    protected override void OnEnable()
    {
        base.OnEnable();

        m_TcpListener = new TcpListener(IPAddress.Any, port);
        m_TcpListener.Start();
        Logger.Log($"Listening for connection on port {port}...");
    }

    protected override void Update()
    {
        if (m_TcpClient == null && m_TcpListener.Pending())
        {
            Logger.Log("Connection pending...");
            m_TcpClient = m_TcpListener.AcceptTcpClient();
            Logger.Log($"Connection established. {((IPEndPoint)m_TcpClient.Client.RemoteEndPoint).Address}");
        }

        base.Update();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        m_TcpListener.Stop();
        m_TcpListener = null;
    }
}