using System.Collections.Generic;

namespace UnityEngine.XR.ARFoundation.Samples.Runtime
{
    public static class ListExtensions
    {
        public static string ToString<T>(this List<T> list) => string.Join("\n", list);
    }
}
