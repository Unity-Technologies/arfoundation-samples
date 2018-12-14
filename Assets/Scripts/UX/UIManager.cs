using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class UIManager : MonoBehaviour
{
    const string k_FadeOffAnim = "FadeOff";
    const string k_FadeOnAnim = "FadeOn";

    [SerializeField]
    ARPlaneManager m_PlaneManager;

    public ARPlaneManager planeManager
    {
        get { return m_PlaneManager; }
        set { m_PlaneManager = value; }
    }

    [SerializeField]
    Animator m_MoveDeviceAnimation;

    public Animator moveDeviceAnimation
    {
        get { return m_MoveDeviceAnimation; }
        set { m_MoveDeviceAnimation = value; }
    }

    [SerializeField]
    Animator m_TapToPlaceAnimation;

    public Animator tapToPlaceAnimation
    {
        get { return m_TapToPlaceAnimation; }
        set { m_TapToPlaceAnimation = value; }
    }

    static List<ARPlane> s_Planes = new List<ARPlane>();

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
