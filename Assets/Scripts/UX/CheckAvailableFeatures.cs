using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif
using UnityEngine.XR.Management;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class CheckAvailableFeatures : MonoBehaviour
    {
        [SerializeField]
        Button m_SimpleAR;
        public Button simpleAR
        {
            get => m_SimpleAR;
            set => m_SimpleAR = value;
        }

        [SerializeField]
        Button m_ImageTracking;
        public Button imageTracking
        {
            get => m_ImageTracking;
            set => m_ImageTracking = value;
        }

        [SerializeField]
        Button m_Anchors;
        public Button anchors
        {
            get => m_Anchors;
            set => m_Anchors = value;
        }

        [SerializeField]
        Button m_ARWorldMap;
        public Button ARWorldMap
        {
            get => m_ARWorldMap;
            set => m_ARWorldMap = value;
        }

        [SerializeField]
        Button m_ARKitGeoAnchors;
        public Button ARKitGeoAnchors
        {
            get => m_ARKitGeoAnchors;
            set => m_ARKitGeoAnchors = value;
        }

        [SerializeField]
        Button m_CpuImages;
        public Button cpuImages
        {
            get => m_CpuImages;
            set => m_CpuImages = value;
        }

        [SerializeField]
        Button m_EnvironmentProbes;
        public Button environmentProbes
        {
            get => m_EnvironmentProbes;
            set => m_EnvironmentProbes = value;
        }

        [SerializeField]
        Button m_ARCollaborationData;
        public Button ARCollaborationData
        {
            get => m_ARCollaborationData;
            set => m_ARCollaborationData = value;
        }

        [SerializeField]
        Button m_ARKitCoachingOverlay;
        public Button ARKitCoachingOverlay
        {
            get => m_ARKitCoachingOverlay;
            set => m_ARKitCoachingOverlay = value;
        }

        [SerializeField]
        Button m_Scale;
        public Button scale
        {
            get => m_Scale;
            set => m_Scale = value;
        }

        [SerializeField]
        Button m_ObjectTracking;
        public Button objectTracking
        {
            get => m_ObjectTracking;
            set => m_ObjectTracking = value;
        }

        [SerializeField]
        Button m_PlaneOcclusion;
        public Button planeOcclusion
        {
            get => m_PlaneOcclusion;
            set => m_PlaneOcclusion = value;
        }

        [SerializeField]
        Button m_PointCloud;
        public Button pointCloud
        {
            get => m_PointCloud;
            set => m_PointCloud = value;
        }

        [SerializeField]
        Button m_FaceTracking;
        public Button faceTracking
        {
            get => m_FaceTracking;
            set => m_FaceTracking = value;
        }

        [SerializeField]
        Button m_FaceBlendShapes;
        public Button faceBlendShapes
        {
            get => m_FaceBlendShapes;
            set => m_FaceBlendShapes = value;
        }

        [SerializeField]
        Button m_FaceRegions;
        public Button faceRegions
        {
            get => m_FaceRegions;
            set => m_FaceRegions = value;
        }

        [SerializeField]
        Button m_BodyTracking;
        public Button bodyTracking
        {
            get => m_BodyTracking;
            set => m_BodyTracking = value;
        }

        [SerializeField]
        Button m_LightEstimation;
        public Button lightEstimation
        {
            get => m_LightEstimation;
            set => m_LightEstimation = value;
        }

        [SerializeField]
        Button m_BasicLightEstimation;
        public Button basicLightEstimation
        {
            get => m_BasicLightEstimation;
            set => m_BasicLightEstimation = value;
        }

        [SerializeField]
        Button m_HDRLightEstimation;
        public Button HDRLightEstimation
        {
            get => m_HDRLightEstimation;
            set => m_HDRLightEstimation = value;
        }

        [SerializeField]
        Button m_PlaneDetection;
        public Button planeDetection
        {
            get => m_PlaneDetection;
            set => m_PlaneDetection = value;
        }

        [SerializeField]
        Button m_PlaneClassification;
        public Button planeClassification
        {
            get => m_PlaneClassification;
            set => m_PlaneClassification = value;
        }

        [SerializeField]
        Button m_Meshing;
        public Button meshing
        {
            get => m_Meshing;
            set => m_Meshing = value;
        }

        [SerializeField]
        Button m_Interaction;
        public Button interaction
        {
            get => m_Interaction;
            set => m_Interaction = value;
        }

        [SerializeField]
        Button m_FixationPoint;
        public Button fixationPoint
        {
            get => m_FixationPoint;
            set => m_FixationPoint = value;
        }

        [SerializeField]
        Button m_EyePoses;
        public Button eyePoses
        {
            get => m_EyePoses;
            set => m_EyePoses = value;
        }

        [SerializeField]
        Button m_EyeLasers;
        public Button eyeLasers
        {
            get => m_EyeLasers;
            set => m_EyeLasers = value;
        }

        [SerializeField]
        Button m_CheckSupport;
        public Button checkSupport
        {
            get => m_CheckSupport;
            set => m_CheckSupport = value;
        }

        [SerializeField]
        Button m_Depth;
        public Button depth
        {
            get => m_Depth;
            set => m_Depth = value;
        }

        [SerializeField]
        Button m_ConfigChooser;
        public Button configChooser
        {
            get => m_ConfigChooser;
            set => m_ConfigChooser = value;
        }

        [SerializeField]
        Button m_InputSystem;
        public Button inputSystem
        {
            get => m_InputSystem;
            set => m_InputSystem = value;
        }

        [SerializeField]
        Button m_CameraGrain;
        public Button cameraGrain
        {
            get => m_CameraGrain;
            set => m_CameraGrain = value;
        }

        [SerializeField]
        Button m_ThermalStateButton;
        public Button thermalStateButton
        {
            get => m_ThermalStateButton;
            set => m_ThermalStateButton = value;
        }

        void Start()
        {
            var planeDescriptors = new List<XRPlaneSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(planeDescriptors);

            var rayCastDescriptors = new List<XRRaycastSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(rayCastDescriptors);

            var faceDescriptors = new List<XRFaceSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(faceDescriptors);

            var imageDescriptors = new List<XRImageTrackingSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(imageDescriptors);

            var envDescriptors = new List<XREnvironmentProbeSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(envDescriptors);

            var anchorDescriptors = new List<XRAnchorSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(anchorDescriptors);

            var objectDescriptors = new List<XRObjectTrackingSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(objectDescriptors);

            var participantDescriptors = new List<XRParticipantSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(participantDescriptors);

            var depthDescriptors = new List<XRDepthSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(depthDescriptors);

            var occlusionDescriptors = new List<XROcclusionSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(occlusionDescriptors);

            var cameraDescriptors = new List<XRCameraSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(cameraDescriptors);

            var sessionDescriptors = new List<XRSessionSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(sessionDescriptors);

            var bodyTrackingDescriptors = new List<XRHumanBodySubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(bodyTrackingDescriptors);

            if(planeDescriptors.Count > 0 && rayCastDescriptors.Count > 0)
            {
                m_SimpleAR.interactable = true;
                m_Scale.interactable = true;
                m_Interaction.interactable = true;
                m_CheckSupport.interactable = true;
                m_ConfigChooser.interactable = true;
                m_InputSystem.interactable = true;
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
                foreach(var faceDescriptor in faceDescriptors)
                {
                    if(faceDescriptor.supportsEyeTracking)
                    {
                        m_EyePoses.interactable = true;
                        m_FixationPoint.interactable = true;
                        m_EyeLasers.interactable = true;
                        break;
                    }
                }
            }

            if(occlusionDescriptors.Count > 0)
            {
                foreach(var occlusionDescriptor in occlusionDescriptors)
                {
#if UNITY_IOS
                    if(occlusionDescriptor.supportsEnvironmentDepthImage
                       || occlusionDescriptor.supportsHumanSegmentationDepthImage
                       || occlusionDescriptor.supportsHumanSegmentationStencilImage)
                    {
                        m_Depth.interactable = true;
                    }
#endif
#if UNITY_ANDROID
                    m_Depth.interactable = true;
#endif
                }
            }

            if(bodyTrackingDescriptors.Count > 0)
            {
                foreach(var bodyTrackingDescriptor in bodyTrackingDescriptors)
                {
                    if(bodyTrackingDescriptor.supportsHumanBody2D || bodyTrackingDescriptor.supportsHumanBody3D)
                    {
                        m_BodyTracking.interactable = true;
                    }
                }
            }

            if(cameraDescriptors.Count > 0)
            {
                m_LightEstimation.interactable = true;
                foreach(var cameraDescriptor in cameraDescriptors)
                {
                    if ((cameraDescriptor.supportsAverageBrightness || cameraDescriptor.supportsAverageIntensityInLumens) &&
                        (cameraDescriptor.supportsAverageColorTemperature || cameraDescriptor.supportsColorCorrection) && cameraDescriptor.supportsCameraConfigurations &&
                        cameraDescriptor.supportsCameraImage)
                    {
                        m_BasicLightEstimation.interactable = true;
                    }

                    if (cameraDescriptor.supportsFaceTrackingHDRLightEstimation || cameraDescriptor.supportsWorldTrackingHDRLightEstimation)
                    {
                        m_HDRLightEstimation.interactable = true;
                    }

#if UNITY_2020_2_OR_NEWER
                    m_CameraGrain.interactable = cameraDescriptor.supportsCameraGrain;
#endif
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

            if (sessionDescriptors.Count > 0 && EnableGeoAnchors.IsSupported)
            {
                m_ARKitGeoAnchors.interactable = true;
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

            var activeLoader = LoaderUtility.GetActiveLoader();
            if(activeLoader && activeLoader.GetLoadedSubsystem<XRMeshSubsystem>() != null)
            {
                m_Meshing.interactable = true;
            }

#if UNITY_IOS
            m_ThermalStateButton.interactable = true;
#endif // UNITY_IOS
        }
    }
}
