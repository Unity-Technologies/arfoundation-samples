using System;
using TMPro;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class UIAnchorLastSaveVisualizer : MonoBehaviour
    {
        [Header("Saved Visualizer")]
        [SerializeField]
        GameObject m_SaveAnchorVisualizer;

        [SerializeField]
        TextMeshProUGUI m_DateLabel;

        [SerializeField]
        TextMeshProUGUI m_TimeLabel;

        [Header("Not Saved Visualizer")]
        [SerializeField]
        GameObject m_NotSaveAnchorVisualizer;

        public void UpdateVisualizer(bool isSaved, DateTime dateTime)
        {
            m_SaveAnchorVisualizer?.SetActive(isSaved);
            m_NotSaveAnchorVisualizer?.SetActive(!isSaved);

            if (isSaved)
            {
                m_DateLabel.text = $"{dateTime:d}";
                m_TimeLabel.text = $"{dateTime:t}";
            }
        }
    }
}
