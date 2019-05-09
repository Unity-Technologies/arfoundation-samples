using UnityEngine.Experimental.XR;
using UnityEngine.Rendering;
using UnityEngine.Rendering.LWRP;
using UnityEngine.Experimental.Rendering.LWRP;

namespace UnityEngine.XR.ARFoundation
{
    public class LWRPBeforeCameraRender : MonoBehaviour, IBeforeRender
    {
        const string k_ARBlitTag = "ARBackground Blit Pass";

        public Material blitMaterial { get; set; }

        /// <summary>
        /// Create, setup and return a ScriptableRenderPass that will blit the pass-thru video when this GameObject is rendered.
        /// </summary>
        /// <param name="baseDescriptor">RenderTextureDescriptor for this pass. </param>
        /// <param name="colorHandle">RenderTargetHandle for color buffer.</param>
        /// <param name="depthHandle">RenderTargetHandle for depth buffer.</param>
        /// <param name="clearFlag">ClearFlag used for rendering.</param>
        /// <returns></returns>
        public ScriptableRenderPass GetPassToEnqueue(RenderTextureDescriptor baseDescriptor, RenderTargetHandle colorHandle,
            RenderTargetHandle depthHandle, ClearFlag clearFlag)
        {
            var lwrpBackgroundRenderPass = new LWRPBackgroundRenderPass();
            lwrpBackgroundRenderPass.Setup(blitMaterial, colorHandle.Identifier(), depthHandle.Identifier(),
                baseDescriptor);
            return lwrpBackgroundRenderPass;
        }
    }
}
