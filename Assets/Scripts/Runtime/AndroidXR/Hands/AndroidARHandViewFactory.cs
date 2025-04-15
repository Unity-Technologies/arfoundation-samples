using UnityEngine.XR.ARFoundation.Samples.Runtime;

namespace UnityEngine.XR.ARFoundation.Samples.Hands
{
    public class AndroidARHandViewFactory : IARHandViewFactory
    {
        IARHandView IARHandViewFactory.CreateHand(string name, Material material, ARShaderOcclusion shaderOcclusion)
        {
            var hand = new GameObject(name);
            var mesh = new Mesh();
            mesh.MarkDynamic();
            var meshFilter = hand.AddComponent<MeshFilter>();
            var meshRenderer = hand.AddComponent<MeshRenderer>();
            meshFilter.mesh = mesh;

            return new AndroidARHandView(meshFilter, meshRenderer, material, shaderOcclusion);
        }
    }
}
