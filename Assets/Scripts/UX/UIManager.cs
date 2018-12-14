using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class UIManager : MonoBehaviour
{
    const string k_FadeOffAnim = "FadeOff";
    const string k_FadeOnAnim = "FadeOn";

    [SerializeField] ARPlaneManager m_PlaneManager;
    [SerializeField] Animator m_MoveDeviceAnimation;
    [SerializeField] Animator m_TapToPlaceAnimation;
    
    List<ARPlane> m_Planes = new List<ARPlane>();

    bool m_ShowingTapToPlace = false;
    bool m_ShowingMoveDevice = true;

    void OnEnable()
    {
        ARSubsystemManager.cameraFrameReceived += FrameChanged;
        PlaceMultipleObjectsOnPlane.onPlacedObject += PlacedObject;
    }

    void OnDisable()
    {
        ARSubsystemManager.cameraFrameReceived -= FrameChanged;
        PlaceMultipleObjectsOnPlane.onPlacedObject -= PlacedObject;
    }

    void FrameChanged(ARCameraFrameEventArgs args)
    {
        if (PlanesFound() && m_ShowingMoveDevice)
        {
            m_MoveDeviceAnimation.SetTrigger(k_FadeOffAnim);
            m_TapToPlaceAnimation.SetTrigger(k_FadeOnAnim);
            m_ShowingTapToPlace = true;
            m_ShowingMoveDevice = false;
        }
    }

    bool PlanesFound()
    {
        if (m_PlaneManager)
        {
            m_PlaneManager.GetAllPlanes(m_Planes);
            return m_Planes.Count > 0;
        }
        return false;
    }

    void PlacedObject()
    {
        if (m_ShowingTapToPlace)
        {
            m_TapToPlaceAnimation.SetTrigger(k_FadeOffAnim);
            m_ShowingTapToPlace = false;
        }
    }

}
