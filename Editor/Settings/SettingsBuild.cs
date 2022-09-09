using System;
using System.IO;

using UnityEngine;

using UnityEditor;

namespace StudioManette
{
    namespace Utils
    {
        namespace Settings
        {
            // Just a plain and stupid way to store file paths for settings we are interested to copy over
            // Notice that editor settings are not in there
            // Also, if ever a new settings class is being created, you will need to add it here manually
            public static class SettingsPaths
            {
                internal static readonly (string, string)[] s_Paths = new (string, string)[] {
                    (ManetteSettings.kEditorSettingsPath, ManetteSettings.kRuntimeSettingsPath),
                    (ProdSettings.kEditorSettingsPath, ProdSettings.kRuntimeSettingsPath)
                };
            }

            // Autolaunched method to make sure settings files are copied to StreamingAssets/
            public class SettingsPreBuild
            {
                [InitializeOnLoadMethod]
                static void AutoloadPresets()
                {
                    try
                    {
                        if (!Directory.Exists(Application.streamingAssetsPath))
                        {
                            Directory.CreateDirectory(Application.streamingAssetsPath);
                        }
                        foreach ((string editorPath, string runtimePath) in SettingsPaths.s_Paths)
                        {
                            string destFolder = Path.GetDirectoryName(runtimePath);
                            if (!Directory.Exists(destFolder))
                            {
                                Directory.CreateDirectory(destFolder);
                            }
                            File.Copy(editorPath, runtimePath, true);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Something went wrong when copying settings files: {e.Message}");
                    }
                }
            }
        }
    }
}
