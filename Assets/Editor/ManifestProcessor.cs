using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Callbacks;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;

// ビルドに.vrmanifestファイルを含める
public class ManifestProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{

    public void OnPreprocessBuild(BuildReport report)
    {
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        File.Copy("Assets/manifest.vrmanifest", Path.Combine(Path.GetDirectoryName(report.summary.outputPath), "manifest.vrmanifest"), true);
        File.Copy("Assets/actions.json", Path.Combine(Path.GetDirectoryName(report.summary.outputPath), "actions.json"), true);
        File.Copy("Assets/vrc_friend_helper_oculus.json", Path.Combine(Path.GetDirectoryName(report.summary.outputPath), "vrc_friend_helper_oculus.json"), true);
        File.Copy("Assets/vrc_friend_helper_knuckles.json", Path.Combine(Path.GetDirectoryName(report.summary.outputPath), "vrc_friend_helper_knuckles.json"), true);
        File.Copy("Assets/friends.csv", Path.Combine(Path.GetDirectoryName(report.summary.outputPath), "friends.csv"), true);
        File.Copy("Assets/VRC Friend Helper Desktop Mode.bat", Path.Combine(Path.GetDirectoryName(report.summary.outputPath), "VRC Friend Helper Desktop Mode.bat"), true);
        File.Copy("Assets/Uninstall.bat", Path.Combine(Path.GetDirectoryName(report.summary.outputPath), "Uninstall.bat"), true);
    }

    // 実行順
    public int callbackOrder { get { return 0; } }
}
