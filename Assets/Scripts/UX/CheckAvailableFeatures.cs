using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class CheckAvailableFeatures : MonoBehaviour
    {
        [SerializeField]
        Button m_SimpleAR;
        public Button simpleAR
        {
            get { return m_SimpleAR; }
            set { m_SimpleAR = value; }
        }

        [SerializeField]
        Button m_ImageTracking;
        public Button imageTracking
        {
            get { return m_ImageTracking; }
            set { m_ImageTracking = value; }
        }

        [SerializeField]
        Button m_Anchors;
        public Button anchors
        {
            get { return m_Anchors; }
            set { m_Anchors = value; }
        }

        [SerializeField]
        Button m_ARWorldMap;
        public Button ARWorldMap
        {
            get { return m_ARWorldMap; }
            set { m_ARWorldMap = value; }
        }

        [SerializeField]
        Button m_CpuImages;
        public Button cpuImages
        {
            get { return m_CpuImages; }
            set { m_CpuImages = value; }
        }

        [SerializeField]
        Button m_EnvironmentProbes;
        public Button environmentProbes
        {
            get { return m_EnvironmentProbes; }
            set { m_EnvironmentProbes = value; }
        }

        [SerializeField]
        Button m_ARCollaborationData;
        public Button ARCollaborationData
        {
            get { return m_ARCollaborationData; }
            set { m_ARCollaborationData = value; }
        }

        [SerializeField]
        Button m_ARKitCoachingOverlay;
        public Button ARKitCoachingOverlay
        {
            get { return m_ARKitCoachingOverlay; }
            set { m_ARKitCoachingOverlay = value; }
        }

        [SerializeField]
        Button m_Scale;
        public Button scale
        {
            get { return m_Scale; }
            set { m_Scale = value; }
        }

        [SerializeField]
        Button m_ObjectTracking;
        public Button objectTracking
        {
            get { return m_ObjectTracking; }
            set { m_ObjectTracking = value; }
        }

        [SerializeField]
        Button m_PlaneOcclusion;
        public Button planeOcclusion
        {
            get { return m_PlaneOcclusion; }
            set { m_PlaneOcclusion = value; }
        }

        [SerializeField]
        Button m_PointCloud;
        public Button pointCloud
        {
            get { return m_PointCloud; }
            set { m_PointCloud = value; }
        }

        [SerializeField]
        Button m_FaceTracking;
        public Button faceTracking
        {
            get { return m_FaceTracking; }
            set { m_FaceTracking = value; }
        }

        [SerializeField]
        Button m_FaceBlendShapes;
        public Button faceBlendShapes
        {
            get { return m_FaceBlendShapes; }
            set { m_FaceBlendShapes = value; }
        }


        [SerializeField]
        Button m_FaceRegions;
        public Button faceRegions
        {
            get { return m_FaceRegions; }
            set { m_FaceRegions = value; }
        }

        [SerializeField]
        Button m_HumanSegmentation;
        public Button humanSegmentation
        {
            get { return m_HumanSegmentation; }
            set { m_HumanSegmentation = value; }
        }

        [SerializeField]
        Button m_LightEstimation;
        public Button lightEstimation
        {
            get { return m_LightEstimation; }
            set { m_LightEstimation = value; }
        }

        [SerializeField]
        Button m_PlaneDetection;
        public Button planeDetection
        {
            get { return m_PlaneDetection; }
            set { m_PlaneDetection = value; }
        }

        [SerializeField]
        Button m_PlaneClassification;
        public Button planeClassification
        {
            get { return m_PlaneClassification; }
            set { m_PlaneClassification = value; }
        }

        [SerializeField]
        Button m_Meshing;
        public Button meshing
        {
            get { return m_Meshing; }
            set { m_Meshing = value; }
        }

        [SerializeField]
        Button m_Interaction;
        public Button interaction
        {
            get { return m_Interaction; }
            set { m_Interaction = value; }
        }

        // Start is called before the first frame update
        void Start()
        {
            var planeDescriptors = new List<XRPlaneSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors<XRPlaneSubsystemDescriptor>(planeDescriptors);

            var rayCastDescriptors = new List<XRRaycastSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors<XRRaycastSubsystemDescriptor>(rayCastDescriptors);

            var faceDescriptors = new List<XRFaceSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors<XRFaceSubsystemDescriptor>(faceDescriptors);

            var imageDescriptors = new List<XRImageTrackingSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors<XRImageTrackingSubsystemDescriptor>(imageDescriptors);

            var envDescriptors = new List<XREnvironmentProbeSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors<XREnvironmentProbeSubsystemDescriptor>(envDescriptors);

            var anchorDescriptors = new List<XRAnchorSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors<XRAnchorSubsystemDescriptor>(anchorDescriptors);

            var objectDescriptors = new List<XRObjectTrackingSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors<XRObjectTrackingSubsystemDescriptor>(objectDescriptors);

            var participantDescriptors = new List<XRParticipantSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors<XRParticipantSubsystemDescriptor>(participantDescriptors);

            var depthDescriptors = new List<XRDepthSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors<XRDepthSubsystemDescriptor>(depthDescriptors);

            var occlusionDescriptors = new List<XROcclusionSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors<XROcclusionSubsystemDescriptor>(occlusionDescriptors);

            var cameraDescriptors = new List<XRCameraSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors<XRCameraSubsystemDescriptor>(cameraDescriptors);

            var sessionDescriptors = new List<XRSessionSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors<XRSessionSubsystemDescriptor>(sessionDescriptors);

            var meshDescriptors = new List<XRMeshSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors<XRMeshSubsystemDescriptor>(meshDescriptors);

            if(planeDescriptors.Count > 0 && rayCastDescriptors.Count > 0)
            {
                m_SimpleAR.interactable = true;
                m_Scale.interactable = true;
                m_Interaction.interactable = true;
            }

            if(faceDescriptors.Count > 0)
            {
                m_FaceTracking.interactable = true;
#if UNITY_IOS
                m_FaceBlendShapes.interactable = true;
#endif
#if UNITY_ANDROID
                m_FaceRegions.interactable = true;
#endif
            }

            if(occlusionDescriptors.Count > 0)
            {
                foreach(XROcclusionSubsystemDescriptor occlusionDescriptor in occlusionDescriptors)
                {
                    if(occlusionDescriptor.supportsHumanSegmentationDepthImage && occlusionDescriptor.supportsHumanSegmentationStencilImage)
                    {
                        m_HumanSegmentation.interactable = true;
                        break;
                    }
                }
            }

            if(cameraDescriptors.Count > 0)
            {
                foreach(var cameraDescriptor in cameraDescriptors)
                {
                    if((cameraDescriptor.supportsAverageBrightness || cameraDescriptor.supportsAverageIntensityInLumens) &&
                        cameraDescriptor.supportsAverageColorTemperature && cameraDescriptor.supportsCameraConfigurations &&
                        cameraDescriptor.supportsCameraImage)
                    {
                        m_LightEstimation.interactable = true;
                    }

                }
            }

            if(imageDescriptors.Count > 0)
            {
                m_ImageTracking.interactable = true;
            }

            if(envDescriptors.Count > 0)
            {
                m_EnvironmentProbes.interactable = true;
            }

            if(planeDescriptors.Count > 0)
            {
                m_PlaneDetection.interactable = true;
                foreach(var planeDescriptor in planeDescriptors)
                {
                    if(planeDescriptor.supportsClassification)
                    {
                        m_PlaneClassification.interactable = true;
                        break;
                    }
                }
            }

            if(anchorDescriptors.Count > 0)
            {
                m_Anchors.interactable = true;
            }

            if(objectDescriptors.Count > 0)
            {
                m_ObjectTracking.interactable = true;
            }

            if(cameraDescriptors.Count > 0)
            {
                foreach(var cameraDescriptor in cameraDescriptors)
                {
                    if(cameraDescriptor.supportsCameraImage)
                    {
                        m_CpuImages.interactable = true;
                        break;
                    }
                }
            }

    #if UNITY_IOS
            if(sessionDescriptors.Count > 0 && ARKitSessionSubsystem.worldMapSupported)
            {
                m_ARWorldMap.interactable = true;
            }

            if(planeDescriptors.Count > 0 && rayCastDescriptors.Count > 0 && participantDescriptors.Count > 0 && ARKitSessionSubsystem.supportsCollaboration)
            {
                m_ARCollaborationData.interactable = true;
            }

            if(sessionDescriptors.Count > 0 && ARKitSessionSubsystem.coachingOverlaySupported)
            {
                m_ARKitCoachingOverlay.interactable = true;
            }
    #endif

            if(depthDescriptors.Count > 0)
            {
                m_PointCloud.interactable = true;
            }

            if(planeDescriptors.Count > 0)
            {
                m_PlaneOcclusion.interactable  = true;
            }

            if(meshDescriptors.Count > 0)
            {
                m_Meshing.interactable = true;
            }
        }
    }
}
