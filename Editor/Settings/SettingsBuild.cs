using System;
using System.IO;

using UnityEngine;

using UnityEditor.Build;
using UnityEditor.Build.Reporting;

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

            // Prebuild step to make sure default assets are copied to StreamingAssets/
            public class SettingsPreBuild : IPreprocessBuildWithReport
            {
                public int callbackOrder { get { return 0; } }

                void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
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
                        Debug.LogError($"Something went wrong when copying settings paths: {e.Message}");
                    }
                }
            }

            // Postbuild step to make sure StreamingAssets is being emptied
            public class SettingsPostBuild : IPostprocessBuildWithReport
            {
                public int callbackOrder { get { return 0; } }

                void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
                {
                    try
                    {
                        foreach ((string _, string runtimePath) in SettingsPaths.s_Paths)
                        {
                            if (File.Exists(runtimePath))
                            {
                                File.Delete(runtimePath);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Something went wrong when deleting StreamingAssets directory after the build: {e.Message}");
                    }
                }
            }
        }
    }
}
