using TMPro;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RequirementEntryView : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI m_IndexLabel;

        [SerializeField]
        TextMeshProUGUI m_ContentLabel;

        public void Initialize(int index, RequirementResult result)
        {
            m_IndexLabel.text = $"{index}.";

            var displayName = result.requirementName;
            if (displayName.StartsWith("m_"))
                displayName = displayName.Substring(2);

            var content = $"<b>{displayName}</b>";
            if (result.hasRemedy)
                content += $": <color=#AAAAAA>{result.remediationText}</color>";

            m_ContentLabel.text = content;
        }
    }
}
