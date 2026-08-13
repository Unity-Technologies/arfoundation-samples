using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine.SceneManagement;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public abstract class MenuController : MonoBehaviour
    {
        protected struct SceneEntry
        {
            public SampleSceneDescriptor descriptor;
            public bool isSupported;
        }

        const string k_AllCategoriesOption = "All Categories";
        const string k_HighlightOpen = "<mark=#337BFF55>";
        const string k_HighlightClose = "</mark>";

        static readonly StringBuilder s_HighlightBuilder = new();

        [SerializeField]
        SceneButtonView m_SceneButtonPrefab;

        [SerializeField]
        Transform m_ContentParent;

        [SerializeField]
        RequirementsPopupView m_RequirementsPopup;

        [SerializeField]
        TMP_Dropdown m_CategoryDropdown;

        [SerializeField]
        MenuSearchBar m_SearchBar;

        readonly List<RequirementResult> m_RequirementResults = new();
        readonly Dictionary<SampleSceneDescriptor, List<RequirementResult>> m_CachedResults = new();
        readonly Dictionary<SceneCategory, List<SceneButtonView>> m_ButtonsByCategory = new();

        protected IReadOnlyDictionary<SceneCategory, List<SceneButtonView>> buttonsByCategory => m_ButtonsByCategory;

        readonly List<SceneCategory> m_CategoryOrder = new();
        readonly Dictionary<SceneCategory, List<SceneEntry>> m_ScenesByCategory = new();

        protected IReadOnlyList<SceneCategory> categoryOrder => m_CategoryOrder;
        protected IReadOnlyDictionary<SceneCategory, List<SceneEntry>> scenesByCategory => m_ScenesByCategory;
        protected Transform contentParent => m_ContentParent;

        bool m_HasBuiltMenu;

        void OnEnable()
        {
            if (m_SearchBar != null)
                m_SearchBar.searchTextChanged += ApplySearchFilter;
        }

        void OnDisable()
        {
            if (m_SearchBar != null)
                m_SearchBar.searchTextChanged -= ApplySearchFilter;
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus && m_HasBuiltMenu)
                RebuildMenu();
        }

        void Start()
        {
            var manifest = Resources.Load<RuntimeSceneManifest>(RuntimeSceneManifest.runtimeSceneManifestPath);
            if (manifest == null)
            {
                Debug.LogError(
                    "RuntimeSceneManifest not found in Resources. Run AR Foundation > Generate Scene Manifest first.",
                    this);
                return;
            }

            RequiresPermission.RequestAllPending(manifest.sceneDescriptors, LoadAndBuildMenu);
        }

        protected void LoadAndBuildMenu()
        {
            var manifest = Resources.Load<RuntimeSceneManifest>(RuntimeSceneManifest.runtimeSceneManifestPath);
            if (manifest == null)
                return;

            BuildMenu(manifest);
        }

        void BuildMenu(RuntimeSceneManifest manifest)
        {
            foreach (var descriptor in manifest.sceneDescriptors)
            {
                var category = descriptor.category;
                if (!m_ScenesByCategory.TryGetValue(category, out var list))
                {
                    list = new List<SceneEntry>();
                    m_ScenesByCategory[category] = list;
                    m_CategoryOrder.Add(category);
                }

                var isSupported = descriptor.EvaluateRequirements(m_RequirementResults);
                if (!isSupported)
                    m_CachedResults[descriptor] = new List<RequirementResult>(m_RequirementResults);

                list.Add(new SceneEntry
                {
                    descriptor = descriptor,
                    isSupported = isSupported,
                });
            }

            m_CategoryOrder.Sort(
                (a, b) => string.Compare(a.categoryName, b.categoryName, StringComparison.Ordinal));

            m_HasBuiltMenu = true;
            BuildMenuLayout();
            PopulateCategoryDropdown();

            if (m_SearchBar != null && !string.IsNullOrEmpty(m_SearchBar.searchText))
                ApplySearchFilter(m_SearchBar.searchText);
        }

        protected abstract void BuildMenuLayout();

        protected SceneButtonView CreateSceneButton(
            SceneCategory category, SceneEntry entry, Transform parent)
        {
            var sceneDisplayName = entry.descriptor.displayName;
            var sceneName = entry.descriptor.name;
            var buttonView = Instantiate(m_SceneButtonPrefab, parent);
            buttonView.Initialize(
                sceneDisplayName,
                entry.descriptor.description,
                entry.isSupported,
                entry.descriptor.previewImage,
                entry.isSupported
                    ? () => LaunchScene(sceneName)
                    : () => ShowRequirementsPopup(entry.descriptor));

            if (!m_ButtonsByCategory.TryGetValue(category, out var buttons))
            {
                buttons = new List<SceneButtonView>();
                m_ButtonsByCategory[category] = buttons;
            }

            buttons.Add(buttonView);
            return buttonView;
        }

        void PopulateCategoryDropdown()
        {
            if (m_CategoryDropdown == null)
                return;

            m_CategoryDropdown.onValueChanged.RemoveListener(ApplyCategoryFilter);
            m_CategoryDropdown.ClearOptions();

            var optionNames = new List<string>(m_CategoryOrder.Count + 1) { k_AllCategoriesOption };
            foreach (var category in m_CategoryOrder)
                optionNames.Add(category.categoryName);

            m_CategoryDropdown.AddOptions(optionNames);
            m_CategoryDropdown.SetValueWithoutNotify(0);
            m_CategoryDropdown.onValueChanged.AddListener(ApplyCategoryFilter);
        }

        void ApplyCategoryFilter(int index)
        {
            SceneCategory selectedCategory = null;
            if (index > 0 && index <= m_CategoryOrder.Count)
                selectedCategory = m_CategoryOrder[index - 1];

            foreach (var (category, buttons) in m_ButtonsByCategory)
            {
                var visible = selectedCategory == null || category == selectedCategory;
                foreach (var button in buttons)
                    button.gameObject.SetActive(visible);
            }

            OnCategoryFilterApplied();
        }

        void ApplySearchFilter(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                foreach (var (_, buttons) in m_ButtonsByCategory)
                {
                    foreach (var button in buttons)
                        button.SetDisplayNameLabel(button.sceneDisplayName);
                }

                ApplyCategoryFilter(m_CategoryDropdown != null ? m_CategoryDropdown.value : 0);
                return;
            }

            foreach (var (_, buttons) in m_ButtonsByCategory)
            {
                foreach (var button in buttons)
                {
                    var matchIndex = button.sceneDisplayName.IndexOf(
                        query, StringComparison.OrdinalIgnoreCase);

                    if (matchIndex < 0)
                    {
                        button.gameObject.SetActive(false);
                        continue;
                    }

                    button.gameObject.SetActive(true);
                    button.SetDisplayNameLabel(BuildHighlightedText(button.sceneDisplayName, matchIndex, query.Length));
                }
            }

            OnCategoryFilterApplied();
        }

        static string BuildHighlightedText(string text, int matchStart, int matchLength)
        {
            s_HighlightBuilder.Clear();
            s_HighlightBuilder.Append(text, 0, matchStart);
            s_HighlightBuilder.Append(k_HighlightOpen);
            s_HighlightBuilder.Append(text, matchStart, matchLength);
            s_HighlightBuilder.Append(k_HighlightClose);
            s_HighlightBuilder.Append(text, matchStart + matchLength, text.Length - matchStart - matchLength);
            return s_HighlightBuilder.ToString();
        }

        protected void LaunchScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        protected void ShowRequirementsPopup(SampleSceneDescriptor descriptor)
        {
            m_RequirementsPopup.Show(m_CachedResults[descriptor]);
        }

        protected virtual void OnCategoryFilterApplied()
        {
        }

        protected virtual void OnClearMenu()
        {
        }

        void ClearMenu()
        {
            if (m_CategoryDropdown != null)
                m_CategoryDropdown.onValueChanged.RemoveListener(ApplyCategoryFilter);

            if (m_ContentParent != null)
            {
                foreach (Transform child in m_ContentParent)
                    Destroy(child.gameObject);
            }

            m_CachedResults.Clear();
            m_CategoryOrder.Clear();
            m_ScenesByCategory.Clear();
            m_ButtonsByCategory.Clear();
            OnClearMenu();
        }

        public void RebuildMenu()
        {
            ClearMenu();
            LoadAndBuildMenu();
        }
    }
}
