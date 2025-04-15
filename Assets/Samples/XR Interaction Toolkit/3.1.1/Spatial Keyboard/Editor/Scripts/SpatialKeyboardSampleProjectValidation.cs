using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.XR.CoreUtils.Editor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager.UI;
using UnityEditor.XR.Interaction.Toolkit.ProjectValidation;
using UnityEngine;
#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
using TMPro;
#endif

namespace UnityEditor.XR.Interaction.Toolkit.Samples.SpatialKeyboard.Editor
{
    /// <summary>
    /// Unity Editor class which registers Project Validation rules for the Spatial Keyboard sample,
    /// checking that required samples and packages are installed.
    /// </summary>
    static class SpatialKeyboardSampleProjectValidation
    {
        const string k_SampleDisplayName = "Spatial Keyboard";
        const string k_Category = "XR Interaction Toolkit";
        const string k_StarterAssetsSampleName = "Starter Assets";
        const string k_ProjectValidationSettingsPath = "Project/XR Plug-in Management/Project Validation";
        const string k_XRIPackageName = "com.unity.xr.interaction.toolkit";
#if UNITY_6000_0_OR_NEWER
        const string k_UIPackageName = "com.unity.ugui";
#else
        const string k_UIPackageName = "com.unity.textmeshpro";
#endif

        static readonly BuildTargetGroup[] s_BuildTargetGroups =
            ((BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup))).Distinct().ToArray();

        static AddRequest s_UIPackageAddRequest;

        static readonly List<BuildValidationRule> s_BuildValidationRules = new List<BuildValidationRule>
        {
            new BuildValidationRule
            {
                Message = $"[{k_SampleDisplayName}] {k_StarterAssetsSampleName} sample from XR Interaction Toolkit ({k_XRIPackageName}) package must be imported or updated to use this sample. {GetImportSampleVersionMessage(k_Category, k_StarterAssetsSampleName, PackageVersionUtility.GetPackageVersion(k_XRIPackageName))}",
                Category = k_Category,
                CheckPredicate = () => ProjectValidationUtility.SampleImportMeetsMinimumVersion(k_Category, k_StarterAssetsSampleName, PackageVersionUtility.GetPackageVersion(k_XRIPackageName)),
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
                IsRuleEnabled = () => s_UIPackageAddRequest == null || s_UIPackageAddRequest.IsCompleted,
                Message = $"[{k_SampleDisplayName}] UGUI ({k_UIPackageName}) package must be installed for this sample.",
                Category = k_Category,
                CheckPredicate = () => PackageVersionUtility.IsPackageInstalled(k_UIPackageName),
                FixIt = () =>
                {
                    s_UIPackageAddRequest = Client.Add(k_UIPackageName);
                    if (s_UIPackageAddRequest.Error != null)
                    {
                        Debug.LogError($"Package installation error: {s_UIPackageAddRequest.Error}: {s_UIPackageAddRequest.Error.message}");
                    }
                },
                FixItAutomatic = true,
                Error = true,
            },
#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
            new BuildValidationRule
            {
                IsRuleEnabled = () => PackageVersionUtility.IsPackageInstalled(k_UIPackageName),
                Message = $"[{k_SampleDisplayName}] TextMesh Pro - TMP Essentials must be installed for this sample.",
                HelpText = "Can be installed using Window > TextMeshPro > Import TMP Essential Resources or by clicking this Edit button and then Import TMP Essentials in the window that appears.",
                Category = k_Category,
                CheckPredicate = () => PackageVersionUtility.IsPackageInstalled(k_UIPackageName) && TextMeshProEssentialsInstalled(),
                FixIt = () =>
                {
                    TMP_PackageResourceImporterWindow.ShowPackageImporterWindow();
                },
                FixItAutomatic = false,
                Error = true,
            },
#endif
        };

        [InitializeOnLoadMethod]
        static void RegisterProjectValidationRules()
        {
            // Delay evaluating conditions for issues to give time for Package Manager and UPM cache to fully initialize.
            EditorApplication.delayCall += AddRulesAndRunCheck;
        }

        static void AddRulesAndRunCheck()
        {
            foreach (var buildTargetGroup in s_BuildTargetGroups)
            {
                BuildValidator.AddRules(buildTargetGroup, s_BuildValidationRules);
            }

            ShowWindowIfIssuesExist();
        }

        static void ShowWindowIfIssuesExist()
        {
            foreach (var validation in s_BuildValidationRules)
            {
                if (validation.CheckPredicate == null || !validation.CheckPredicate.Invoke())
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

        static bool TextMeshProEssentialsInstalled()
        {
            // Matches logic in Project Settings window, see TMP_PackageResourceImporter.cs.
            // For simplicity, we don't also copy the check if the asset needs to be updated.
            return File.Exists("Assets/TextMesh Pro/Resources/TMP Settings.asset");
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
