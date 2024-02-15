using System;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresVisualScripting : IBooleanExpression
    {
        public bool Evaluate()
        {
#if VISUALSCRIPTING_1_8_OR_NEWER
            return true;
#endif
#pragma warning disable CS0162
            return false;
#pragma warning restore CS0162
        }
    }
}
