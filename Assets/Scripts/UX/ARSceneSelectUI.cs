using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ARSceneSelectUI : MonoBehaviour
{
    public GameObject allMenu;
    public GameObject faceTrackingMenu;
    public GameObject humanSegmentationMenu;
    public GameObject lightEstimationMenu;
    public GameObject planeDetectionMenu;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
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


    //TODO: Face Tracking, Body Segementation, Light Estimation, Plane Detection, Stuff...

    //Face Tracking
    public void FaceTrackingMenuButtonPressed()
    {
        faceTrackingMenu.SetActive(true);
        allMenu.SetActive(false);
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
        SceneManager.LoadScene("FixationPoin", LoadSceneMode.Single);
    }

    public void RearCameraWithFrontCameraFaceMeshButtonPressed()
    {
        SceneManager.LoadScene("RearCameraWithFrontCameraFaceMesh", LoadSceneMode.Single);
    }


    //Body Segmentation
    public void HumanSegmentationMenuButtonPressed()
    {
        humanSegmentationMenu.SetActive(true);
        allMenu.SetActive(false);
    }
    public void HumanSegmentation2DButtonPressed()
    {
        SceneManager.LoadScene("HumanBodyTracking2D", LoadSceneMode.Single);
    }
    public void HumanBodyTracking3DButtonPressed()
    {
        SceneManager.LoadScene("HumanBodyTracking3D", LoadSceneMode.Single);
    }
    public void HumanSegmentationImagesButtonPressed()
    {
        SceneManager.LoadScene("HumanSegmentationImages", LoadSceneMode.Single);
    }

    //Light Estimation
    public void LightEstimationMenuButtonPressed()
    {
        lightEstimationMenu.SetActive(true);
        allMenu.SetActive(false);
    }

    public void ARKitHDRLightEstimationButtonPressed()
    {
        SceneManager.LoadScene("ARKitHDRLightEstimation", LoadSceneMode.Single);
    }

     public void LightEstimationButtonPressed()
    {
        SceneManager.LoadScene("Light Estimation", LoadSceneMode.Single);
    }

    //Plane Detection
    public void PlaneDetectionMenuButtonPressed()
    {
        planeDetectionMenu.SetActive(true);
        allMenu.SetActive(false);
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
        faceTrackingMenu.SetActive(false);
        planeDetectionMenu.SetActive(false);
        humanSegmentationMenu.SetActive(false);
        lightEstimationMenu.SetActive(false);
        allMenu.SetActive(true);
    }
}
