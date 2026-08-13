namespace UnityEngine.XR.ARFoundation.Samples.Runtime
{
    public interface IARHandViewFactory
    {
        IARHandView CreateHand(string name, Material material, ARShaderOcclusion shaderOcclusion, Transform xrOriginTransform);
    }
}
