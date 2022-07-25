using System.IO;
using System.Reflection;

using UnityEditor;
using UnityEngine;

namespace StudioManette
{
    namespace Utils
    {
        // Abstract base class for a class of JSON settings that can be used both in-editor or ingame
        // Stored in Streaming Assets so the JSON can be edited manually later on
        //
        // This way, child classes only need to hold the actual settings data and nothing else
        abstract public class SettingsBase<T> : ScriptableObject where T : ScriptableObject
        {
            // All settings are stored in the same folder, with the file named after their class
            public static readonly string kSettingsPath = Path.Combine(Application.streamingAssetsPath, $"ManetteSettings/{typeof(T).Name}.json");

            private static T _instance = null;

            // Static getter for retrieving the instance
            // If no instance is loaded already it does so by reading the JSON file
            // If no file can be found it writes it
            public static T GetOrCreateSettings()
            {
                if (_instance == null)
                {
                    _instance = ScriptableObject.CreateInstance<T>();
                    string directory = Path.GetDirectoryName(kSettingsPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    if (File.Exists(kSettingsPath))
                    {
                        JsonUtility.FromJsonOverwrite(File.ReadAllText(kSettingsPath), _instance);
                    }
                    // We always write the file in case the base ScriptableObject gets updated from the code
                    File.WriteAllText(kSettingsPath, JsonUtility.ToJson(_instance, true));
#if UNITY_EDITOR
                        // This is only useful so the file appears immediately in the projet browser
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
#endif  // UNITY_EDITOR
                    CreateFolders(_instance);
                }
                return _instance;
            }

            // Tiny helper for settings marked with the "IsFilepath" attribute:
            // it automagically creates all referenced folders
            private static void CreateFolders(T settings)
            {
                foreach (FieldInfo info in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    foreach (CustomAttributeData attributeData in info.CustomAttributes)
                    {
                        if (attributeData.AttributeType == typeof(IsFilepathAttribute))
                        {
                            string propertyValue = info.GetValue(settings) as string;
                            if (!string.IsNullOrEmpty(propertyValue))
                            {
                                // Make sure reference folders exist
                                if (!Directory.Exists(propertyValue))
                                {
                                    // Make sure the drive exists before trying to create it
                                    string pathRoot = Path.GetPathRoot(propertyValue);
                                    if(string.IsNullOrEmpty(pathRoot))
                                    {
                                        if (propertyValue.StartsWith("Assets", System.StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            // If we are trying to create some "assets/***" subfolder within a runtime build, just stop
                                            if (!Application.isEditor)
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                    if (string.IsNullOrEmpty(pathRoot) || Directory.Exists(pathRoot))
                                    {
                                        // This might fail for various reasons (typically access rights),
                                        // so better make sure we catch the potential exception
                                        try
                                        {
                                            Directory.CreateDirectory(propertyValue);
                                        }
                                        catch
                                        {
                                            Debug.LogWarning($"No access rights to create folder {propertyValue}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
