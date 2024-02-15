using System;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Use the attribute in combination with SerializeReference to select a concrete implementation type of a field
    /// in the Inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SelectImplementationAttribute : PropertyAttribute
    {
        public Type fieldType { get; }

        public SelectImplementationAttribute(Type fieldType)
        {
            this.fieldType = fieldType;
        }
    }
}
