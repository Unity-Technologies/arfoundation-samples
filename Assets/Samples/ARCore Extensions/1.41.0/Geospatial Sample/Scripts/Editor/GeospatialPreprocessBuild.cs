// <copyright file="GeospatialPreprocessBuild.cs" company="Google LLC">
//
// Copyright 2022 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace Google.XR.ARCoreExtensions.Samples.Geospatial.Editor
{
    using System.Diagnostics.CodeAnalysis;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;
    using UnityEngine;

    /// <summary>
    /// Setup Geospatial sample at build time.
    /// </summary>
    public class GeospatialPreprocessBuild : IPreprocessBuildWithReport
    {
        private const string _sceneGuid = "1ed15e9657253406185e4b2c7c5a4fc4";
        private const string _iconGuid = "007bbe997743f4b919e22f853abd8ceb";
        private const string _iconForegroundGuid = "d88cfb29aa03649de965d693ff90fe7a";
        private const string _iconBackgroundGuid = "08f878fa5803c4f6d85d0bf80987d1a4";

        /// <summary>
        /// Overridden <see cref="IOrderedCallback"/> property.
        /// </summary>
        [SuppressMessage(
            "UnityRules.UnityStyleRules", "US1109:PublicPropertiesMustBeUpperCamelCase",
            Justification = "Overridden property.")]
        public int callbackOrder => 0;

        /// <summary>
        /// A callback received before the build is started.</para>
        /// </summary>
        /// <param name="report">
        /// A report containing information about the build,
        /// such as its target platform and output path.
        /// </param>
        public void OnPreprocessBuild(BuildReport report)
        {
            EditorBuildSettingsScene enabledBuildScene = null;
            int enabledSceneCount = 0;
            foreach (var buildScene in EditorBuildSettings.scenes)
            {
                if (!buildScene.enabled)
                {
                    continue;
                }

                enabledBuildScene = buildScene;
                enabledSceneCount++;
            }

            if (enabledSceneCount != 1 || enabledBuildScene.guid.ToString() != _sceneGuid)
            {
                return;
            }

            // update application icon.
            if (report.summary.platform == BuildTarget.Android)
            {
#if UNITY_ANDROID
                Texture2D background = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    AssetDatabase.GUIDToAssetPath(_iconBackgroundGuid));
                Texture2D foreground = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    AssetDatabase.GUIDToAssetPath(_iconForegroundGuid));
                PlatformIcon[] platformIcons =
                    PlayerSettings.GetPlatformIcons(BuildTargetGroup.Android,
                        UnityEditor.Android.AndroidPlatformIconKind.Adaptive);
                foreach (var platformIcon in platformIcons)
                {
                    if (platformIcon.GetTexture() != null &&
                        platformIcon.GetTexture() != background &&
                        platformIcon.GetTexture() != foreground)
                    {
                        // Keep the custom icons.
                        return;
                    }
                    else
                    {
                        platformIcon.SetTexture(background, 0);
                        platformIcon.SetTexture(foreground, 1);
                    }
                }

                PlayerSettings.SetPlatformIcons(BuildTargetGroup.Android,
                    UnityEditor.Android.AndroidPlatformIconKind.Adaptive, platformIcons);
#endif // UNITY_ANDROID
            }
            else if (report.summary.platform == BuildTarget.iOS)
            {
#if UNITY_IOS
                Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    AssetDatabase.GUIDToAssetPath(_iconGuid));
                PlatformIcon[] platformIcons =
                    PlayerSettings.GetPlatformIcons(BuildTargetGroup.iOS,
                        UnityEditor.iOS.iOSPlatformIconKind.Application);
                foreach (var platformIcon in platformIcons)
                {
                    if (platformIcon.GetTexture() != null &&
                        platformIcon.GetTexture() != icon)
                    {
                        // Keep the custom icons.
                        return;
                    }
                    else
                    {
                        platformIcon.SetTexture(icon);
                    }
                }

                PlayerSettings.SetPlatformIcons(BuildTargetGroup.iOS,
                    UnityEditor.iOS.iOSPlatformIconKind.Application, platformIcons);
#endif // UNITY_IOS
            }
        }
    }
}
