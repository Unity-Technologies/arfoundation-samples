namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Defines a category used to group sample scenes in the menu UI.
    /// </summary>
    [CreateAssetMenu(fileName = "SceneCategory", menuName = "XR/AR Foundation/Samples/Scene Category")]
    public class SceneCategory : ScriptableObject
    {
        [SerializeField, Tooltip("The display name of this category shown in the menu UI.")]
        string m_CategoryName;

        /// <summary>
        /// The display name of this category.
        /// </summary>
        public string categoryName => m_CategoryName;
    }
}
