namespace UnityEngine.XR.ARFoundation.Samples
{
    public class UIAnchorInSceneVisualizer : MonoBehaviour
    {
        [SerializeField]
        GameObject m_InSceneVisualizer;

        [SerializeField]
        GameObject m_NotInSceneVisualizer;

        public void UpdateVisualizer(bool isInScene)
        {
            m_InSceneVisualizer.SetActive(isInScene);
            m_NotInSceneVisualizer.SetActive(!isInScene);
        }
    }
}
