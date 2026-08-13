using UnityEngine.XR.ARFoundation.Samples.Runtime;

namespace UnityEngine.XR.ARFoundation.Samples.Hands
{
    public class MetaHandViewFactory : IARHandViewFactory
    {
        IARHandView IARHandViewFactory.CreateHand(string name, Material material, ARShaderOcclusion shaderOcclusion, Transform xrOriginTransform)
        {
#if XR_HANDS_1_9_0_1_OR_NEWER
            var hand = new GameObject(name);
            var mesh = new Mesh();
            var skinnedMeshRenderer = hand.AddComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.sharedMesh = mesh;

            return new MetaHandView(skinnedMeshRenderer, material, xrOriginTransform, shaderOcclusion);
#else
            return null;
#endif
        }
    }
}
