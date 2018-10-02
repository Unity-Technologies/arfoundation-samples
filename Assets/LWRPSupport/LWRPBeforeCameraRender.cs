using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.LightweightPipeline;

namespace UnityEngine.XR.ARFoundation
{
	public class LWRPBeforeCameraRender : MonoBehaviour, LightweightPipeline.IBeforeCameraRender
	{
		const string k_ARBlitTag = "ARBackground Blit Pass";

		public Material blitMaterial { get; set; }

		public void ExecuteBeforeCameraRender(ScriptableRenderContext context, Camera currentCamera, LightweightPipeline.PipelineSettings pipelineSettings,
			ScriptableRenderer currentRenderer)
		{
			if (blitMaterial == null) 
				return;
			
			var cmd = CommandBufferPool.Get(k_ARBlitTag);
			
			cmd.Blit(null, BuiltinRenderTextureType.CameraTarget, blitMaterial);

			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
		}
	}
}
