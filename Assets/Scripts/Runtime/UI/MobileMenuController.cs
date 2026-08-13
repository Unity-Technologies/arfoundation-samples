using System.Collections.Generic;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MobileMenuController : MenuController
    {
        [SerializeField]
        CategoryGroupView m_CategoryGroupPrefab;

        readonly Dictionary<SceneCategory, CategoryGroupView> m_CategoryViews = new();

        protected override void BuildMenuLayout()
        {
            foreach (var category in categoryOrder)
            {
                if (!scenesByCategory.TryGetValue(category, out var entries))
                    continue;

                var categoryView = Instantiate(m_CategoryGroupPrefab, contentParent);
                categoryView.Initialize(category.categoryName);
                m_CategoryViews[category] = categoryView;

                foreach (var entry in entries)
                    CreateSceneButton(category, entry, categoryView.buttonContainer);
            }
        }

        protected override void OnCategoryFilterApplied()
        {
            foreach (var (category, categoryView) in m_CategoryViews)
            {
                var hasVisibleButton = false;
                if (buttonsByCategory.TryGetValue(category, out var buttons))
                {
                    foreach (var button in buttons)
                    {
                        if (button.gameObject.activeSelf)
                        {
                            hasVisibleButton = true;
                            break;
                        }
                    }
                }

                categoryView.gameObject.SetActive(hasVisibleButton);
            }
        }

        protected override void OnClearMenu()
        {
            m_CategoryViews.Clear();
        }
    }
}
