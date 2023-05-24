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

using System.Collections.Generic;
using System.IO;

using UnityEngine.Profiling;

using MachinMachines.Utils;

namespace MachinMachines.Packages
{
    /// <summary>
    /// An abstraction for packages manifest (e.g. manifest.json)
    /// </summary>
    [System.Serializable]
    public class Manifest : PackageDependencies
    {
        // Not handling scoped registries yet: they will be lost during a roundtrip

        /// <summary>
        /// Create a similar manifest but with local packages replaced with their version
        /// </summary>
        public Manifest CreatePinnedVersionsManifest()
        {
            Profiler.BeginSample("MachinMachines - Package - CreateVersionedManifest");

            Manifest result = Instantiate(this);
            List<PackageDependency> dependencies = new(Dependencies.Length);
            foreach (var dependency in Dependencies)
            {
                if (!string.IsNullOrEmpty(dependency.packageVersion))
                {
                    int intTest;
                    if (!int.TryParse(dependency.packageVersion.Substring(0, 1), out intTest))
                    {
                        // Not an actual version number: extract it from the package manifest
                        string packagePath = Paths.GetPackagePath(dependency.packageName, false);
                        string packageManifestPath = Path.Combine(packagePath, "package.json");
                        PackageManifest manifest = null;
                        using (StreamReader stream = new StreamReader(packageManifestPath))
                        {
                            string data = stream.ReadToEnd();
                            manifest = PackageManifest.Read<PackageManifest>(data);
                        }
                        if (manifest != null)
                        {
                            PackageDependency packageDependency = new PackageDependency
                            {
                                packageName = dependency.packageName,
                                packageVersion = manifest.SemanticVersion.ToString()
                            };
                            dependencies.Add(packageDependency);
                        }
                    }
                    else
                    {
                        // Regular case, nothing special here
                        dependencies.Add(dependency);
                    }
                }
            }
            result._dependenciesStr = PackageDependency.ToString(dependencies);

            Profiler.EndSample();
            return result;
        }

//#if UNITY_EDITOR
//        [UnityEditor.MenuItem("Assets/Create/MachinMachines/manifestTest", priority = 1000)]
//        static void CreateTestFile()
//        {
//            Manifest data = CreateInstance<Manifest>();
//            data.Dependencies = new PackageDependency[]
//            {
//                new PackageDependency{ packageName = "com.machinsmachines.utils", packageVersion = "1.3.54" },
//                new PackageDependency{ packageName = "com.machinsmachines.utils2", packageVersion = "1.0.11" }
//            };
//            using (StreamWriter stream = new("Assets/manifest.json"))
//            {
//                stream.Write(data.Write());
//            }
//        }

//        [UnityEditor.MenuItem("Assets/MachinMachines/manifestRoundtrip", priority = 1000)]
//        static void Roundtrip()
//        {
//            foreach (string guid in UnityEditor.Selection.assetGUIDs)
//            {
//                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
//                if (!string.IsNullOrEmpty(assetPath) && assetPath.EndsWith("manifest.json"))
//                {
//                    using (StreamReader reader = new(assetPath))
//                    {
//                        Manifest result = Manifest.Read<Manifest>(reader.ReadToEnd());
//                        using (StreamWriter writer = new(assetPath + "_roundtrip"))
//                        {
//                            writer.Write(result.Write());
//                        }
//                    }
//                }
//            }
//        }

//        [UnityEditor.MenuItem("Assets/MachinMachines/manifestFromLocalToVersion", priority = 1000)]
//        static void FromLocalToVersion()
//        {
//            foreach (string guid in UnityEditor.Selection.assetGUIDs)
//            {
//                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
//                if (!string.IsNullOrEmpty(assetPath) && assetPath.EndsWith("manifest.json"))
//                {
//                    using (StreamReader reader = new(assetPath))
//                    {
//                        Manifest baseManifest = Manifest.Read<Manifest>(reader.ReadToEnd());
//                        Manifest result = baseManifest.CreatePinnedVersionsManifest();
//                        using (StreamWriter writer = new(assetPath + "_versioned"))
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
