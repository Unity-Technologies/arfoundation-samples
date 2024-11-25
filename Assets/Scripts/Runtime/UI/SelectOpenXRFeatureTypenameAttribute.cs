using System;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Use this attribute to serialize the typename of an OpenXRFeature in your project to a string variable
    /// in the Inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SelectOpenXRFeatureTypenameAttribute : PropertyAttribute
    {
    }
}
