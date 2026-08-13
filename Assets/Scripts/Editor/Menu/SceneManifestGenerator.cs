using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Build.Profile;
using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples;

namespace UnityEditor.XR.ARFoundation.Samples
{
    static class SceneManifestGenerator
    {
        static readonly HashSet<string> s_ScenePathSet = new();
        static readonly List<string> s_ScenePathList = new();

        [MenuItem("AR Foundation/Refresh Scene Manifest")]
        public static void RefreshFromActiveBuildProfile()
        {
            Refresh(BuildProfile.GetActiveBuildProfile());
        }

        /// <summary>
        /// Refreshes the scene manifest from the given build profile. If <paramref name="profile"/>
        /// is <c>null</c>, falls back to collecting scenes from all build profile assets.
        /// </summary>
        public static RuntimeSceneManifest Refresh(BuildProfile profile)
        {
            if (profile == null)
            {
                Debug.LogWarning("[SceneManifestGenerator] No build profile provided. Falling back to all build profile scenes.");
                return RefreshAll();
            }

            var scenePaths = new List<string>();
            ResolveScenePaths(profile, scenePaths);
            var descriptors = CurateDescriptors(scenePaths);
            return SaveManifest(descriptors);
        }

        static void ResolveScenePaths(BuildProfile profile, List<string> scenePathsOutput)
        {
            scenePathsOutput.Clear();
            foreach (var scene in profile.scenes)
            {
                if (scene.enabled)
                    scenePathsOutput.Add(scene.path);
            }
        }

        static List<SampleSceneDescriptor> CurateDescriptors(List<string> scenePaths)
        {
            var result = new List<SampleSceneDescriptor>();
            foreach (var scenePath in scenePaths)
            {
                if (IsMenuScene(scenePath))
                    continue;

                var descriptor = FindDescriptor(scenePath);
                if (descriptor == null)
                {
                    throw new InvalidOperationException(
                        $"[SceneManifestGenerator] No {nameof(SampleSceneDescriptor)} found for scene: <color=white>{scenePath}</color>. Expected a co-located .asset file with the same name.");
                }

                result.Add(descriptor);
            }

            return result;
        }

        static SampleSceneDescriptor FindDescriptor(string scenePath)
        {
            var descriptorPath = Path.ChangeExtension(scenePath, ".asset");
            return AssetDatabase.LoadAssetAtPath<SampleSceneDescriptor>(descriptorPath);
        }

        static bool IsMenuScene(string scenePath)
        {
            return scenePath.Contains(SceneManifestPaths.menuScenesDirectory);
        }

        static RuntimeSceneManifest SaveManifest(List<SampleSceneDescriptor> descriptors)
        {
            EnsureResourcesDirectoryExists();

            var manifest = AssetDatabase.LoadAssetAtPath<RuntimeSceneManifest>(SceneManifestPaths.manifestAssetPath);
            if (manifest == null)
            {
                manifest = ScriptableObject.CreateInstance<RuntimeSceneManifest>();
                AssetDatabase.CreateAsset(manifest, SceneManifestPaths.manifestAssetPath);
                Debug.Log(
                    $"[SceneManifestGenerator] Created RuntimeSceneManifest at {SceneManifestPaths.manifestAssetPath}",
                    manifest);
            }

            manifest.SetScenes(descriptors);

            return manifest;
        }

        static RuntimeSceneManifest RefreshAll()
        {
            s_ScenePathSet.Clear();
            if (!Directory.Exists(SceneManifestPaths.buildProfilesDirectory))
            {
                Debug.LogWarning($"[SceneManifestGenerator] Build profiles directory does not exist: {SceneManifestPaths.buildProfilesDirectory}");
                return SaveManifest(new List<SampleSceneDescriptor>());
            }
            var files = Directory.GetFiles(SceneManifestPaths.buildProfilesDirectory, "*.asset");

            foreach (var file in files)
            {
                var assetPath = file.Replace('\\', '/');
                var profile = AssetDatabase.LoadAssetAtPath<BuildProfile>(assetPath);

                if (profile == null)
                    continue;

                foreach (var scene in profile.scenes)
                {
                    if (scene.enabled)
                        s_ScenePathSet.Add(scene.path);
                }
            }

            s_ScenePathList.Clear();
            s_ScenePathList.AddRange(s_ScenePathSet);
            var descriptors = CurateDescriptors(s_ScenePathList);
            return SaveManifest(descriptors);
        }

        static void EnsureResourcesDirectoryExists()
        {
            if (!AssetDatabase.IsValidFolder(SceneManifestPaths.resourcesDirectory))
                Directory.CreateDirectory(SceneManifestPaths.resourcesDirectory);
        }
    }
}
