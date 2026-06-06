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
        const string k_MenuScenesDirectory = "Scenes/Menu/";
        const string k_ResourcesDirectory = "Assets/Settings/Build Profiles/Resources";
        const string k_ManifestAssetPath = k_ResourcesDirectory + "/RuntimeSceneManifest.asset";

        [MenuItem("AR Foundation/Refresh Scene Manifest")]
        public static void RefreshFromActiveBuildProfile()
        {
            Refresh(BuildProfile.GetActiveBuildProfile());
        }

        public static RuntimeSceneManifest Refresh(BuildProfile profile)
        {
            var scenePaths = new List<string>();
            ResolveScenePaths(profile, scenePaths);
            var descriptors = CurateDescriptors(scenePaths);
            return SaveManifest(descriptors);
        }

        static void ResolveScenePaths(BuildProfile profile, List<string> scenePathsOutput)
        {
            scenePathsOutput.Clear();

            if (profile != null)
            {
                foreach (var scene in profile.scenes)
                {
                    if (scene.enabled)
                        scenePathsOutput.Add(scene.path);
                }

                return;
            }

            // Fallback for non-profile build path
            foreach (var scene in EditorBuildSettings.scenes)
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
                        $"No {nameof(SampleSceneDescriptor)} found for scene: <color=white>{scenePath}</color>. Expected a co-located .asset file with the same name.");
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
            return scenePath.Contains(k_MenuScenesDirectory);
        }

        static RuntimeSceneManifest SaveManifest(List<SampleSceneDescriptor> descriptors)
        {
            EnsureResourcesDirectoryExists();

            var manifest = AssetDatabase.LoadAssetAtPath<RuntimeSceneManifest>(k_ManifestAssetPath);
            if (manifest == null)
            {
                manifest = ScriptableObject.CreateInstance<RuntimeSceneManifest>();
                AssetDatabase.CreateAsset(manifest, k_ManifestAssetPath);
                Debug.Log(
                    $"[SceneManifestGenerator] Created RuntimeSceneManifest at {k_ManifestAssetPath}",
                    manifest);
            }

            manifest.SetScenes(descriptors);

            return manifest;
        }

        static void EnsureResourcesDirectoryExists()
        {
            if (!AssetDatabase.IsValidFolder(k_ResourcesDirectory))
                Directory.CreateDirectory(k_ResourcesDirectory);
        }
    }
}
