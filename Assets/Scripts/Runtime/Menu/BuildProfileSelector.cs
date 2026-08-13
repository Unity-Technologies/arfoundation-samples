using System;
using TMPro;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Profile;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class BuildProfileSelector : MonoBehaviour
    {
        const string k_AllProfilesOption = "All Profiles";

#if UNITY_EDITOR
        public static Func<BuildProfile, RuntimeSceneManifest> refreshManifest;
#endif

        [SerializeField]
        TMP_Dropdown m_ProfileDropdown;

        [SerializeField]
        MenuController m_MenuController;

#if UNITY_EDITOR
        readonly List<BuildProfile> m_Profiles = new();
#endif

        void Start()
        {
#if UNITY_EDITOR
            PopulateDropdown();
            m_ProfileDropdown.onValueChanged.AddListener(OnProfileChanged);
#else
            m_ProfileDropdown.gameObject.SetActive(false);
#endif
        }

#if UNITY_EDITOR
        void OnDestroy()
        {
            m_ProfileDropdown.onValueChanged.RemoveListener(OnProfileChanged);
        }

        void PopulateDropdown()
        {
            m_Profiles.Clear();
            m_ProfileDropdown.ClearOptions();

            var files = Directory.GetFiles(SceneManifestPaths.buildProfilesDirectory, "*.asset");

            foreach (var file in files)
            {
                var assetPath = file.Replace('\\', '/');
                var profile = AssetDatabase.LoadAssetAtPath<BuildProfile>(assetPath);

                if (profile != null)
                    m_Profiles.Add(profile);
            }

            m_Profiles.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));

            var activeProfile = BuildProfile.GetActiveBuildProfile();
            var activeIndex = 0;
            var optionNames = new List<string>(m_Profiles.Count + 1)
            {
                k_AllProfilesOption
            };

            for (var i = 0; i < m_Profiles.Count; i++)
            {
                optionNames.Add(m_Profiles[i].name);
                if (m_Profiles[i] == activeProfile)
                    activeIndex = i + 1;
            }

            m_ProfileDropdown.AddOptions(optionNames);
            m_ProfileDropdown.SetValueWithoutNotify(activeIndex);
        }

        void OnProfileChanged(int index)
        {
            var selectedProfile =
                index > 0 && index <= m_Profiles.Count ?
                    m_Profiles[index - 1] :
                    null;

            refreshManifest?.Invoke(selectedProfile);
            m_MenuController.RebuildMenu();
        }
#endif
    }
}
