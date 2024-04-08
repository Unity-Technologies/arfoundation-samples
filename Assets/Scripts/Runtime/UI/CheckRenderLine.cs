using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals.XRInteractorLineVisual))]
#pragma warning disable CS0618 // temporary fix to remove console warnings while we decide what to do with this component
[RequireComponent(typeof(ActionBasedController))]
#pragma warning restore CS0618
public class CheckRenderLine : MonoBehaviour
{
#pragma warning disable CS0618
    ActionBasedController m_Controller;
#pragma warning restore CS0618
    UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals.XRInteractorLineVisual m_InteractorLine;

    [SerializeField]
    Camera m_CameraAR;

    public Camera cameraAR
    {
        get => m_CameraAR;
        set => m_CameraAR = value;
    }

    void LogDeprecatedWarning()
    {
        Debug.LogWarning($"{nameof(CheckRenderLine)} uses deprecated functionality from XRI 2.0. Avoid using this component.", this);
    }
    
    void Reset()
    {
        LogDeprecatedWarning();
    }

    void Start()
    {
        LogDeprecatedWarning();

        if(!m_CameraAR.stereoEnabled)
        {
            enabled = false;
        }
#pragma warning disable CS0618
        m_Controller = GetComponent<ActionBasedController>();
#pragma warning restore CS0618
        m_InteractorLine = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals.XRInteractorLineVisual>();
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
