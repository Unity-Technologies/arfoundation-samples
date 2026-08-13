using System;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Meshing;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public interface IARHandView : IDisposable
    {
        Renderer renderer { get; }
        bool isInitialized { get; }

        void Update(XRHandSubsystem subsystem, in XRHandMeshData meshData);
        void UpdatePoses(XRHandSubsystem subsystem);
    }
}
