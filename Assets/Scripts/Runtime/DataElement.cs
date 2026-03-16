using TMPro;

namespace UnityEngine.XR.ARFoundation
{
    public class DataElement : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI m_FieldLabel;
        
        [SerializeField]
        TextMeshProUGUI m_FieldValue;

        public void SetFieldLabel(string label)
        {
            if (m_FieldLabel != null)
                m_FieldLabel.text = label;
        }

        public void SetFieldValue<ValueType>(ValueType value)
        {
            if (m_FieldValue != null)
                m_FieldValue.text = value.ToString();
        }
    }
}
