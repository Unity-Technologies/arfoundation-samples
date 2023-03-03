using System.Runtime.CompilerServices;

namespace UnityEngine.XR.ARFoundation.Samples
{
    static class FindObjectsUtility
    {
        /// <summary>
        /// <see cref="Object.FindObjectOfType{T}()"/> was deprecated in Unity 2023.1.0. This method uses the new
        /// <c>Object.FindAnyObjectByType</c> method in Unity Editor versions 2023.1 and newer.
        /// </summary>
        /// <typeparam name="T">The Unity Object type to find.</typeparam>
        /// <returns>At most one active loaded instance of type <typeparamref name="T"/>.</returns>
        /// <remarks>
        /// Note that the new implementation does not sort the list of Objects before returning. If there are multiple
        /// active loaded instances of the requested type in your scene, you may find a different instance on 2023.1
        /// and newer than you would in older Editor versions.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T FindAnyObjectByType<T>() where T : Object
        {
#if UNITY_2023_1_OR_NEWER
            return Object.FindAnyObjectByType<T>();
#else
            return Object.FindObjectOfType<T>();
#endif
        }

        /// <summary>
        /// <see cref="Object.FindObjectOfType(System.Type)"/> was deprecated in Unity 2023.1.0. This method uses the new
        /// <c>Object.FindAnyObjectByType(System.Type)</c> method in Unity Editor versions 2023.1 and newer.
        /// </summary>
        /// <param name="type">The Unity Object type to find.</param>
        /// <returns>At most one active loaded instance of type <paramref name="type"/>.</returns>
        /// <remarks>
        /// Note that the new implementation does not sort the list of Objects before returning. If there are multiple
        /// active loaded instances of the requested type in your scene, you may find a different instance on 2023.1
        /// and newer than you would in older Editor versions.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Object FindAnyObjectByType(System.Type type)
        {
#if UNITY_2023_1_OR_NEWER
            return Object.FindAnyObjectByType(type);
#else
            return Object.FindObjectOfType(type);
#endif
        }

        /// <summary>
        /// <see cref="Object.FindObjectsOfType{T}()"/> was deprecated in Unity 2023.1.0. This method uses the new
        /// <c>Object.FindObjectsByType{T}(FindObjectsSortMode)</c> method in Unity Editor versions 2023.1 and newer.
        /// </summary>
        /// <typeparam name="T">The Unity Object type to find.</typeparam>
        /// <returns>All active loaded instances of type <typeparamref name="T"/>.</returns>
        /// <remarks>
        /// <para>Note that the new implementation does not sort the list of Objects before returning. If there are multiple
        /// active loaded instances of the requested type in your scene, the returned array may be in a different order
        /// on 2023.1 and newer than it will be in older Editor versions.</para>
        /// <para>If you require a sorted array, use <c>Object.FindObjectsByType{T}(FindObjectsSortMode)</c> with
        /// your desired sort mode.</para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] FindObjectsByType<T>() where T : Object
        {
#if UNITY_2023_1_OR_NEWER
            return Object.FindObjectsByType<T>(FindObjectsSortMode.None);
#else
            return Object.FindObjectsOfType<T>();
#endif
        }

        /// <summary>
        /// <see cref="Object.FindObjectsOfType(System.Type)"/> was deprecated in Unity 2023.1.0. This method uses the new
        /// <c>Object.FindObjectsByType(System.Type, FindObjectsSortMode)</c> method in Unity Editor versions 2023.1 and newer.
        /// </summary>
        /// <param name="type">The Unity Object type to find.</param>
        /// <returns>All active loaded instances of type <paramref name="type"/>.</returns>
        /// <remarks>
        /// <para>Note that the new implementation does not sort the list of Objects before returning. If there are multiple
        /// active loaded instances of the requested type in your scene, the returned array may be in a different order
        /// on 2023.1 and newer than it will be in older Editor versions.</para>
        /// <para>If you require a sorted array, use <c>Object.FindObjectsByType(System.Type, FindObjectsSortMode)</c>
        /// with your desired sort mode.</para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Object[] FindObjectsByType(System.Type type)
        {
#if UNITY_2023_1_OR_NEWER
            return Object.FindObjectsByType(type, FindObjectsSortMode.None);
#else
            return Object.FindObjectsOfType(type);
#endif
        }
    }
}
