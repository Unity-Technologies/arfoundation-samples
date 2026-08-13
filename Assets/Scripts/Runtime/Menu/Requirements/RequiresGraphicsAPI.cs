using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresGraphicsAPI : ISceneRequirement
    {
        [SerializeField]
        GraphicsDeviceType m_RequiredGraphicsDeviceType;

        public virtual void Evaluate(List<RequirementResult> results)
        {
            results.Add(new RequirementResult(
                SystemInfo.graphicsDeviceType == m_RequiredGraphicsDeviceType,
                $"Graphics API ({m_RequiredGraphicsDeviceType})",
                $"Enable {m_RequiredGraphicsDeviceType} in <b>Project Settings</b> > <b>Player</b> > <b>Other Settings</b> > <b>Graphics API</b>."));
        }
    }
}
