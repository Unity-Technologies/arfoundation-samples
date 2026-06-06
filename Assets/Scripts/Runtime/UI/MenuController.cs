using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public abstract class MenuController : MonoBehaviour
    {
        [SerializeField]
        SceneButtonView m_SceneButtonPrefab;

        [SerializeField]
        Transform m_ContentParent;

        [SerializeField]
        RequirementsPopupView m_RequirementsPopup;

        readonly List<RequirementResult> m_RequirementResults = new();
        readonly Dictionary<SampleSceneDescriptor, List<RequirementResult>> m_CachedResults = new();

        protected readonly List<SceneCategory> m_CategoryOrder = new();
        protected readonly Dictionary<SceneCategory, List<SceneEntry>> m_ScenesByCategory = new();

        protected SceneButtonView sceneButtonPrefab => m_SceneButtonPrefab;
        protected Transform contentParent => m_ContentParent;

        protected struct SceneEntry
        {
            public SampleSceneDescriptor descriptor;
            public bool isSupported;
        }

        void Start()
        {
            LoadAndBuildMenu();
        }

        protected void LoadAndBuildMenu()
        {
            var manifest = Resources.Load<RuntimeSceneManifest>(RuntimeSceneManifest.k_ResourcesPath);
            if (manifest == null)
            {
                Debug.LogError(
                    "RuntimeSceneManifest not found in Resources. Run AR Foundation > Generate Scene Manifest first.",
                    this);
                return;
            }

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

            m_CategoryOrder.Sort((a, b) => string.Compare(a.categoryName, b.categoryName, StringComparison.Ordinal));

            BuildMenuLayout();
        }

        protected abstract void BuildMenuLayout();

        protected void LaunchScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        protected void ShowRequirementsPopup(SampleSceneDescriptor descriptor)
        {
            m_RequirementsPopup.Show(m_CachedResults[descriptor]);
        }

        protected virtual void OnClearMenu()
        {
        }

        void ClearMenu()
        {
            if (m_ContentParent != null)
            {
                foreach (Transform child in m_ContentParent)
                    Destroy(child.gameObject);
            }

            m_CachedResults.Clear();
            m_CategoryOrder.Clear();
            m_ScenesByCategory.Clear();
            OnClearMenu();
        }

        public void RebuildMenu()
        {
            ClearMenu();
            LoadAndBuildMenu();
        }
    }
}
