
namespace UnityEngine.XR.ARFoundation
{

    public class LWRPBackgroundRenderer : ARFoundationBackgroundRenderer
    {
        CameraClearFlags m_SavedClearFlags;
        LWRPBeforeCameraRender m_LWRPBeforeCameraRender;

        protected override bool EnableARBackgroundRendering()
        {
            if (m_BackgroundMaterial == null)
                return false;

            camera = m_Camera ? m_Camera : Camera.main;

            if (camera == null)
                return false;

            // Clear flags
            m_SavedClearFlags = camera.clearFlags;
            camera.clearFlags = CameraClearFlags.Depth;

            if (m_LWRPBeforeCameraRender == null)
            {
                m_LWRPBeforeCameraRender = camera.gameObject.GetComponent<LWRPBeforeCameraRender>();
            }
            
            m_LWRPBeforeCameraRender.blitMaterial = m_BackgroundMaterial;

            return true;

        }


        protected override void DisableARBackgroundRendering()
        {
            if (m_BackgroundMaterial == null)
                return;

            camera = m_Camera ? m_Camera : Camera.main;
            if (camera == null)
                return;
            camera.clearFlags = m_SavedClearFlags;

            if (m_LWRPBeforeCameraRender != null)
            {
                m_LWRPBeforeCameraRender.blitMaterial = null;
                m_LWRPBeforeCameraRender = null;
            }

        }

    }

}
