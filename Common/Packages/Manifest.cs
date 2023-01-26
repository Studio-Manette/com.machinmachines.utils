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

namespace MachinMachines.Packages
{
    /// <summary>
    /// An abstraction for packages manifest (e.g. manifest.json)
    /// </summary>
    [System.Serializable]
    public class Manifest : PackageDependencyHolder
    {
        // Not handling scoped registries yet: they will be lost during a roundtrip

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
//            using (System.IO.StreamWriter stream = new("Assets/manifest.json"))
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
//                    using (System.IO.StreamReader reader = new(assetPath))
//                    {
//                        Manifest result = Manifest.Read<Manifest>(reader.ReadToEnd());
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
