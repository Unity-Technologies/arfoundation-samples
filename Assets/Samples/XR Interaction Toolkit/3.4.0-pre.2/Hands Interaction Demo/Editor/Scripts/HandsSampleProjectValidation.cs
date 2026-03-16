using System;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils.Editor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager.UI;
using UnityEditor.XR.Interaction.Toolkit.ProjectValidation;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Samples.Hands.Editor
{
    /// <summary>
    /// Unity Editor class which registers Project Validation rules for the Hands Interaction Demo sample,
    /// checking that other required samples and packages are installed.
    /// </summary>
    static class HandsSampleProjectValidation
    {
        const string k_SampleDisplayName = "Hands Interaction Demo";
        const string k_Category = "XR Interaction Toolkit";
        const string k_StarterAssetsSampleName = "Starter Assets";
        const string k_HandVisualizerSampleName = "HandVisualizer";
        const string k_ProjectValidationSettingsPath = "Project/XR Plug-in Management/Project Validation";
        const string k_HandsPackageDisplayName = "XR Hands";
        const string k_HandsPackageName = "com.unity.xr.hands";
        const string k_XRIPackageName = "com.unity.xr.interaction.toolkit";
        const string k_ShaderGraphPackageName = "com.unity.shadergraph";
        static readonly PackageVersion s_MinimumHandsPackageVersion = new PackageVersion("1.5.1");
        static readonly PackageVersion s_RecommendedHandsPackageVersion = new PackageVersion("1.6.1");

        static readonly BuildTargetGroup[] s_BuildTargetGroups =
            ((BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup))).Distinct().ToArray();

        static readonly List<BuildValidationRule> s_BuildValidationRules = new List<BuildValidationRule>
        {
            new BuildValidationRule
            {
                IsRuleEnabled = () => s_HandsPackageAddRequest == null || s_HandsPackageAddRequest.IsCompleted,
                Message = $"[{k_SampleDisplayName}] XR Hands ({k_HandsPackageName}) package must be installed or updated to use this sample.",
                Category = k_Category,
                CheckPredicate = () => PackageVersionUtility.GetPackageVersion(k_HandsPackageName) >= s_MinimumHandsPackageVersion,
                FixIt = () =>
                {
                    if (s_HandsPackageAddRequest == null || s_HandsPackageAddRequest.IsCompleted)
                        ProjectValidationUtility.InstallOrUpdatePackage(k_HandsPackageName, s_RecommendedHandsPackageVersion, ref s_HandsPackageAddRequest);
                },
                FixItAutomatic = true,
                Error = true,
            },
            new BuildValidationRule
            {
                IsRuleEnabled = () => s_HandsPackageAddRequest == null || s_HandsPackageAddRequest.IsCompleted,
                Message = $"[{k_SampleDisplayName}] XR Hands ({k_HandsPackageName}) package must be at version {s_RecommendedHandsPackageVersion} or higher to use the latest sample features.",
                Category = k_Category,
                CheckPredicate = () => PackageVersionUtility.GetPackageVersion(k_HandsPackageName) >= s_RecommendedHandsPackageVersion,
                FixIt = () =>
                {
                    if (s_HandsPackageAddRequest == null || s_HandsPackageAddRequest.IsCompleted)
                        ProjectValidationUtility.InstallOrUpdatePackage(k_HandsPackageName, s_RecommendedHandsPackageVersion, ref s_HandsPackageAddRequest);
                },
                FixItAutomatic = true,
                Error = false,
            },
            new BuildValidationRule
            {
                IsRuleEnabled = () => PackageVersionUtility.GetPackageVersion(k_HandsPackageName) >= s_MinimumHandsPackageVersion,
                Message = $"[{k_SampleDisplayName}] {k_HandVisualizerSampleName} sample from XR Hands ({k_HandsPackageName}) package must be imported or updated to use this sample.",
                Category = k_Category,
                CheckPredicate = () => ProjectValidationUtility.SampleImportMeetsMinimumVersion(k_HandsPackageDisplayName, k_HandVisualizerSampleName, PackageVersionUtility.GetPackageVersion(k_HandsPackageName)),
                FixIt = () =>
                {
                    if (TryFindSample(k_HandsPackageName, string.Empty, k_HandVisualizerSampleName, out var sample))
                    {
                        sample.Import(Sample.ImportOptions.OverridePreviousImports);
                    }
                },
                FixItAutomatic = true,
                Error = !ProjectValidationUtility.SampleImportMeetsMinimumVersion(k_HandsPackageDisplayName, k_HandVisualizerSampleName, s_MinimumHandsPackageVersion),
            },
            new BuildValidationRule
            {
                Message = $"[{k_SampleDisplayName}] {k_StarterAssetsSampleName} sample from XR Interaction Toolkit ({k_XRIPackageName}) package must be imported or updated to use this sample. {GetImportSampleVersionMessage(k_Category, k_StarterAssetsSampleName, ProjectValidationUtility.minimumXRIStarterAssetsSampleVersion)}",
                Category = k_Category,
                CheckPredicate = () => ProjectValidationUtility.SampleImportMeetsMinimumVersion(k_Category, k_StarterAssetsSampleName, ProjectValidationUtility.minimumXRIStarterAssetsSampleVersion),
                FixIt = () =>
                {
                    if (TryFindSample(k_XRIPackageName, string.Empty, k_StarterAssetsSampleName, out var sample))
                    {
                        sample.Import(Sample.ImportOptions.OverridePreviousImports);
                    }
                },
                FixItAutomatic = true,
                Error = !ProjectValidationUtility.HasSampleImported(k_Category, k_StarterAssetsSampleName),
            },
            new BuildValidationRule
            {
                IsRuleEnabled = () => s_ShaderGraphPackageAddRequest == null || s_ShaderGraphPackageAddRequest.IsCompleted,
                Message = $"[{k_SampleDisplayName}] Shader Graph ({k_ShaderGraphPackageName}) package must be installed for materials used in this sample.",
                Category = k_Category,
                CheckPredicate = () => PackageVersionUtility.IsPackageInstalled(k_ShaderGraphPackageName),
                FixIt = () =>
                {
                    s_ShaderGraphPackageAddRequest = Client.Add(k_ShaderGraphPackageName);
                    if (s_ShaderGraphPackageAddRequest.Error != null)
                    {
                        Debug.LogError($"Package installation error: {s_ShaderGraphPackageAddRequest.Error}: {s_ShaderGraphPackageAddRequest.Error.message}");
                    }
                },
                FixItAutomatic = true,
                Error = false,
            },
        };

        static AddRequest s_HandsPackageAddRequest;
        static AddRequest s_ShaderGraphPackageAddRequest;

        [InitializeOnLoadMethod]
        static void RegisterProjectValidationRules()
        {
            foreach (var buildTargetGroup in s_BuildTargetGroups)
            {
                BuildValidator.AddRules(buildTargetGroup, s_BuildValidationRules);
            }

            // Delay evaluating conditions for issues to give time for Package Manager and UPM cache to fully initialize.
            EditorApplication.delayCall += ShowWindowIfIssuesExist;
        }

        static void ShowWindowIfIssuesExist()
        {
            foreach (var validation in s_BuildValidationRules)
            {
                if (validation.CheckPredicate == null || (!validation.CheckPredicate.Invoke() && validation.Error))
                {
                    ShowWindow();
                    return;
                }
            }
        }

        internal static void ShowWindow()
        {
            // Delay opening the window since sometimes other settings in the player settings provider redirect to the
            // project validation window causing serialized objects to be nullified.
            EditorApplication.delayCall += () =>
            {
                SettingsService.OpenProjectSettings(k_ProjectValidationSettingsPath);
            };
        }

        static bool TryFindSample(string packageName, string packageVersion, string sampleDisplayName, out Sample sample)
        {
            sample = default;

            if (!PackageVersionUtility.IsPackageInstalled(packageName))
                return false;

            IEnumerable<Sample> packageSamples;
            try
            {
                packageSamples = Sample.FindByPackage(packageName, packageVersion);
            }
            catch (Exception e)
            {
                Debug.LogError($"Couldn't find samples of the {ToString(packageName, packageVersion)} package; aborting project validation rule. Exception: {e}");
                return false;
            }

            if (packageSamples == null)
            {
                Debug.LogWarning($"Couldn't find samples of the {ToString(packageName, packageVersion)} package; aborting project validation rule.");
                return false;
            }

            foreach (var packageSample in packageSamples)
            {
                if (packageSample.displayName == sampleDisplayName)
                {
                    sample = packageSample;
                    return true;
                }
            }

            Debug.LogWarning($"Couldn't find {sampleDisplayName} sample in the {ToString(packageName, packageVersion)} package; aborting project validation rule.");
            return false;
        }

        static string ToString(string packageName, string packageVersion)
        {
            return string.IsNullOrEmpty(packageVersion) ? packageName : $"{packageName}@{packageVersion}";
        }

        static string GetImportSampleVersionMessage(string packageFolderName, string sampleDisplayName, PackageVersion version)
        {
            if (ProjectValidationUtility.SampleImportMeetsMinimumVersion(packageFolderName, sampleDisplayName, version) || !ProjectValidationUtility.HasSampleImported(packageFolderName, sampleDisplayName))
                return string.Empty;

            return $"An older version of {sampleDisplayName} has been found. This may cause errors.";
        }
    }
}
