using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.LWRP;
using UnityEngine.Experimental.Rendering.LWRP;

namespace UnityEngine.XR.ARFoundation
{
    public class LWRPBackgroundRenderPass : ScriptableRenderPass
    {
        Material m_BlitMaterial;
        RenderTargetIdentifier m_DepthIdentifier;
        RenderTargetIdentifier m_TargetIdentifier;
        RenderTextureDescriptor m_Descriptor;
        const string k_ARBlitTag = "ARBackground Blit Pass";

        /// <summary>
        /// This render pass will need to be setup with the following parameters to be able to execute when called to do so.
        /// In this case, this method is called from GetPassToEnqueue in IBeforeRender interface.
        /// </summary>
        /// <param name="mat"> Material to be used to clear the screen. </param>
        /// <param name="ident"> RenderTargetIdentifier for the color buffer. </param>
        /// <param name="depthIdent"> RenderTargetIdentifier for the depth buffer.</param>
        /// <param name="descript"> RenderTextureDescriptor for the target RenderTexture. </param>
        /// <returns> True if the material set is not null. </returns>
        public bool Setup(Material mat, RenderTargetIdentifier ident, RenderTargetIdentifier depthIdent, RenderTextureDescriptor descript)
        {
            m_BlitMaterial = mat;
            m_TargetIdentifier = ident;
            m_DepthIdentifier = depthIdent;
            m_Descriptor = descript;
            return m_BlitMaterial != null;
        }

        /// <summary>
        /// Custom rendering for clearing the background with the pass-thru video for AR happens here.  We use the parameters passed in via setup
        /// to create command buffer that clears the render target and blits to it using the clear material.
        /// </summary>
        /// <param name="renderer"> The ScriptableRenderer that is being used. </param>
        /// <param name="context"> The ScriptableRenderContext used to execute the command buffer that we create. </param>
        /// <param name="renderingData"> RenderingData (unused here). </param>
        /// <exception cref="ArgumentNullException"> Throw this exception if the renderer is null. </exception>
        public override void Execute(ScriptableRenderer renderer, ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderer == null)
                throw new ArgumentNullException("renderer");

            var cmd = CommandBufferPool.Get(k_ARBlitTag);

            RenderBufferLoadAction colorLoadOp = RenderBufferLoadAction.DontCare;
            RenderBufferStoreAction colorStoreOp = RenderBufferStoreAction.Store;

            RenderBufferLoadAction depthLoadOp = RenderBufferLoadAction.DontCare;
            RenderBufferStoreAction depthStoreOp = RenderBufferStoreAction.Store;

            SetRenderTarget(cmd, m_TargetIdentifier, colorLoadOp, colorStoreOp,
                m_DepthIdentifier, depthLoadOp, depthStoreOp, ClearFlag.All, Color.clear, m_Descriptor.dimension);

            cmd.Blit(null, m_TargetIdentifier, m_BlitMaterial);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
