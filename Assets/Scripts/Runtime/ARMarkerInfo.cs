using System.Text;
using TMPro;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class ARMarkerInfo : MonoBehaviour
    {
        static readonly Color k_EnabledFeaturesTextColor = new(196f / 255f, 196f / 255f, 196f / 255f, 1f);

        [SerializeField]
        Image m_MarkerIcon;

        [SerializeField]
        TextMeshProUGUI m_MarkerTypeLabel;

        [SerializeField]
        TextMeshProUGUI m_EnabledFeaturesLabel;

        [SerializeField]
        GameObject m_DetectionEnabledIcon;

        StringBuilder m_StringBuilder = new();

        public void Set(
            bool isFixedSizeEnabled = false,
            float fixedSizeSideLength = 0f,
            bool isStatic = false,
            string dictionary = "")
        {
            m_MarkerIcon.color = Color.white;
            m_MarkerTypeLabel.color = Color.white;
            m_EnabledFeaturesLabel.color = k_EnabledFeaturesTextColor;
            m_DetectionEnabledIcon.SetActive(true);

            m_StringBuilder.Clear();

            if (dictionary != string.Empty)
                m_StringBuilder.Append(dictionary);

            if (!isStatic && !isFixedSizeEnabled)
            {
                m_EnabledFeaturesLabel.text = string.Empty;
                return;
            }

            if (isStatic)
            {
                // Add spacer
                if (dictionary != string.Empty)
                    m_StringBuilder.Append(" | ");

                m_StringBuilder.Append("Static");
            }

            if (isFixedSizeEnabled)
            {
                // Add spacer
                if (dictionary != string.Empty || isStatic)
                    m_StringBuilder.Append(" | ");

                m_StringBuilder.Append(fixedSizeSideLength.ToString("0.##"));
            }

            m_EnabledFeaturesLabel.text = m_StringBuilder.ToString();
        }
    }
}
