using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Used for any UI that is intended to be used on an HMD. Converts from screen space
    /// to world space UI and places world space UI in front of the camera. Also adds a
    /// UI Window Handle to allow UI to be moved around.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class HMDCanvasController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("The XR camera.")]
        Camera m_Camera;

        [SerializeField, Tooltip("The background image of the UI to optionally display for HMD.")]
        GameObject m_CanvasBackground;

        [SerializeField]
        BoxCollider m_UIBoxCollider;

        [SerializeField]
        GameObject m_UIWindowHandlePrefab;

        [Header("Settings")]
        [SerializeField, Tooltip("The dimensions the canvas well be set to for HMD in world space in meters.")]
        Vector2 m_HMDCanvasDimensionsInMeters = new(1.15f, 0.85f);

        [SerializeField, Tooltip("Distance in front of the camera for the UI to be placed in world space."), Range(.3f, 3)]
        float m_DistanceFromCamera = 1.5f;

        [SerializeField, Tooltip("Displays the background image when running on HMD.")]
        bool m_ShowBackgroundForHMD = true;

        [SerializeField, Tooltip("Will convert the canvas to world space in the editor for testing how the UI will look on HMD.")]
        bool m_EnableInEditor;

        [SerializeField, HideInInspector]
        Canvas m_Canvas;

        [SerializeField, HideInInspector]
        RectTransform m_CanvasRT;

        // The canvas will get set to world space if either of these two are true.
        // We can't check the canvas directly because it doesn't get set until the frame after start.
        public bool isWorldSpaceCanvas => m_EnableInEditor || MenuLoader.IsHmdDevice();

        // The dimensions of the canvas in pixels
        public Vector2 canvasDimensions => m_CanvasRT.sizeDelta;

        Vector2 m_HMDCanvasTargetSize;
        const float k_CanvasWorldSpaceScale = 0.001f;
        const float k_UIHandleSpacing = 0.05f;


        void Reset()
        {
            m_Camera = Camera.main;
            m_Canvas = GetComponent<Canvas>();
            m_CanvasRT = m_Canvas.GetComponent<RectTransform>();
        }

        async void Start()
        {
            if (m_Canvas == null)
                m_Canvas = GetComponent<Canvas>();

            if (!isWorldSpaceCanvas)
                return;

            if (m_CanvasRT == null)
                m_CanvasRT = m_Canvas.GetComponent<RectTransform>();

            m_HMDCanvasTargetSize = m_HMDCanvasDimensionsInMeters / k_CanvasWorldSpaceScale;

            // Wait until next frame when the transform values are updated for the UI since
            // they get updated in the frame some point after Start
            await Awaitable.NextFrameAsync();
            SetToWorldSpace();
            PlaceInFrontOfCamera();
            AddUIWindowHandle();
        }

        void SetToWorldSpace()
        {
            if (m_Canvas.renderMode == RenderMode.WorldSpace)
                return;

            m_Canvas.renderMode = RenderMode.WorldSpace;
            m_Canvas.worldCamera = m_Camera;

            m_Canvas.transform.localScale = Vector3.one * k_CanvasWorldSpaceScale;
            m_Canvas.GetComponent<RectTransform>().sizeDelta = m_HMDCanvasTargetSize;
            m_UIBoxCollider.size = m_HMDCanvasTargetSize;

            if (!m_Canvas.TryGetComponent(out TrackedDeviceGraphicRaycaster _))
                m_Canvas.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();

            if (m_CanvasBackground != null)
                m_CanvasBackground.SetActive(m_ShowBackgroundForHMD);
        }

        void PlaceInFrontOfCamera()
        {
            var cameraTransform = m_Camera.transform;
            var cameraPosition = cameraTransform.position;
            var cameraForward = cameraTransform.forward;

            // make the camera forward vector parallel to the xz plane
            cameraForward.y = 0;
            cameraForward.Normalize();

            transform.position = cameraPosition + cameraForward * m_DistanceFromCamera;
            var lookAtPosition = transform.position + (transform.position - cameraPosition);
            transform.LookAt(lookAtPosition);
        }

        void AddUIWindowHandle()
        {
            var handleVerticalOffset = -m_CanvasRT.sizeDelta.y * transform.localScale.x * 0.5f - k_UIHandleSpacing;
            var handlePosition = transform.position + (Vector3.up * handleVerticalOffset);
            var handle = Instantiate(m_UIWindowHandlePrefab);
            handle.transform.position = handlePosition;
            handle.transform.rotation = transform.rotation;
            transform.SetParent(handle.transform);
        }
    }
}
