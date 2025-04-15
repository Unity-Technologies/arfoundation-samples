namespace UnityEngine.XR.ARFoundation.Samples.Runtime
{
    public interface IARHandViewFactory
    {
        public IARHandView CreateHand(string name, Material material, ARShaderOcclusion shaderOcclusion);
    }
}
