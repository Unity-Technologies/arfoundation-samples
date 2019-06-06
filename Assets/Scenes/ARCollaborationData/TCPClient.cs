using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ClientServerSelector))]
public class TCPClient : TCPConnection
{
    string m_ServerIP;

    public string serverIP
    {
        get { return m_ServerIP; }
        set
        {
            if (enabled)
                throw new InvalidOperationException("Cannot change server IP address while enabled.");

            m_ServerIP = value;
        }
    }

    public void Connect()
    {
        Logger.Log($"Connecting to {serverIP} on port {port}");

        try
        {
            m_TcpClient = new TcpClient(serverIP, port);
            Logger.Log("Connected!");
        }
        catch (SocketException e)
        {
            Logger.Log(e.Message);
            enabled = false;
            GetComponent<ClientServerSelector>().enabled = true;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Connect();
    }
}
