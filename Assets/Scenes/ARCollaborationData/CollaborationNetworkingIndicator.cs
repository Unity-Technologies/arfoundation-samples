using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CollaborationNetworkingIndicator : MonoBehaviour
{
    [SerializeField]
    Image m_IncomingDataImage;

    public Image incomingDataImage
    {
        get { return m_IncomingDataImage; }
        set { m_IncomingDataImage = value; }
    }

    [SerializeField]
    Image m_OutgoingDataImage;

    public Image outgoingDataImage
    {
        get { return m_OutgoingDataImage; }
        set { m_OutgoingDataImage = value; }
    }

    [SerializeField]
    Image m_HasCollaborationDataImage;

    public Image hasCollaborationDataImage
    {
        get { return m_HasCollaborationDataImage; }
        set { m_HasCollaborationDataImage = value; }
    }

    static bool s_IncomingDataReceived;

    static bool s_OutgoingDataSent;

    static bool s_HasCollaborationData;

    void Update()
    {
        m_IncomingDataImage.color = s_IncomingDataReceived ? Color.green : Color.red;
        m_OutgoingDataImage.color = s_OutgoingDataSent ? Color.green : Color.red;
        m_HasCollaborationDataImage.color = s_HasCollaborationData ? Color.green : Color.red;

        s_IncomingDataReceived = false;
        s_OutgoingDataSent = false;
        s_HasCollaborationData = false;
    }

    public static void NotifyIncomingDataReceived()
    {
        s_IncomingDataReceived = true;
    }

    public static void NotifyOutgoingDataSent()
    {
        s_OutgoingDataSent = true;
    }

    public static void NotifyHasCollaborationData()
    {
        s_HasCollaborationData = true;
    }
}
