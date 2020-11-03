using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class ARSceneSelectUI : MonoBehaviour
    {
        [SerializeField]
        Scrollbar m_HorizontalScrollBar;
        public Scrollbar horizontalScrollBar
        {
            get => m_HorizontalScrollBar;
            set => m_HorizontalScrollBar = value;
        }

        [SerializeField]
        Scrollbar m_VerticalScrollBar;
        public Scrollbar verticalScrollBar
        {
            get => m_VerticalScrollBar;
            set => m_VerticalScrollBar = value;
        }

        [SerializeField]
        GameObject m_AllMenu;
        public GameObject allMenu
        {
            get => m_AllMenu;
            set => m_AllMenu = value;
        }

        [SerializeField]
        GameObject m_ImageTrackingMenu;
        public GameObject imageTrackingMenu
        {
            get => m_ImageTrackingMenu;
            set => m_ImageTrackingMenu = value;
        }

        [SerializeField]
        GameObject m_FaceTrackingMenu;
        public GameObject faceTrackingMenu
        {
            get => m_FaceTrackingMenu;
            set => m_FaceTrackingMenu = value;
        }

        [SerializeField]
        GameObject m_BodyTrackingMenu;
        public GameObject bodyTrackingMenu
        {
            get { return m_BodyTrackingMenu; }
            set { m_BodyTrackingMenu = value; }
        }

        [SerializeField]
        GameObject m_PlaneDetectionMenu;
        public GameObject planeDetectionMenu
        {
            get => m_PlaneDetectionMenu;
            set => m_PlaneDetectionMenu = value;
        }

        [SerializeField]
        GameObject m_MeshingMenu;
        public GameObject meshingMenu
        {
            get => m_MeshingMenu;
            set => m_MeshingMenu = value;
        }

        [SerializeField]
        GameObject m_DepthMenu;
        public GameObject depthMenu
        {
            get => m_DepthMenu;
            set => m_DepthMenu = value;
        }

        [SerializeField]
        GameObject m_LightEstimationMenu;
        public GameObject lightEstimationMenu
        {
            get => m_LightEstimationMenu;
            set => m_LightEstimationMenu = value;
        }

        void Start()
        {
            if(ActiveMenu.currentMenu == MenuType.FaceTracking)
            {
                m_FaceTrackingMenu.SetActive(true);
                m_AllMenu.SetActive(false);
            }
            else if(ActiveMenu.currentMenu == MenuType.ImageTracking)
            {
                m_ImageTrackingMenu.SetActive(true);
                m_AllMenu.SetActive(false);
            }
            else if(ActiveMenu.currentMenu == MenuType.PlaneDetection)
            {
                m_PlaneDetectionMenu.SetActive(true);
                m_AllMenu.SetActive(false);
            }
            else if(ActiveMenu.currentMenu == MenuType.BodyTracking)
            {
                m_BodyTrackingMenu.SetActive(true);
                m_AllMenu.SetActive(false);
            }
            else if(ActiveMenu.currentMenu == MenuType.Meshing)
            {
                m_MeshingMenu.SetActive(true);
                m_AllMenu.SetActive(false);
            }
            else if(ActiveMenu.currentMenu == MenuType.Depth)
            {
                m_DepthMenu.SetActive(true);
                m_AllMenu.SetActive(false);
            }
            else if(ActiveMenu.currentMenu == MenuType.LightEstimation)
            {
                m_LightEstimationMenu.SetActive(true);
                m_AllMenu.SetActive(false);
            }
            ScrollToStartPosition();
        }

        static void LoadScene(string sceneName)
        {
            LoaderUtility.Initialize();
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        public void SimpleARButtonPressed()
        {
            LoadScene("SimpleAR");
        }

        public void ImageTrackableButtonPressed()
        {
            ActiveMenu.currentMenu = MenuType.ImageTracking;
            m_ImageTrackingMenu.SetActive(true);
            m_AllMenu.SetActive(false);
            ScrollToStartPosition();
        }

        public void BasicImageTrackingButtonPressed()
        {
            LoadScene("BasicImageTracking");
        }

        public void MultiImagesTrackingButtonPressed()
        {
            LoadScene("ImageTrackingWithMultiplePrefabs");
        }

        public void AnchorsButtonPressed()
        {
            LoadScene("Anchors");
        }

        public void ARCollaborationDataButtonPressed()
        {
            LoadScene("ARCollaborationDataExample");
        }

        public void ARKitCoachingOverlayButtonPressed()
        {
            LoadScene("ARKitCoachingOverlay");
        }

        public void ARWorldMapButtonPressed()
        {
            LoadScene("ARWorldMap");
        }

        public void ARKitGeoAnchorsButtonPressed()
        {
            LoadScene("ARKitGeoAnchors");
        }

        public void CpuImagesButtonPressed()
        {
            LoadScene("CpuImages");
        }

        public void CheckSupportButtonPressed()
        {
            LoadScene("Check Support");
        }

        public void EnvironmentProbesButtonPressed()
        {
            LoadScene("EnvironmentProbes");
        }

        public void ObjectTrackingButtonPressed()
        {
            LoadScene("ObjectTracking");
        }

        public void PlaneOcclusionButtonPressed()
        {
            LoadScene("PlaneOcclusion");
        }

        public void PointCloudButtonPressed()
        {
            LoadScene("AllPointCloudPoints");
        }

        public void ScaleButtonPressed()
        {
            LoadScene("Scale");
        }

        public void ConfigChooserButtonPressed()
        {
            LoadScene("ConfigurationChooser");
        }

        public void FaceTrackingMenuButtonPressed()
        {
            ActiveMenu.currentMenu = MenuType.FaceTracking;
            m_FaceTrackingMenu.SetActive(true);
            m_AllMenu.SetActive(false);
            ScrollToStartPosition();
        }

        public void ARCoreFaceRegionsButtonPressed()
        {
            LoadScene("ARCoreFaceRegions");
        }

        public void ARKitFaceBlendShapesButtonPressed()
        {
            LoadScene("ARKitFaceBlendShapes");
        }

        public void EyeLasersButtonPressed()
        {
            LoadScene("EyeLasers");
        }

        public void EyePosesButtonPressed()
        {
            LoadScene("EyePoses");
        }

        public void FaceMeshButtonPressed()
        {
            LoadScene("FaceMesh");
        }

        public void FacePoseButtonPressed()
        {
            LoadScene("FacePose");
        }

        public void FixationPointButtonPressed()
        {
            LoadScene("FixationPoint");
        }

        public void RearCameraWithFrontCameraFaceMeshButtonPressed()
        {
            LoadScene("WorldCameraWithUserFacingFaceTracking");
        }

        public void BodyTrackingMenuButtonPressed()
        {
            ActiveMenu.currentMenu = MenuType.BodyTracking;
            m_BodyTrackingMenu.SetActive(true);
            m_AllMenu.SetActive(false);
            ScrollToStartPosition();
        }

        public void BodyTracking2DButtonPressed()
        {
            LoadScene("HumanBodyTracking2D");
        }

        public void BodyTracking3DButtonPressed()
        {
            LoadScene("HumanBodyTracking3D");
        }

        public void LightEstimationMenuButtonPressed()
        {
            ActiveMenu.currentMenu = MenuType.LightEstimation;
            m_LightEstimationMenu.SetActive(true);
            m_AllMenu.SetActive(false);
            ScrollToStartPosition();
        }

        public void BasicLightEstimationButtonPressed()
        {
            LoadScene("BasicLightEstimation");
        }

        public void HDRLightEstimationButtonPressed()
        {
            LoadScene("HDRLightEstimation");
        }

        public void PlaneDetectionMenuButtonPressed()
        {
            ActiveMenu.currentMenu = MenuType.PlaneDetection;
            m_PlaneDetectionMenu.SetActive(true);
            m_AllMenu.SetActive(false);
            ScrollToStartPosition();
        }

        public void FeatheredPlanesButtonPressed()
        {
            LoadScene("FeatheredPlanes");
        }

        public void PlaneClassificationButtonPressed()
        {
            LoadScene("PlaneClassification");
        }

        public void TogglePlaneDetectionButtonPressed()
        {
            LoadScene("TogglePlaneDetection");
        }

        public void BackButtonPressed()
        {
            ActiveMenu.currentMenu = MenuType.Main;
            m_ImageTrackingMenu.SetActive(false);
            m_FaceTrackingMenu.SetActive(false);
            m_PlaneDetectionMenu.SetActive(false);
            m_BodyTrackingMenu.SetActive(false);
            m_MeshingMenu.SetActive(false);
            m_DepthMenu.SetActive(false);
            m_LightEstimationMenu.SetActive(false);
            m_AllMenu.SetActive(true);
            ScrollToStartPosition();
        }

        public void MeshingMenuButtonPressed()
        {
            ActiveMenu.currentMenu = MenuType.Meshing;
            m_MeshingMenu.SetActive(true);
            m_AllMenu.SetActive(false);
            ScrollToStartPosition();
        }

        public void DepthMenuButtonPressed()
        {
            ActiveMenu.currentMenu = MenuType.Depth;
            m_DepthMenu.SetActive(true);
            m_AllMenu.SetActive(false);
            ScrollToStartPosition();
        }

        public void ClassificationMeshesButtonPressed()
        {
            LoadScene("ClassificationMeshes");
        }

        public void NormalMeshesButtonPressed()
        {
            LoadScene("NormalMeshes");
        }

        public void OcclusionMeshesButtonPressed()
        {
            LoadScene("OcclusionMeshes");
        }

        public void InteractionButtonPressed()
        {
            LoadScene("Interaction");
        }

        public void SimpleOcclusionButtonPressed()
        {
            LoadScene("SimpleOcclusion");
        }

        public void DepthImagesButtonPressed()
        {
            LoadScene("DepthImages");
        }

        public void InputSystemButtonPressed()
        {
            LoadScene("InputSystem");
        }

        public void CameraGrainButtonPressed()
        {
            LoadScene("CameraGrain");
        }

        public void ThermalStateButtonPressed()
        {
            LoadScene("ThermalState");
        }

        void ScrollToStartPosition()
        {
            m_HorizontalScrollBar.value = 0;
            m_VerticalScrollBar.value = 1;
        }
    }
}
