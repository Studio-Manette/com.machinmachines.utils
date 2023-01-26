// Copyright 2023 MachinMachines
//
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;

namespace MachinMachines.Packages
{
    /// <summary>
    /// Really simple abstraction of a package manifest file e.g. package.json
    /// An additional field "additionalData" allows to store custom informations
    /// </summary>
    [System.Serializable]
    public class PackageManifest : PackageDependencies
    {
        // Hiding the inherited member "name", hence this "new"
        new public string name;
        public string displayName;
        // The version has to be a semantic version
        // Custom format: private
        [SerializeField]
        private string version;
        // Our own tambouille: this allows to store custom data
        // The main use is to have a decoupling so Tupac does not need to know EVERYTHING
        // that goes here - the sending and receiving systems are fully reponsible
        public string additionalData;

        public SemanticVersion SemanticVersion
        {
            get
            {
                return SemanticVersion.FromString(version);
            }
            set
            {
                version = value.ToString();
            }
        }

//#if UNITY_EDITOR
//        [UnityEditor.MenuItem("Assets/Create/MachinMachines/packageManifestTest", priority = 1000)]
//        static void CreateTestFile()
//        {
//            PackageManifest data = CreateInstance<PackageManifest>();
//            data.name = "com.machinmachines.toto";
//            data.displayName = "Machinmachines Toto";
//            data.SemanticVersion = new SemanticVersion{ major = 2, minor = 0, patch = 45 };
//            using (System.IO.StreamWriter stream = new("Assets/package_nodependencies.json"))
//            {
//                stream.Write(data.Write());
//            }
//            data.Dependencies = new PackageDependency[]
//            {
//                new PackageDependency{ packageName = "com.machinsmachines.utils", packageVersion = "1.3.54" },
//                new PackageDependency{ packageName = "com.machinsmachines.utils2", packageVersion = "1.0.11" }
//            };
//            using (System.IO.StreamWriter stream = new("Assets/package.json"))
//            {
//                stream.Write(data.Write());
//            }
//        }

//        [UnityEditor.MenuItem("Assets/MachinMachines/packageManifestRoundtrip", priority = 1000)]
//        static void Roundtrip()
//        {
//            foreach (string guid in UnityEditor.Selection.assetGUIDs)
//            {
//                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
//                if (!string.IsNullOrEmpty(assetPath) && assetPath.EndsWith(".json"))
//                {
//                    using (System.IO.StreamReader reader = new(assetPath))
//                    {
//                        PackageManifest result = PackageManifest.Read<PackageManifest>(reader.ReadToEnd());
//                        using (System.IO.StreamWriter writer = new(assetPath + "_roundtrip"))
//                        {
//                            writer.Write(result.Write());
//                        }
//                    }
//                }
//            }
//        }
//#endif  // UNITY_EDITOR
    }
}
