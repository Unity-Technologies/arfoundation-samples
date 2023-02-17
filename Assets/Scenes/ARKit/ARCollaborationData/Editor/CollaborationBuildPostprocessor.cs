#if UNITY_IOS
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;
using System.IO;
using System.Linq;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples;

namespace UnityEditor.XR.ARFoundation.Samples
{
    /// <summary>
    /// This build processor finds all <see cref="CollaborativeSession"/> components in the build and adds the
    /// necessary plist entries according to https://developer.apple.com/documentation/multipeerconnectivity
    /// </summary>
    class CollaborationBuildProcessor : IPostprocessBuildWithReport, IPreprocessBuildWithReport
    {
        public int callbackOrder => 2;

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            s_CollaborativeSessions = new List<CollaborativeSession>();
        }

        static PlistElementArray GetOrCreatePlistElementArray(PlistElementDict dict, string key)
        {
            var element = dict[key];
            return element == null ? dict.CreateArray(key) : element.AsArray();
        }

        /// <summary>
        /// Adds necessary plist entries required by iOS 14+.
        /// See https://developer.apple.com/documentation/multipeerconnectivity for details.
        /// </summary>
        /// <param name="report">The BuildReport provided by the post build process.</param>
        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.iOS)
                return;

            if (s_CollaborativeSessions.Count == 0)
                return;

            var plistPath = Path.Combine(report.summary.outputPath, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
            var root = plist.root;

            // The app requires permission from the user to use the local network
            if (root["NSLocalNetworkUsageDescription"] == null)
            {
                // If no entry exists, then we will add one with the prompt "Collaborative Session"
                root["NSLocalNetworkUsageDescription"] = new PlistElementString("Collaborative Session");
            }

            // Collect all the service names we need to add
            var bonjourServices = GetOrCreatePlistElementArray(root, "NSBonjourServices");
            var existingValues = new HashSet<string>(bonjourServices.values
                .Where(value => value is PlistElementString)
                .Select(value => value.AsString()));
            var valuesToAdd = new HashSet<string>();

            foreach (var serviceType in s_CollaborativeSessions
                .Select(collaborativeSession => collaborativeSession.serviceType)
                .Where(serviceType => !string.IsNullOrEmpty(serviceType)))
            {
                // Each "serviceType" must be registered as a Bonjour Service or the app will crash
                // See https://developer.apple.com/documentation/bundleresources/information_property_list/nsbonjourservices
                AddIfNecessary(existingValues, valuesToAdd, $"_{serviceType}._tcp");
                AddIfNecessary(existingValues, valuesToAdd, $"_{serviceType}._udp");
            }

            foreach (var value in valuesToAdd)
            {
                bonjourServices.AddString(value);
            }

            File.WriteAllText(plistPath, plist.WriteToString());
        }

        static void AddIfNecessary(HashSet<string> existingValues, HashSet<string> valuesToAdd, string value)
        {
            if (!existingValues.Contains(value))
                valuesToAdd.Add(value);
        }

        /// <summary>
        /// Collects all CollaborativeSession components from each scene in the final build
        /// </summary>
        [PostProcessScene]
        static void OnPostProcessScene()
        {
#if UNITY_2023_1_OR_NEWER
            foreach (var collaborativeSession in Object.FindObjectsByType<CollaborativeSession>(FindObjectsSortMode.None))
#else
            foreach (var collaborativeSession in Object.FindObjectsOfType<CollaborativeSession>())
#endif
            {
                s_CollaborativeSessions.Add(collaborativeSession);
            }
        }

        static List<CollaborativeSession> s_CollaborativeSessions = new();
    }
}
#endif
