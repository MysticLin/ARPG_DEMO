using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildRelease
{
    private const string OutputPath = @"D:\RPG\Builds\Release_v0.2\RPG.exe";

    [MenuItem("Build/Build Release v0.2")]
    public static void BuildFromMenu()
    {
        Build();
    }

    public static void Build()
    {
        string outputDir = Path.GetDirectoryName(OutputPath);
        if (!string.IsNullOrEmpty(outputDir))
            Directory.CreateDirectory(outputDir);

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = new[]
            {
                "Assets/Scenes/Boot.unity",
                "Assets/Scenes/RPG.unity",
                "Assets/Scenes/RPG2.unity"
            },
            locationPathName = OutputPath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"BUILD SUCCEEDED: {OutputPath} ({summary.totalSize} bytes)");
            if (Application.isBatchMode)
                EditorApplication.Exit(0);
        }
        else
        {
            Debug.LogError($"BUILD FAILED: {summary.result}, errors={summary.totalErrors}");
            if (Application.isBatchMode)
                EditorApplication.Exit(1);
        }
    }
}
