using UnityEngine;
using UnityEditor;
using System.IO;

public class SetNativePluginsCPU : EditorWindow
{
    [MenuItem("Tools/Set .so CPU to ARM64")]
    public static void SetCPUForAllNativePlugins()
    {
        string[] pluginPaths = Directory.GetFiles("Assets/Ros2ForUnity/Plugins/Android", "*.so", SearchOption.AllDirectories);
        int count = 0;

        foreach (string path in pluginPaths)
        {
            PluginImporter importer = AssetImporter.GetAtPath(path.Replace("\\", "/")) as PluginImporter;

            if (importer != null)
            {
                importer.ClearSettings();
                importer.SetCompatibleWithAnyPlatform(false);
                importer.SetCompatibleWithPlatform(BuildTarget.Android, true);
                importer.SetPlatformData(BuildTarget.Android, "CPU", "ARM64");

                importer.SaveAndReimport();
                count++;
            }
        }

        Debug.Log($"✅ 設定完了: {count} 個の .so ファイルを ARM64 に設定しました。");
    }
}
