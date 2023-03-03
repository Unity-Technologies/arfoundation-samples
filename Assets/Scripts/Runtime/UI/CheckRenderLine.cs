using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

[RequireComponent(typeof(XRInteractorLineVisual))]
[RequireComponent(typeof(ActionBasedController))]
public class CheckRenderLine : MonoBehaviour
{
    ActionBasedController m_Controller;
    XRInteractorLineVisual m_InteractorLine;

    [SerializeField]
    Camera m_CameraAR;

    public Camera cameraAR
    {
        get => m_CameraAR;
        set => m_CameraAR = value;
    }

    void Start()
    {
        if(!m_CameraAR.stereoEnabled)
        {
            enabled = false;
        }

        m_Controller = GetComponent<ActionBasedController>();
        m_InteractorLine = GetComponent<XRInteractorLineVisual>();
    }

    void Update()
    {
        HandleLineRender();
    }

    void HandleLineRender()
    {
        var state = m_Controller.currentControllerState;
        if(state != null && (state.inputTrackingState & InputTrackingState.Position) != 0)
        {
            m_InteractorLine.enabled = true;
            if (m_InteractorLine.reticle != null)
                m_InteractorLine.reticle.SetActive(true);
        }
        else
        {
            m_InteractorLine.enabled = false;
            if (m_InteractorLine.reticle != null)
                m_InteractorLine.reticle.SetActive(false);
        }
    }
}
