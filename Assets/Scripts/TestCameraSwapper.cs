using System;
using System.Text;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class TestCameraSwapper : MonoBehaviour
    {
        /// <summary>
        /// The camera manager for switching the camera direction.
        public ARCameraManager cameraManager
        {
            get => m_CameraManager;
            set => m_CameraManager = value;
        }

        [SerializeField]
        ARCameraManager m_CameraManager;

        public void OnSwapCameraButtonPress()
        {
            Debug.Assert(m_CameraManager != null, "camera manager cannot be null");
            CameraFacingDirection newFacingDirection;
            switch (m_CameraManager.requestedFacingDirection)
            {
                case CameraFacingDirection.World:
                    newFacingDirection = CameraFacingDirection.User;
                    break;
                case CameraFacingDirection.User:
                default:
                    newFacingDirection = CameraFacingDirection.World;
                    break;
            }

            Debug.Log($"Switching ARCameraManager.requestedFacingDirection from {m_CameraManager.requestedFacingDirection} to {newFacingDirection}");
            m_CameraManager.requestedFacingDirection = newFacingDirection;
        }
    }
}
