using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RequirementsPopupView : MonoBehaviour
    {
        [SerializeField]
        RequirementEntryView m_EntryPrefab;

        [SerializeField]
        Transform m_ContentParent;

        [SerializeField]
        Button m_CloseButton;

        readonly List<RequirementEntryView> m_ActiveEntries = new();

        void Awake()
        {
            m_CloseButton.onClick.AddListener(Hide);
        }

        void OnDestroy()
        {
            if (m_CloseButton != null)
                m_CloseButton.onClick.RemoveListener(Hide);
        }

        public void Show(List<RequirementResult> results)
        {
            ClearEntries();

            var index = 1;
            foreach (var result in results)
            {
                if (result.isSupported)
                    continue;

                var entry = Instantiate(m_EntryPrefab, m_ContentParent);
                entry.Initialize(index++, result);
                m_ActiveEntries.Add(entry);
            }

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            ClearEntries();
        }

        void ClearEntries()
        {
            foreach (var entry in m_ActiveEntries)
                Destroy(entry.gameObject);

            m_ActiveEntries.Clear();
        }
    }
}
