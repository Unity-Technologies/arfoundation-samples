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
    class Processor : IPostprocessBuildWithReport, IPreprocessBuildWithReport
    {
        public int callbackOrder => 2;

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            s_CollaborativeSessions = new List<CollaborativeSession>();
        }

        PlistElementArray GetOrCreatePlistElementArray(PlistElementDict dict, string key)
        {
            var element = dict[key];
            return element == null ? dict.CreateArray(key) : element.AsArray();
        }

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

            if (root["NSLocalNetworkUsageDescription"] == null)
            {
                root["NSLocalNetworkUsageDescription"] = new PlistElementString("Collaborative Session");
            }

            // Get all the service names
            var bonjourServices = GetOrCreatePlistElementArray(root, "NSBonjourServices");
            var existingValues = new HashSet<string>(bonjourServices.values
                .Where(value => value is PlistElementString)
                .Select(value => value.AsString()));
            var valuesToAdd = new HashSet<string>();

            foreach (var serviceType in s_CollaborativeSessions
                .Select(collaborativeSession => collaborativeSession.serviceType)
                .Where(serviceType => !string.IsNullOrEmpty(serviceType)))
            {
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

        [PostProcessScene]
        static void OnPostProcessScene()
        {
            foreach (var collaborativeSession in Object.FindObjectsOfType<CollaborativeSession>())
            {
                s_CollaborativeSessions.Add(collaborativeSession);
            }
        }

        static List<CollaborativeSession> s_CollaborativeSessions = new List<CollaborativeSession>();
    }
}
#endif
