using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TCPClient))]
[RequireComponent(typeof(TCPServer))]
public class ClientServerSelector : MonoBehaviour
{
    [SerializeField]
    Button m_JoinButton;

    public Button joinButton
    {
        get { return m_JoinButton; }
        set { m_JoinButton = value; }
    }

    [SerializeField]
    Button m_HostButton;

    public Button hostButton
    {
        get { return m_HostButton; }
        set { m_HostButton = value; }
    }

    [SerializeField]
    InputField m_IPAddressField;

    public InputField ipAddressField
    {
        get { return m_IPAddressField; }
        set { m_IPAddressField = value; }
    }

    public void Join()
    {
        var client = GetComponent<TCPClient>();
        var ipAddress = m_IPAddressField.text;
        try
        {
            File.WriteAllText(GetIPAddressPath(), ipAddress);
        }
        catch (Exception e)
        {
            Logger.Log($"Could not save IP address because {e.ToString()}");
        }

        client.serverIP = ipAddress;
        client.enabled = true;
        enabled = false;
    }

    public void Host()
    {
        GetComponent<TCPClient>().enabled = false;
        GetComponent<TCPServer>().enabled = true;
        enabled = false;
    }

    string GetIPAddressPath()
    {
        return Path.Combine(Application.persistentDataPath, "ipaddress.txt");
    }

    void OnEnable()
    {
        if (File.Exists(GetIPAddressPath()))
        {
            var storedIPAddress = File.ReadAllText(GetIPAddressPath());
            if (storedIPAddress != null)
            {
                Logger.Log($"Found stored IP address {storedIPAddress}");
                m_IPAddressField.text = storedIPAddress;
            }
            else
            {
                Logger.Log($"No IP address tored at {GetIPAddressPath()}");
            }
        }

        if (m_JoinButton != null)
            m_JoinButton.gameObject.SetActive(true);

        if (m_HostButton != null)
            m_HostButton.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        if (m_JoinButton != null)
            m_JoinButton.gameObject.SetActive(false);

        if (m_HostButton != null)
            m_HostButton.gameObject.SetActive(false);

        if (m_IPAddressField != null)
            m_IPAddressField.gameObject.SetActive(false);
    }
}
