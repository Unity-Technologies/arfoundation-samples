
using System;
using UnityEngine.Rendering;

namespace UnityEngine.XR.ARFoundation
{
    [CreateAssetMenu(fileName = "LWRPBackgroundRendererAsset", menuName = "XR/LWRPBackgroundRendererAsset")]
    public class LWRPBackgroundRendererAsset : ARBackgroundRendererAsset
    {
        /// <summary>
        /// we're going to reference all materials that we want to use so that they get built into the project
        /// </summary>
        [SerializeField]
        Material[] m_MaterialsUsed;

        static bool useRenderPipeline {  get { return GraphicsSettings.renderPipelineAsset != null; } }
        
        public override ARFoundationBackgroundRenderer CreateARBackgroundRenderer()
        {
            return useRenderPipeline ? new LWRPBackgroundRenderer() : new ARFoundationBackgroundRenderer();
        }

        public override void CreateHelperComponents(GameObject cameraGameObject)
        {
            if (useRenderPipeline)
            {
                var lwrpBeforeCameraRender = cameraGameObject.GetComponent<LWRPBeforeCameraRender>();
                if (lwrpBeforeCameraRender == null)
                {
                    cameraGameObject.AddComponent<LWRPBeforeCameraRender>();
                }
            }
        }

        public override Material CreateCustomMaterial()
        {
            var cameraSubsystem = ARSubsystemManager.cameraSubsystem;
            if (cameraSubsystem == null)
                return null;

            // Try to create a material from the plugin's provided shader.
            var shaderName = "";
            if (!cameraSubsystem.TryGetShaderName(ref shaderName))
                return null;

            shaderName = shaderName + "LWRP";
            
            var shader = Shader.Find(shaderName);
            if (shader == null)
            {
                 throw new InvalidOperationException(string.Format(
                    "Could not find shader named \"{0}\" required for LWRP video overlay on camera subsystem named \"{1}\".",
                    shaderName,
                    cameraSubsystem.SubsystemDescriptor.id));
            }

            return new Material(shader);
        }
    }
    
}    
