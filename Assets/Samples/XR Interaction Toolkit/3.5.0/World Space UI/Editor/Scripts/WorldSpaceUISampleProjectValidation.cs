using System;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils.Editor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager.UI;
using UnityEditor.XR.Interaction.Toolkit.ProjectValidation;
using UnityEngine;

#if UGUI_2_0_PRESENT && UNITY_6000_2_A9_OR_NEWER
using System.IO;
using TMPro;
#endif

namespace UnityEditor.XR.Interaction.Toolkit.Samples.WorldSpaceUI
{
    /// <summary>
    /// Unity Editor class which registers Project Validation rules for the UI Toolkit World Space UI sample,
    /// checking that required samples and packages are installed.
    /// </summary>
    static class WorldSpaceUISampleProjectValidation
    {
        const string k_SampleDisplayName = "World Space UI";
        const string k_Category = "XR Interaction Toolkit";
        const string k_StarterAssetsSampleName = "Starter Assets";
        const string k_HandsInteractionDemoSampleName = "Hands Interaction Demo";
        const string k_ProjectValidationSettingsPath = "Project/XR Plug-in Management/Project Validation";
        const string k_XRIPackageName = "com.unity.xr.interaction.toolkit";
        const string k_UIElementsName = "com.unity.modules.uielements";

        static readonly BuildTargetGroup[] s_BuildTargetGroups =
            ((BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup))).Distinct().ToArray();

        static AddRequest s_UIElementsModuleAddRequest;

        static readonly List<BuildValidationRule> s_BuildValidationRules = new List<BuildValidationRule>
        {
            new BuildValidationRule
            {
                Message = $"[{k_SampleDisplayName}] {k_SampleDisplayName} sample from XR Interaction Toolkit ({k_XRIPackageName}) package only works on Unity 6.2 (6000.2.0a9) or newer.",
                Category = k_Category,
#if UNITY_6000_2_A9_OR_NEWER
                CheckPredicate = () => true,
#else
                CheckPredicate = () => false,
#endif
                HelpText = $"To clear this error, delete the {k_SampleDisplayName} folder from the Assets/Samples/XR Interaction Toolkit/<version>/ project folder.",
                FixItAutomatic = false,
                Error = true,
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
                Message = $"[{k_SampleDisplayName}] {k_HandsInteractionDemoSampleName} sample from XR Interaction Toolkit ({k_XRIPackageName}) package must be imported or updated to use this sample. {GetImportSampleVersionMessage(k_Category, k_StarterAssetsSampleName, ProjectValidationUtility.minimumXRIStarterAssetsSampleVersion)}",
                Category = k_Category,
                CheckPredicate = () => ProjectValidationUtility.SampleImportMeetsMinimumVersion(k_Category, k_HandsInteractionDemoSampleName, ProjectValidationUtility.minimumXRIStarterAssetsSampleVersion),
                FixIt = () =>
                {
                    if (TryFindSample(k_XRIPackageName, string.Empty, k_HandsInteractionDemoSampleName, out var sample))
                    {
                        sample.Import(Sample.ImportOptions.OverridePreviousImports);
                    }
                },
                FixItAutomatic = true,
                Error = !ProjectValidationUtility.HasSampleImported(k_Category, k_HandsInteractionDemoSampleName),
            },
            new BuildValidationRule
            {
                IsRuleEnabled = () => s_UIElementsModuleAddRequest == null || s_UIElementsModuleAddRequest.IsCompleted,
                Message = $"[{k_SampleDisplayName}] UIElements ({k_UIElementsName}) module must be installed for this sample.",
                Category = k_Category,
                CheckPredicate = () => PackageVersionUtility.IsPackageInstalled(k_UIElementsName),
                FixIt = () =>
                {
                    s_UIElementsModuleAddRequest = Client.Add(k_UIElementsName);
                    if (s_UIElementsModuleAddRequest.Error != null)
                    {
                        Debug.LogError($"Package installation error: {s_UIElementsModuleAddRequest.Error}: {s_UIElementsModuleAddRequest.Error.message}");
                    }
                },
                FixItAutomatic = true,
                Error = true,
            },
#if UGUI_2_0_PRESENT && UNITY_6000_2_A9_OR_NEWER
            new BuildValidationRule
            {
                Message = $"[{k_SampleDisplayName}] TextMesh Pro - TMP Essentials must be installed for this sample.",
                HelpText = "Can be installed using Window > TextMeshPro > Import TMP Essential Resources or by clicking this Edit button and then Import TMP Essentials in the window that appears.",
                Category = k_Category,
                CheckPredicate = () => TextMeshProEssentialsInstalled(),
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

#if UGUI_2_0_PRESENT && UNITY_6000_2_A9_OR_NEWER
        static bool TextMeshProEssentialsInstalled()
        {
            // Matches logic in Project Settings window, see TMP_PackageResourceImporter.cs.
            // For simplicity, we don't also copy the check if the asset needs to be updated.
            return File.Exists("Assets/TextMesh Pro/Resources/TMP Settings.asset");
        }
#endif
    }
}
