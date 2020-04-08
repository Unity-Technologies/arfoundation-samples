using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ARSceneSelectUI : MonoBehaviour
{
    [SerializeField]
    GameObject m_AllMenu;
    public GameObject allMenu
    {
        get { return m_AllMenu; }
        set { m_AllMenu = value; }
    }

    [SerializeField]
    GameObject m_FaceTrackingMenu;
    public GameObject faceTrackingMenu
    {
        get { return m_FaceTrackingMenu; }
        set { m_FaceTrackingMenu = value; }
    }

    [SerializeField]
    GameObject m_HumanSegmentationMenu;
    public GameObject humanSegmentationMenu
    {
        get { return m_HumanSegmentationMenu; }
        set { m_HumanSegmentationMenu = value; }
    }


    [SerializeField]
    GameObject m_LightEstimationMenu;
    public GameObject lightEstimationMenu
    {
        get { return m_LightEstimationMenu; }
        set { m_LightEstimationMenu = value; }
    }

    [SerializeField]
    GameObject m_PlaneDetectionMenu;
    public GameObject planeDetectionMenu
    {
        get { return m_PlaneDetectionMenu; }
        set { m_PlaneDetectionMenu = value; }
    }


    void Start()
    {
        if(ActiveMenu.currentMenu == MenuType.FaceTracking)
        {
            m_FaceTrackingMenu.SetActive(true);
            m_AllMenu.SetActive(false);
        }
        else if(ActiveMenu.currentMenu == MenuType.PlaneDetection)
        {
            m_PlaneDetectionMenu.SetActive(true);
            m_AllMenu.SetActive(false);
        }
        else if(ActiveMenu.currentMenu == MenuType.HumanSegmentation)
        {
            m_HumanSegmentationMenu.SetActive(true);
            m_AllMenu.SetActive(false);
        }
        else if(ActiveMenu.currentMenu == MenuType.LightEstimation)
        {
            m_LightEstimationMenu.SetActive(true);
            m_AllMenu.SetActive(false);
        }
    }

    public void SimpleARButtonPressed()
    {
        SceneManager.LoadScene("SimpleAR", LoadSceneMode.Single);
    }

    public void ImageTrackableButtonPressed()
    {
       SceneManager.LoadScene("ImageTracking", LoadSceneMode.Single);
    }

    public void AnchorsButtonPressed()
    {
        SceneManager.LoadScene("Anchors", LoadSceneMode.Single);
    }

    public void ARCollaborationDataButtonPressed()
    {
        SceneManager.LoadScene("ARCollaborationDataExample", LoadSceneMode.Single);
    }

    public void ARKitCoachingOverlayButtonPressed()
    {
        SceneManager.LoadScene("ARKitCoachingOverlay", LoadSceneMode.Single);
    }

    public void ARWorldMapButtonPressed()
    {
        SceneManager.LoadScene("ARWorldMap", LoadSceneMode.Single);
    }

    public void CameraImageButtonPressed()
    {
        SceneManager.LoadScene("CameraImage", LoadSceneMode.Single);
    }

    public void CheckSupportButtonPressed()
    {
        SceneManager.LoadScene("Check Support", LoadSceneMode.Single);
    }

    public void EnvironmentProbesButtonPressed()
    {
        SceneManager.LoadScene("EnvironmentProbes", LoadSceneMode.Single);
    }

    public void ObjectTrackingButtonPressed()
    {
        SceneManager.LoadScene("ObjectTracking", LoadSceneMode.Single);
    }

    public void PlaneOcclusionButtonPressed()
    {
        SceneManager.LoadScene("PlaneOcclusion", LoadSceneMode.Single);
    }

    public void PointCloudButtonPressed()
    {
        SceneManager.LoadScene("AllPointCloudPoints", LoadSceneMode.Single);
    }

    public void ScaleButtonPressed()
    {
        SceneManager.LoadScene("Scale", LoadSceneMode.Single);
    }

    public void SampleUXButtonPressed()
    {
        SceneManager.LoadScene("SampleUXScene", LoadSceneMode.Single);
    }

    public void FaceTrackingMenuButtonPressed()
    {
        ActiveMenu.currentMenu = MenuType.FaceTracking;
        m_FaceTrackingMenu.SetActive(true);
        m_AllMenu.SetActive(false);
    }
    public void ARCoreFaceRegionsButtonPressed()
    {
        SceneManager.LoadScene("ARCoreFaceRegions", LoadSceneMode.Single);
    }
    public void ARKitFaceBlendShapesButtonPressed()
    {
        SceneManager.LoadScene("ARKitFaceBlendShapes", LoadSceneMode.Single);
    }
    public void EyeLasersButtonPressed()
    {
        SceneManager.LoadScene("EyeLasers", LoadSceneMode.Single);
    }
    public void EyePosesButtonPressed()
    {
        SceneManager.LoadScene("EyePoses", LoadSceneMode.Single);
    }

    public void FaceMeshButtonPressed()
    {
        SceneManager.LoadScene("FaceMesh", LoadSceneMode.Single);
    }

    public void FacePoseButtonPressed()
    {
        SceneManager.LoadScene("FacePose", LoadSceneMode.Single);
    }

    public void FixationPointButtonPressed()
    {
        SceneManager.LoadScene("FixationPoint", LoadSceneMode.Single);
    }

    public void RearCameraWithFrontCameraFaceMeshButtonPressed()
    {
        SceneManager.LoadScene("WorldCameraWithUserFacingFaceTracking", LoadSceneMode.Single);
    }

    public void HumanSegmentationMenuButtonPressed()
    {
        ActiveMenu.currentMenu = MenuType.HumanSegmentation;
        m_HumanSegmentationMenu.SetActive(true);
        m_AllMenu.SetActive(false);
    }
    public void HumanSegmentation2DButtonPressed()
    {
        SceneManager.LoadScene("HumanBodyTracking2D", LoadSceneMode.Single);
    }
    public void HumanSegmentation3DButtonPressed()
    {
        SceneManager.LoadScene("HumanBodyTracking3D", LoadSceneMode.Single);
    }
    public void HumanSegmentationImagesButtonPressed()
    {
        SceneManager.LoadScene("HumanSegmentationImages", LoadSceneMode.Single);
    }

    public void LightEstimationMenuButtonPressed()
    {
        ActiveMenu.currentMenu = MenuType.LightEstimation;
        m_LightEstimationMenu.SetActive(true);
        m_AllMenu.SetActive(false);
    }


     public void LightEstimationButtonPressed()
    {
        SceneManager.LoadScene("LightEstimation", LoadSceneMode.Single);
    }

    public void PlaneDetectionMenuButtonPressed()
    {
        ActiveMenu.currentMenu = MenuType.PlaneDetection;
        m_PlaneDetectionMenu.SetActive(true);
        m_AllMenu.SetActive(false);
    }
    public void FeatheredPlanesButtonPressed()
    {
        SceneManager.LoadScene("FeatheredPlanes", LoadSceneMode.Single);
    }

    public void PlaneClassificationButtonPressed()
    {
        SceneManager.LoadScene("PlaneClassification", LoadSceneMode.Single);
    }

    public void TogglePlaneDetectionButtonPressed()
    {
        SceneManager.LoadScene("TogglePlaneDetection", LoadSceneMode.Single);
    }


    public void BackButtonPressed()
    {
        ActiveMenu.currentMenu = MenuType.Main;
        m_FaceTrackingMenu.SetActive(false);
        m_PlaneDetectionMenu.SetActive(false);
        m_HumanSegmentationMenu.SetActive(false);
        m_LightEstimationMenu.SetActive(false);
        m_AllMenu.SetActive(true);
    }
}
