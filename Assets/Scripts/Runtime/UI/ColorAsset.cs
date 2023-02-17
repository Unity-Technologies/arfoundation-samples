namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Use this class to store colors with notes in an easy-to-find location in your file system.
    /// </summary>
    public class ColorAsset : ScriptableObject
    {
        [SerializeField]
        Color m_Color;

        public Color color => m_Color;

        [SerializeField]
        [TextArea(4, 4)]
        [Space(4)]
        string m_Notes;
    }
}
