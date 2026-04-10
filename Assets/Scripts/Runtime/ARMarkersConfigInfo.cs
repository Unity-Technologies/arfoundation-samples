using System;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class ARMarkersConfigInfo : MonoBehaviour
    {
        const float k_TimeoutDuration = 6;

        [SerializeField]
        ARMarkerManager m_MarkerManager;

        [Header("Marker Info UI")]
        [SerializeField]
        ARMarkerInfo m_QRCodeInfo;

        [SerializeField]
        ARMarkerInfo m_MicroQRCodeInfo;

        [SerializeField]
        ARMarkerInfo m_ArUcoMarkerInfo;

        [SerializeField]
        ARMarkerInfo m_AprilTagInfo;

        async void Start()
        {
            try
            {
                var elapsedTime = 0f;
                while (m_MarkerManager.subsystem == null && elapsedTime < k_TimeoutDuration)
                {
                    elapsedTime += Time.deltaTime;
                    await Awaitable.NextFrameAsync();
                }
            }
            catch (OperationCanceledException)
            {
                // Graceful exit
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            if (m_MarkerManager.subsystem != null)
                OnSubsystemCreated();
        }

        void OnSubsystemCreated()
        {
            var result = m_MarkerManager.subsystem.subsystemDescriptor.supportedMarkerTypes;
            if (result.status.IsError())
                return;

            var supportedMarkerTypes = result.value;
            foreach (var markerType in supportedMarkerTypes)
            {
                switch (markerType)
                {
                    case XRMarkerType.QRCode:
                        SetMarkerInfo(m_QRCodeInfo);
                        break;
                    case XRMarkerType.MicroQRCode:
                        SetMarkerInfo(m_MicroQRCodeInfo);
                        break;
                    case XRMarkerType.ArUco:
                        SetMarkerInfo(m_ArUcoMarkerInfo);
                        break;
                    case XRMarkerType.AprilTag:
                        SetMarkerInfo(m_AprilTagInfo);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Unsupported marker type: {markerType}");
                }
            }
        }

        static void SetMarkerInfo(ARMarkerInfo markerInfo)
        {
            markerInfo.Set();
        }
    }
}
