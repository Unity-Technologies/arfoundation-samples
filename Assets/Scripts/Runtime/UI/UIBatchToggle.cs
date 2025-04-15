using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(Toggle))]
    public class UIBatchToggle : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, ReadOnlyField]
        Toggle m_Toggle;

        [SerializeField]
        GameObject m_BatchToggleCheckMark;

        [SerializeField]
        GameObject m_MixedToggleVisualizer;

        [Header("Events")]
        [SerializeField]
        UnityEvent<IEnumerable<Toggle>> m_SelectedTogglesChanged = new();
        public UnityEvent<IEnumerable<Toggle>> selectedTogglesChanged => m_SelectedTogglesChanged;

        HashSet<Toggle> m_SelectedToggles = new();
        public ICollection<Toggle> selectedToggles => m_SelectedToggles;

        HashSet<Toggle> m_ChangedSelectedToggles = new();
        HashSet<Toggle> m_AllToggles = new();

        public void AddToggle(Toggle toggle)
        {
            toggle.onValueChanged.AddListener(isOn => OnToggleValueChanged(toggle, isOn));
            m_AllToggles.Add(toggle);
            UpdateBatchToggle();
        }

        public void RemoveToggle(Toggle toggle)
        {
            m_AllToggles.Remove(toggle);
            if (toggle.isOn)
                toggle.isOn = false;
            else
                UpdateBatchToggle();
        }

        void Reset()
        {
            m_Toggle = GetComponent<Toggle>();
        }

        void OnEnable()
        {
            m_Toggle.onValueChanged.AddListener(SetAllToggles);

            foreach (var toggle in m_AllToggles)
            {
                toggle.onValueChanged.AddListener(isOn => OnToggleValueChanged(toggle, isOn));
            }
        }

        void OnDisable()
        {
            m_Toggle.onValueChanged.RemoveListener(SetAllToggles);

            foreach (var toggle in m_AllToggles)
            {
                toggle.onValueChanged.RemoveListener(isOn => OnToggleValueChanged(toggle, isOn));
            }
        }

        void OnToggleValueChanged(Toggle toggle, bool isOn)
        {
            if (isOn)
                m_SelectedToggles.Add(toggle);
            else
                m_SelectedToggles.Remove(toggle);

            m_ChangedSelectedToggles.Add(toggle);
            UpdateBatchToggle();
        }

        void SetAllToggles(bool isOn)
        {
            foreach (var toggle in m_AllToggles)
            {
                if (toggle.isOn == isOn)
                    continue;

                toggle.SetIsOnWithoutNotify(isOn);
                m_ChangedSelectedToggles.Add(toggle);

                if (isOn)
                    m_SelectedToggles.Add(toggle);
                else
                    m_SelectedToggles.Remove(toggle);
            }

            UpdateBatchToggle();
        }

        void UpdateBatchToggle()
        {
            m_MixedToggleVisualizer.SetActive(false);
            m_BatchToggleCheckMark.SetActive(true);

            if (m_SelectedToggles.Count == 0)
            {
                m_Toggle.SetIsOnWithoutNotify(false);
                m_BatchToggleCheckMark.SetActive(false);
            }
            else if (m_SelectedToggles.Count == m_AllToggles.Count)
            {
                m_Toggle.SetIsOnWithoutNotify(true);
            }
            else
            {
                m_Toggle.SetIsOnWithoutNotify(m_SelectedToggles.Count == m_AllToggles.Count);
                m_MixedToggleVisualizer.SetActive(true);
                m_BatchToggleCheckMark.SetActive(false);
            }
            m_SelectedTogglesChanged?.Invoke(m_ChangedSelectedToggles);
            m_ChangedSelectedToggles.Clear();
        }
    }
}
