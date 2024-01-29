using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Used for any UI that is intended to be used on an HMDs. Can convert from screen space
    /// to world space UI and place world space UI in front of the camera.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class HMDCanvasController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("The XR camera.")]
        Camera m_Camera;

        [SerializeField, Tooltip("The background image of the UI to optionally display for HMD.")]
        GameObject m_CanvasBackground;

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

        Vector2 m_HMDCanvasDimensionsScaled;
        const float k_CanvasWorldSpaceScale = 0.001f;

        void Reset()
        {
            m_Camera = Camera.main;
            m_Canvas = GetComponent<Canvas>();
        }

        void Start()
        {
            if (m_Canvas == null)
                m_Canvas = GetComponent<Canvas>();

            if (!m_EnableInEditor && !MenuLoader.IsHmdDevice())
                return;

            // Since the canvas is scaled to preserve UI elements size on the canvas,
            // the values that get applied need to be updated by the scale the canvas will be set to
            m_HMDCanvasDimensionsScaled = m_HMDCanvasDimensionsInMeters / k_CanvasWorldSpaceScale;

            StartCoroutine(InitializeNextFrame());
            IEnumerator InitializeNextFrame()
            {
                yield return null;
                SetToWorldSpace();
                PlaceInFrontOfCamera();
            }
        }

        void SetToWorldSpace()
        {
            if (m_Canvas.renderMode == RenderMode.WorldSpace)
                return;

            m_Canvas.renderMode = RenderMode.WorldSpace;
            m_Canvas.worldCamera = m_Camera;
            m_Canvas.transform.localScale = Vector3.one * k_CanvasWorldSpaceScale;

            if (!m_Canvas.TryGetComponent(out TrackedDeviceGraphicRaycaster _))
                m_Canvas.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();

            if (m_CanvasBackground != null)
                m_CanvasBackground.SetActive(m_ShowBackgroundForHMD);

            m_Canvas.GetComponent<RectTransform>().sizeDelta = m_HMDCanvasDimensionsScaled;
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
    }
}
