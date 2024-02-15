using System;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils.Editor;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEditor.XR.Interaction.Toolkit.Samples
{
    /// <summary>
    /// Unity Editor class which registers Project Validation rules for the Starter Assets sample package.
    /// </summary>
    class StarterAssetsSampleProjectValidation
    {
        const string k_Category = "XR Interaction Toolkit";
        const string k_StarterAssetsSampleName = "Starter Assets";
        const string k_TeleportLayerName = "Teleport";
        const int k_TeleportLayerIndex = 31;

        static readonly BuildTargetGroup[] s_BuildTargetGroups =
            ((BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup))).Distinct().ToArray();

        static readonly List<BuildValidationRule> s_BuildValidationRules = new List<BuildValidationRule>();

        [InitializeOnLoadMethod]
        static void RegisterProjectValidationRules()
        {
            // In the Player Settings UI we have to delay the call one frame to let the settings provider get initialized
            // since we need to access the settings asset to set the rule's non-delegate properties (FixItAutomatic).
            EditorApplication.delayCall += AddRules;
        }

        static void AddRules()
        {
            if (s_BuildValidationRules.Count == 0)
            {
                s_BuildValidationRules.Add(
                    new BuildValidationRule
                    {
                        Category = k_Category,
                        Message = $"[{k_StarterAssetsSampleName}] Interaction Layer {k_TeleportLayerIndex} should be set to '{k_TeleportLayerName}' for teleportation locomotion.",
                        FixItMessage = $"XR Interaction Toolkit samples reserve Interaction Layer {k_TeleportLayerIndex} for teleportation locomotion. Set Interaction Layer {k_TeleportLayerIndex} to '{k_TeleportLayerName}' to prevent conflicts.",
                        HelpText = "Please note Interaction Layers are unique to the XR Interaction Toolkit and can be found in Edit > Project Settings > XR Plug-in Management > XR Interaction Toolkit",
                        FixItAutomatic = InteractionLayerSettings.Instance.IsLayerEmpty(k_TeleportLayerIndex) || IsInteractionLayerTeleport(),
                        Error = false,
                        CheckPredicate = IsInteractionLayerTeleport,
                        FixIt = () =>
                        {
                            if (InteractionLayerSettings.Instance.IsLayerEmpty(k_TeleportLayerIndex) || DisplayTeleportDialog())
                                InteractionLayerSettings.Instance.SetLayerNameAt(k_TeleportLayerIndex, k_TeleportLayerName);
                            else
                                SettingsService.OpenProjectSettings(XRInteractionToolkitSettingsProvider.k_SettingsPath);
                        },
                    });
            }

            foreach (var buildTargetGroup in s_BuildTargetGroups)
            {
                BuildValidator.AddRules(buildTargetGroup, s_BuildValidationRules);
            }
        }

        static bool IsInteractionLayerTeleport()
        {
            return string.Equals(InteractionLayerSettings.Instance.GetLayerNameAt(k_TeleportLayerIndex), k_TeleportLayerName, StringComparison.OrdinalIgnoreCase);
        }

        static bool DisplayTeleportDialog()
        {
            return EditorUtility.DisplayDialog(
                "Fixing Teleport Interaction Layer",
                $"Interaction Layer {k_TeleportLayerIndex} for teleportation locomotion is currently set to '{InteractionLayerSettings.Instance.GetLayerNameAt(k_TeleportLayerIndex)}' instead of '{k_TeleportLayerName}'",
                "Automatically Replace",
                "Cancel");
        }
    }
}