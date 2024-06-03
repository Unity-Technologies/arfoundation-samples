using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(Button))]
    [DisallowMultipleComponent]
    public class MenuCategory : MonoBehaviour
    {
        const string k_MenuSelectorErrorMsg =
            "Could not find a MenuSelector component in the scene. Add a MenuSelector then Reset this component.";

        [SerializeField, Tooltip("Children of this GameObject should have SceneLauncher components.")]
        GameObject m_CategoryRootGameObject;

        [SerializeField, Tooltip("Will be displayed at the top of the Menu scene when this category is selected.")]
        string m_CategoryName;

        [SerializeField, HideInInspector]
        Button m_Button;

        [SerializeField, HideInInspector]
        MenuSelector m_MenuSelector;

        List<IList<IBooleanExpression>> m_AllRequirementsInCategory = new();

        void Awake()
        {
            if (m_CategoryRootGameObject == null)
            {
                Debug.LogError($"{nameof(MenuCategory)} component on {name} has null Inspector properties and will be disabled.", this);
                return;
            }

            var sceneLaunchers = m_CategoryRootGameObject.GetComponentsInChildren<SceneLauncher>();
            foreach (var s in sceneLaunchers)
            {
                m_AllRequirementsInCategory.Add(s.sceneDescriptor.requirements);
            }
        }

        void OnEnable()
        {
            foreach (var sceneRequirements in m_AllRequirementsInCategory)
            {
                // Only enable the menu button if at least one sample scene in the category is supported on this device
                if (sceneRequirements.EvaluateAll())
                {
                    m_Button.onClick.AddListener(SelectCategory);
                    return;
                }
            }

            m_Button.SetEnabled(false);
        }

        void OnDisable()
        {
            m_Button.onClick.RemoveListener(SelectCategory);
        }
        
        void SelectCategory()
        {
            if (m_MenuSelector == null)
            {
                m_MenuSelector = FindAnyObjectByType<MenuSelector>();
                if (m_MenuSelector == null)
                {
                    Debug.LogError(k_MenuSelectorErrorMsg, this);
                    return;
                }
            }

            m_MenuSelector.SelectMenu(m_CategoryRootGameObject);
            m_MenuSelector.SetTitleLabelMenuName(m_CategoryName);
        }

        void Reset()
        {
            m_Button = GetComponent<Button>();
            m_MenuSelector = FindAnyObjectByType<MenuSelector>();

            if (m_MenuSelector == null)
                Debug.LogError(k_MenuSelectorErrorMsg, this);
        }
    }
}
