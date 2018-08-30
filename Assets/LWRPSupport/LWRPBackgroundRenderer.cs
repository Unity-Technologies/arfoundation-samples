
namespace UnityEngine.XR.ARFoundation.LWRPSupport
{

    public class LWRPBackgroundRenderer : ARFoundationBackgroundRenderer
    {
        CameraClearFlags _savedClearFlags;
        LWRPBeforeCameraRender _lwrpBeforeCameraRender;

        protected override bool EnableARBackgroundRendering()
        {
            if (m_BackgroundMaterial == null)
                return false;

            camera = m_Camera ? m_Camera : Camera.main;

            if (camera == null)
                return false;

            // Clear flags
            _savedClearFlags = camera.clearFlags;
            camera.clearFlags = CameraClearFlags.Depth;

            if (_lwrpBeforeCameraRender == null)
            {
                _lwrpBeforeCameraRender = camera.gameObject.GetComponent<LWRPBeforeCameraRender>();
            }
            
            _lwrpBeforeCameraRender.SetupBlitMaterial(m_BackgroundMaterial);

            return true;

        }


        protected override void DisableARBackgroundRendering()
        {
            if (m_BackgroundMaterial == null)
                return;

            camera = m_Camera ? m_Camera : Camera.main;
            if (camera == null)
                return;
            camera.clearFlags = _savedClearFlags;

            if (_lwrpBeforeCameraRender != null)
            {
                _lwrpBeforeCameraRender.SetupBlitMaterial(null);
                _lwrpBeforeCameraRender = null;
            }

        }

    }

}
