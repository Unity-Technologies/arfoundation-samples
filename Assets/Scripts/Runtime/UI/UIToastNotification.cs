using TMPro;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class UIToastNotification : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI m_label;

        public void SetNotificationLabel(string text)
        {
            m_label.text = text;
        }
    }
}
