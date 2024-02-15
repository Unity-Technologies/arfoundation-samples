using System.Collections.Generic;
using UnityEngine.Rendering;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Hacky workaround for bug where screen space UI is not rendered in scenes with
    /// no camera. Disables screen space UI Overlay for current render pipeline, forcing 
    /// rendering at the end on native side so screen space UI canvas doesn't appear 
    /// on output test image.
    /// </summary>
    public class ScreenSpaceUIWorkaround : MonoBehaviour
    {
        void Start()
        {
            RenderPipelineManager.beginContextRendering += OnBeginContextRendering;
        }

        void OnBeginContextRendering(ScriptableRenderContext context, List<Camera> cameras)
        {
            SupportedRenderingFeatures.active.rendersUIOverlay = false;
        }

        void OnDestroy()
        {
            RenderPipelineManager.beginContextRendering -= OnBeginContextRendering;
        }
    }
}