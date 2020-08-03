using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// On ARKit, HDR light estimation only works in face tracking mode.
    /// On ARCore, HDR light estimation only works when NOT in face tracking mode.
    /// This script enables face tracking on iOS and disables it otherwise.
    /// </summary>
    [RequireComponent(typeof(ARSessionOrigin))]
    [RequireComponent(typeof(ARFaceManager))]
    [RequireComponent(typeof(ARCameraManager))]
    public class FaceDirectionManager : MonoBehaviour
    {
        [SerializeField]
        GameObject m_WorldSpaceObject;

        public GameObject worldSpaceObject
        {
            get => m_WorldSpaceObject;
            set => m_WorldSpaceObject = value;
        }

        CameraFacingDirection updatedCameraFacingDirection;
        CameraFacingDirection originalCameraFacingDirection;
        void OnEnable()
        {
            originalCameraFacingDirection = GetComponentInChildren<ARCameraManager>().currentFacingDirection;
            updatedCameraFacingDirection = originalCameraFacingDirection;
        }

        void Update()
        {
            updatedCameraFacingDirection = GetComponentInChildren<ARCameraManager>().currentFacingDirection;
            if (updatedCameraFacingDirection != CameraFacingDirection.None && updatedCameraFacingDirection != originalCameraFacingDirection)
            {
                if (updatedCameraFacingDirection == CameraFacingDirection.User)
                {
                    originalCameraFacingDirection = updatedCameraFacingDirection;
                    GetComponent<ARFaceManager>().enabled = true;
                    worldSpaceObject.SetActive(false);
                }
                else if (updatedCameraFacingDirection == CameraFacingDirection.World)
                {
                    originalCameraFacingDirection = updatedCameraFacingDirection;
                    GetComponent<ARFaceManager>().enabled = false;
                    worldSpaceObject.SetActive(true);
                    Application.onBeforeRender += OnBeforeRender;
                }
            }
        }

        void OnDisable()
        {
            GetComponent<ARFaceManager>().enabled = false;
            Application.onBeforeRender -= OnBeforeRender;
        }

        void OnBeforeRender()
        {
            var camera = GetComponent<ARSessionOrigin>().camera;
            if (camera && worldSpaceObject)
            {
                worldSpaceObject.transform.position = camera.transform.position + camera.transform.forward;
            }
        }
    }
}