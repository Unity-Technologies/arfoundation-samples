using TMPro;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RequirementEntryView : MonoBehaviour
    {
        static readonly Color s_OddColor = new(16f / 255f, 16f / 255f, 16f / 255f, 1f);
        static readonly Color s_EvenColor = new(25f / 255f, 25f / 255f, 25f / 255f, 1f);

        [SerializeField]
        TextMeshProUGUI m_IndexLabel;

        [SerializeField]
        TextMeshProUGUI m_ContentLabel;

        [SerializeField]
        Image m_BackgroundImage;

        public void Initialize(int index, RequirementResult result)
        {
            var isOdd = index % 2 != 0;
            m_BackgroundImage.color = isOdd ? s_OddColor : s_EvenColor;

            m_IndexLabel.text = $"{index}.";

            var content = $"<b>{result.requirementName}</b>";
            if (result.hasRemedy)
                content += $": <color=#AAAAAA>{result.remediationText}</color>";

            m_ContentLabel.text = content;
        }
    }
}
