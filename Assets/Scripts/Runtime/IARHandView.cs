using System;
using UnityEngine.XR.Hands.Meshing;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public interface IARHandView : IDisposable
    {
        MeshFilter meshFilter { get; }
        MeshRenderer meshRenderer { get; }

        void Update(in XRHandMeshData meshData);
    }
}
