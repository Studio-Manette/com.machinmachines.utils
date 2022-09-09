using UnityEngine;

namespace StudioManette
{
    namespace Utils
    {
        namespace Settings
        {
            public class ProdSettings : SettingsBase<ProdSettings>
            {
                [Header("Production")]

                [Tooltip("Project short initials")]
                public string ProductionShortName = "SMJ";

                [Tooltip("Sequence prefix for folder/file names")]
                public string ProductionSequencePrefixName = "s";

                [Tooltip("Shot prefix for folder/file names")]
                public string ProductionShotPrefixName = "p";

                [Tooltip("Animation framerate")]
                public float Framerate = 25.0f;
            }
        }
    }
}
