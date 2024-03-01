using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif // UNITY_IOS

public class CustomPostBuildProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }
    public void OnPostprocessBuild(BuildReport report)
    {
#if UNITY_IOS
        // Add -ld_classic to OTHER_LDFLAGS to avoid Xcode 15 build error
        // https://forum.unity.com/threads/project-wont-build-using-xode15-release-candidate.1491761/
        // https://developer.apple.com/documentation/xcode-release-notes/xcode-15-release-notes#Linking

        string projectPath = PBXProject.GetPBXProjectPath(report.summary.outputPath);

        PBXProject pbxProject = new();
        pbxProject.ReadFromFile(projectPath);

        string target = pbxProject.GetUnityFrameworkTargetGuid();

        pbxProject.AddBuildProperty(target, "OTHER_LDFLAGS", "-ld_classic");
        pbxProject.WriteToFile(projectPath);
#endif // UNITY_IOS
    }
}