using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

public class CheckAvailableFeatures : MonoBehaviour
{
    public Button simpleAR;
    public Button imageTracking;
    public Button anchors;
    public Button ARWorldMap;
    public Button cameraImage;
    public Button environmentProbes;
    public Button ARCollaborationData;
    public Button ARKitCoachingOverlay;
    public Button scale;
    public Button objectTracking;
    public Button planeOcclusion;
    public Button pointCloud;
    public Button faceTracking;
    public Button faceBlendShapes;
    public Button humanSegmentation;
    public Button lightEstimation;
    public Button planeDetection;
    public Button planeClassification;

#if UNITY_IOS
    OperatingSystem os = Environment.OSVersion;
#endif
    // Start is called before the first frame update
    void Start()
    {
        List<XRPlaneSubsystemDescriptor> planeDescriptors = new List<XRPlaneSubsystemDescriptor>();
        SubsystemManager.GetSubsystemDescriptors<XRPlaneSubsystemDescriptor>(planeDescriptors);

        List<XRRaycastSubsystemDescriptor> rayCastDescriptors = new List<XRRaycastSubsystemDescriptor>();
        SubsystemManager.GetSubsystemDescriptors<XRRaycastSubsystemDescriptor>(rayCastDescriptors);

        List<XRFaceSubsystemDescriptor> faceDescriptors = new List<XRFaceSubsystemDescriptor>();
        SubsystemManager.GetSubsystemDescriptors<XRFaceSubsystemDescriptor>(faceDescriptors);

        List<XRImageTrackingSubsystemDescriptor> imageDescriptors = new List<XRImageTrackingSubsystemDescriptor>();
        SubsystemManager.GetSubsystemDescriptors<XRImageTrackingSubsystemDescriptor>(imageDescriptors);

        List<XREnvironmentProbeSubsystemDescriptor> envDescriptors = new List<XREnvironmentProbeSubsystemDescriptor>();
        SubsystemManager.GetSubsystemDescriptors<XREnvironmentProbeSubsystemDescriptor>(envDescriptors);

        List<XRAnchorSubsystemDescriptor> anchorDescriptors = new List<XRAnchorSubsystemDescriptor>();
        SubsystemManager.GetSubsystemDescriptors<XRAnchorSubsystemDescriptor>(anchorDescriptors);

        List<XRObjectTrackingSubsystemDescriptor> objectDescriptors = new List<XRObjectTrackingSubsystemDescriptor>();
        SubsystemManager.GetSubsystemDescriptors<XRObjectTrackingSubsystemDescriptor>(objectDescriptors);

        List<XRParticipantSubsystemDescriptor> participantDescriptors = new List<XRParticipantSubsystemDescriptor>();
        SubsystemManager.GetSubsystemDescriptors<XRParticipantSubsystemDescriptor>(participantDescriptors);

        List<XRDepthSubsystemDescriptor> depthDescriptors = new List<XRDepthSubsystemDescriptor>();
        SubsystemManager.GetSubsystemDescriptors<XRDepthSubsystemDescriptor>(depthDescriptors);

        List<XROcclusionSubsystemDescriptor> occlusionDescriptors = new List<XROcclusionSubsystemDescriptor>();
        SubsystemManager.GetSubsystemDescriptors<XROcclusionSubsystemDescriptor>(occlusionDescriptors);

        List<XRCameraSubsystemDescriptor> cameraDescriptors = new List<XRCameraSubsystemDescriptor>();
        SubsystemManager.GetSubsystemDescriptors<XRCameraSubsystemDescriptor>(cameraDescriptors);

        if(planeDescriptors.Count > 0 && rayCastDescriptors.Count > 0)
        {
            simpleAR.interactable = true;
            scale.interactable = true;
        }  
        if(faceDescriptors.Count > 0)
        {
            faceTracking.interactable = true;
#if UNITY_IOS
            faceBlendShapes.interactable = true;
#endif

        }
        if(occlusionDescriptors.Count > 0)
        {
            foreach(XROcclusionSubsystemDescriptor occlusionDescriptor in occlusionDescriptors)
            {
                if(occlusionDescriptor.supportsHumanSegmentationDepthImage && occlusionDescriptor.supportsHumanSegmentationStencilImage)
                {
                    humanSegmentation.interactable = true;
                    break;
                }
            }
        }
        if(cameraDescriptors.Count > 0)
        {
            foreach(var cameraDescriptor in cameraDescriptors)
            {
                //TODO: Change face blend to blendshapes (Only for ios)
                if((cameraDescriptor.supportsAverageBrightness || cameraDescriptor.supportsAverageIntensityInLumens) && 
                    cameraDescriptor.supportsAverageColorTemperature && cameraDescriptor.supportsCameraConfigurations && 
                    cameraDescriptor.supportsCameraImage)
                {
                    lightEstimation.interactable = true;
                }
                  
            }
        }
        if(imageDescriptors.Count > 0)
        {
            imageTracking.interactable = true;
        }
        if(envDescriptors.Count > 0)
        {
            environmentProbes.interactable = true;
        }

        if(planeDescriptors.Count > 0){
            planeDetection.interactable = true;
            foreach(XRPlaneSubsystemDescriptor planeDescriptor in planeDescriptors)
            {
                if(planeDescriptor.supportsClassification)
                {
                    planeClassification.interactable = true;
                    break;
                }
            } 
        }
        if(anchorDescriptors.Count > 0)
        {
            anchors.interactable = true;
        }
        if(objectDescriptors.Count > 0)
        {
            objectTracking.interactable = true;
        }
        if(cameraDescriptors.Count > 0)
        {
            foreach(XRCameraSubsystemDescriptor cameraDescriptor in cameraDescriptors)
            {
                if(cameraDescriptor.supportsCameraImage)
                {
                    cameraImage.interactable = true;
                    break;
                }
            }

        }
#if UNITY_IOS
        if(os.Version.Major >= 12)
        {
            ARWorldMap.interactable = true;
        }
#endif
        if(planeDescriptors.Count > 0 && rayCastDescriptors.Count > 0 && participantDescriptors.Count > 0)
        {
            ARCollaborationData.interactable = true;
        }

        if(depthDescriptors.Count > 0)
        {
            pointCloud.interactable = true;
        }

        if(planeDescriptors.Count > 0)
        {
            planeOcclusion.interactable  = true;
        }    
#if UNITY_IOS
        if(os.Version.Major >= 13)
        {
            ARKitCoachingOverlay.interactable = true;
        }      
#endif
    }

}
