using System;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
#if UNITY_IOS && UNITY_XR_ARKIT_LOADER_ENABLED
using UnityEngine.XR.ARKit;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Sample component to demonstrate the use of <see cref="ARKitCameraSubsystem.TryAcquireHighResolutionCpuImage"/>.
    /// </summary>
    public class HighResolutionCpuImageSample : MonoBehaviour
    {
        const string k_BadDeviceErrorMsg = "This sample requires the ARKit platform and is not supported on this device";

        /// <summary>
        /// The desired texture format to display on screen.
        /// </summary>
        const TextureFormat k_DesiredTextureFormat = TextureFormat.RGBA32;

        [SerializeField]
        [Tooltip("The active AR Camera Manager.")]
        ARCameraManager m_CameraManager;

        [SerializeField]
        [Tooltip("A Raw Image used to render the high resolution capture.")]
        RawImage m_RawImage;

        [Header("Optional UI Elements")]
        [SerializeField]
        [Tooltip("The label used to display the high resolution capture size.")]
        TextMeshProUGUI m_ImageSizeValueText;

        [SerializeField]
        [Tooltip("The GameObject to enable to display the capture results.")]
        GameObject m_ResultsPanel;

        void OnEnable()
        {
            if (m_CameraManager == null)
#if UNITY_2023_1_OR_NEWER
                m_CameraManager = FindAnyObjectByType<ARCameraManager>();
#else
                m_CameraManager = FindObjectOfType<ARCameraManager>();
#endif

            if (m_CameraManager == null || m_RawImage == null)
            {
                Debug.LogException(new InvalidOperationException(
                    $"Serialized fields of {nameof(HighResolutionCpuImageSample)} component on {name} are not initialized."), this);
            }
        }

        /// <summary>
        /// Attempts an asynchronous high resolution frame capture. If successful, renders the captured frame to the screen.
        /// </summary>
        public void CaptureHighResolutionCpuImage()
        {
            if (Application.isEditor)
            {
                Debug.LogError("This sample requires the ARKit platform and is not supported in the Editor");
                return;
            }

#if !UNITY_IOS
            Debug.LogError(k_BadDeviceErrorMsg);
            return;
#elif !UNITY_XR_ARKIT_LOADER_ENABLED
            Debug.LogError("This sample requires the ARKit platform and is not supported in the current XRLoader configuration.");
            return;
#else
            StartCoroutine(CaptureHighResolutionCpuImageCoroutine());
#endif
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local -- invalid suggestion will break conditional compilation
        IEnumerator CaptureHighResolutionCpuImageCoroutine()
        {
#if UNITY_IOS && UNITY_XR_ARKIT_LOADER_ENABLED
            if (m_CameraManager.subsystem is not ARKitCameraSubsystem subsystem)
            {
                Debug.LogError(k_BadDeviceErrorMsg);
                yield break;
            }

            // Yield return on the promise returned by the ARKitCameraSubsystem
            var promise = subsystem.TryAcquireHighResolutionCpuImage();
            yield return promise;

            // If the promise was not successful, check your Console logs for more information about the error.
            if (!promise.result.wasSuccessful)
                yield break;

            // If the promise was successful, handle the result.
            UpdateRawImageTexture(promise.result.highResolutionCpuImage);
#else
            throw new InvalidOperationException(k_BadDeviceErrorMsg);
#endif
        }

        // ReSharper disable once UnusedMember.Local
        void UpdateRawImageTexture(XRCpuImage cpuImage)
        {
            // If the Raw Image texture is not the same size as the CPU image, re-initialize the texture to match.
            var texture = m_RawImage.texture as Texture2D;
            if (texture == null || texture.width != cpuImage.width || texture.height != cpuImage.height)
            {
                texture = new Texture2D(cpuImage.width, cpuImage.height, k_DesiredTextureFormat, false);
                m_RawImage.texture = texture;

                // Scale the Raw Image rect so that the CPU image will fit on screen.
                var rt = m_RawImage.rectTransform;
                rt.SetWidth(600 * ((float)cpuImage.width / cpuImage.height));
            }

            // Render the CPU image to our Raw Image texture
            var conversionParams = new XRCpuImage.ConversionParams(cpuImage, k_DesiredTextureFormat, XRCpuImage.Transformation.MirrorY);
            var rawTextureData = texture.GetRawTextureData<byte>();
            try
            {
                // Convert the raw CPU image to a Unity-supported texture format and copy those pixels into our texture
                cpuImage.Convert(conversionParams, rawTextureData);
                texture.Apply();
            }
            finally
            {
                // Release the ARKit resources associated with the CPU image.
                // In this sample we no longer need the CPU image after we have copied its pixels into our texture.
                cpuImage.Dispose();
            }

            // Update any other UI elements
            if (m_ImageSizeValueText != null)
                m_ImageSizeValueText.text = $"{cpuImage.width} x {cpuImage.height}";
            
            if (m_ResultsPanel != null)
                m_ResultsPanel.SetActive(true);
        }
    }
}
