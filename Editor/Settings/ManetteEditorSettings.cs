using UnityEditor;
using UnityEngine;

namespace StudioManette
{
    namespace Utils
    {
        namespace Settings
        {
            public class ManetteEditorSettings : SettingsBase<ManetteEditorSettings>
            {
                [Header("General paths")]

                [IsFilepath(false, "", IsFilepathAttribute.FileType.Directory)]
                [Tooltip("'Local folder' (within the episode project assets hierarchy) where we store temporary data pulled by the description files")]
                public string LocalEpisodeFolder = "Assets/Local";

                [Header("Anim importer")]
                public bool RemoveFBXAfterImport = true;
                public bool OverwriteExistingFiles = false;
                public bool StoreBinarizedAssets = true;
                [IsFilepath(false, "", IsFilepathAttribute.FileType.Directory)]
                public string AnimationsAssetsFolder = "Assets/_Anim/Clips";
                [IsFilepath(false, "", IsFilepathAttribute.FileType.Directory)]
                public string FBXImportFolder = "Assets/_Anim/SourceFBX";
                public AnimationUtility.TangentMode TangentMode = AnimationUtility.TangentMode.Constant;

                [Space(32)]

                [Header("FBX importer")]
                [SerializeField]
                public bool EnableFBXPostProcessorChecks = false;
            }
        }
    }
}
