

using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.LightweightPipeline;

namespace UnityEngine.XR.ARFoundation
{

	public class LWRPBeforeCameraRender : MonoBehaviour, LightweightPipeline.IBeforeCameraRender
	{
		const string k_ARBlitTag = "ARBackground Blit Pass";

		Material BlitMaterial { get; set; }

		public void SetupBlitMaterial(Material material)
		{
			this.BlitMaterial = material;
		}

		public void ExecuteBeforeCameraRender(ScriptableRenderContext context, Camera camera, LightweightPipeline.PipelineSettings pipelineSettings,
			ScriptableRenderer renderer)
		{
			if (BlitMaterial == null) return;
			
			CommandBuffer cmd = CommandBufferPool.Get(k_ARBlitTag);
			
			cmd.Blit(null, BuiltinRenderTextureType.CameraTarget, BlitMaterial);

			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);

		}
	}
}
