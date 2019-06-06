using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARFaceManager))]
public class DisplayFaceInfo : MonoBehaviour
{
    [SerializeField]
    Text m_FaceInfoText;

    public Text faceInfoText
    {
        get { return m_FaceInfoText; }
        set { m_FaceInfoText = value; }
    }

    ARFaceManager m_FaceManager;

    void Awake()
    {
        m_FaceManager = GetComponent<ARFaceManager>();
    }

    void Update()
    {
        if (m_FaceManager.subsystem != null && faceInfoText != null)
        {
            faceInfoText.text = $"Supported number of tracked faces: {m_FaceManager.supportedFaceCount}\n" +
                                $"Max number of faces to track: {m_FaceManager.maximumFaceCount}\n" +
                                $"Number of tracked faces: {m_FaceManager.trackables.count}";
        }
    }
}
