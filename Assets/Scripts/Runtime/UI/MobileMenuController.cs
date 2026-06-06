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
            foreach (var category in m_CategoryOrder)
            {
                if (!m_ScenesByCategory.TryGetValue(category, out var entries))
                    continue;

                var categoryView = Instantiate(m_CategoryGroupPrefab, contentParent);
                categoryView.Initialize(category.categoryName);
                m_CategoryViews[category] = categoryView;

                foreach (var entry in entries)
                {
                    var sceneName = entry.descriptor.name;
                    var buttonView = Instantiate(sceneButtonPrefab, categoryView.buttonContainer);
                    buttonView.Initialize(
                        sceneName,
                        entry.descriptor.description,
                        entry.isSupported,
                        entry.descriptor.previewImage,
                        entry.isSupported
                            ? () => LaunchScene(sceneName)
                            : () => ShowRequirementsPopup(entry.descriptor));
                }
            }
        }

        protected override void OnClearMenu()
        {
            m_CategoryViews.Clear();
        }
    }
}
