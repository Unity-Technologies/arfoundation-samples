using TMPro;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class CategoryGroupView : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI m_CategoryLabel;

        public Transform buttonContainer => transform;

        public void Initialize(string categoryName)
        {
            m_CategoryLabel.text = categoryName;
        }
    }
}
