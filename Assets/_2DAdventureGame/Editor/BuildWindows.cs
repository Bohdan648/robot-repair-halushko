using System;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BuildWindows
{
    private const string Version = "1.0.0";
    private const string Executable = "AvariinyiDvir.exe";
    private static readonly string[] Scenes =
    {
        "Assets/_2DAdventureGame/Scenes/StartScene.unity",
        "Assets/_2DAdventureGame/Scenes/MyLevel.unity",
        "Assets/_2DAdventureGame/Scenes/MainScene.unity"
    };

    [MenuItem("Robot Repair/Build Windows ZIP")]
    public static void Build()
    {
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("Збірка Windows", "Спочатку вимкни Play.", "OK");
            return;
        }
        if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64)
            || EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64)
        {
            EditorUtility.DisplayDialog("Збірка Windows",
                "У File > Build Profiles обери Windows і натисни Switch Platform. " +
                "Якщо Windows недоступний, додай Windows Build Support через Unity Hub.", "OK");
            return;
        }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        string parent = EditorUtility.OpenFolderPanel("Папка для збірки — поза проєктом Unity", "", "");
        if (string.IsNullOrEmpty(parent))
        {
            return;
        }
        string project = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        parent = Path.GetFullPath(parent);
        if (parent.Equals(project, StringComparison.OrdinalIgnoreCase)
            || parent.StartsWith(project + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
        {
            EditorUtility.DisplayDialog("Потрібна інша папка",
                "Обери місце поза папкою проєкту, наприклад Документи/RobotRepairBuilds.", "OK");
            return;
        }

        foreach (string scene in Scenes)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scene) == null)
            {
                EditorUtility.DisplayDialog("Немає сцени", "Перенеси файл: " + scene, "OK");
                return;
            }
        }

        EditorBuildSettings.scenes = Array.ConvertAll(Scenes, scene => new EditorBuildSettingsScene(scene, true));
        PlayerSettings.companyName = "Bohdan648";
        PlayerSettings.productName = "Аварійний двір";
        PlayerSettings.bundleVersion = Version;
        PlayerSettings.defaultScreenWidth = 1280;
        PlayerSettings.defaultScreenHeight = 720;
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.resizableWindow = false;
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
        AssetDatabase.SaveAssets();

        // Кожна спроба має свою папку; наявні збірки не видаляються.
        string basePath = Path.Combine(parent, "AvariinyiDvir-v" + Version + "-Windows");
        string buildPath = basePath;
        for (int number = 2;
            Directory.Exists(buildPath) || File.Exists(buildPath)
            || File.Exists(buildPath + ".zip") || Directory.Exists(buildPath + ".zip"); number++)
        {
            buildPath = basePath + "-" + number;
        }

        try
        {
            Directory.CreateDirectory(buildPath);
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = Scenes,
                locationPathName = Path.Combine(buildPath, Executable),
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };
            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report == null || report.summary.result != BuildResult.Succeeded)
            {
                EditorUtility.DisplayDialog("Збірка не завершена",
                    "ZIP не створено. Перевір червоні повідомлення в Console.", "OK");
                return;
            }

            string archive = buildPath + ".zip";
            ZipFile.CreateFromDirectory(buildPath, archive, System.IO.Compression.CompressionLevel.Optimal, true);
            EditorUtility.RevealInFinder(archive);
            EditorUtility.DisplayDialog("Збірка і ZIP готові",
                "Закрий Unity, розпакуй ZIP у нову папку й запусти " + Executable +
                ". Перевір звуки, здоров’я, двері, перемогу, поразку та перезапуск.\n\n" + archive, "OK");
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorUtility.DisplayDialog("Не вдалося зібрати або запакувати гру", exception.Message, "OK");
        }
    }
}
