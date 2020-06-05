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
        GameObject m_PlaneDetectionMenu;
        public GameObject planeDetectionMenu
        {
            get { return m_PlaneDetectionMenu; }
            set { m_PlaneDetectionMenu = value; }
        }

        [SerializeField]
        GameObject m_MeshingMenu;
        public GameObject meshingMenu
        {
            get { return m_MeshingMenu; }
            set { m_MeshingMenu = value; }
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
            else if(ActiveMenu.currentMenu == MenuType.Meshing)
            {
                m_MeshingMenu.SetActive(true);
                m_AllMenu.SetActive(false);
            }
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
            LoadScene("ImageTracking");
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

        public void SampleUXButtonPressed()
        {
            LoadScene("SampleUXScene");
        }

        public void FaceTrackingMenuButtonPressed()
        {
            ActiveMenu.currentMenu = MenuType.FaceTracking;
            m_FaceTrackingMenu.SetActive(true);
            m_AllMenu.SetActive(false);
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

        public void HumanSegmentationMenuButtonPressed()
        {
            ActiveMenu.currentMenu = MenuType.HumanSegmentation;
            m_HumanSegmentationMenu.SetActive(true);
            m_AllMenu.SetActive(false);
        }

        public void HumanSegmentation2DButtonPressed()
        {
            LoadScene("HumanBodyTracking2D");
        }

        public void HumanSegmentation3DButtonPressed()
        {
            LoadScene("HumanBodyTracking3D");
        }

        public void HumanSegmentationImagesButtonPressed()
        {
            LoadScene("HumanSegmentationImages");
        }

        public void LightEstimationButtonPressed()
        {
            LoadScene("LightEstimation");
        }

        public void PlaneDetectionMenuButtonPressed()
        {
            ActiveMenu.currentMenu = MenuType.PlaneDetection;
            m_PlaneDetectionMenu.SetActive(true);
            m_AllMenu.SetActive(false);
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
            m_FaceTrackingMenu.SetActive(false);
            m_PlaneDetectionMenu.SetActive(false);
            m_HumanSegmentationMenu.SetActive(false);
            m_MeshingMenu.SetActive(false);
            m_AllMenu.SetActive(true);
        }

        public void MeshingMenuButtonPressed()
        {
            ActiveMenu.currentMenu = MenuType.Meshing;
            m_MeshingMenu.SetActive(true);
            m_AllMenu.SetActive(false);
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
    }
}
