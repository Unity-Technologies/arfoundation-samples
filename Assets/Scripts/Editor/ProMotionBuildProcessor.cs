#if UNITY_EDITOR && UNITY_IOS
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using UnityEngine;

namespace UnityEditor.XR.ARFoundation.Samples
{
    public static class ProMotionBuildProcessor
    {
        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget target, string path)
        {
            if (target != BuildTarget.iOS)
                return;

            var plistPath = path + "/Info.plist";
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            plist.root.SetBoolean("CADisableMinimumFrameDurationOnPhone", true);

            File.WriteAllText(plistPath, plist.WriteToString());

            Debug.Log("[ProMotionBuildProcessor] Successfully enabled 120Hz ProMotion in Info.plist!");
        }
    }
}
#endif
