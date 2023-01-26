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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using UnityEngine;

namespace MachinMachines.Packages
{
    /// <summary>
    /// An abstraction for packages manifest (e.g. manifest.json)
    /// </summary>
    [System.Serializable]
    public class Manifest : PackageDependencyHolder
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/MachinMachines/manifestTest", priority = 1000)]
        static void CreateTestFile()
        {
            Manifest data = CreateInstance<Manifest>();
            data.Dependencies = new PackageDependency[]
            {
                new PackageDependency{ packageName = "com.machinsmachines.utils", packageVersion = "1.3.54" },
                new PackageDependency{ packageName = "com.machinsmachines.utils2", packageVersion = "1.0.11" }
            };
            using (StreamWriter stream = new StreamWriter("Assets/manifest.json"))
            {
                stream.Write(data.Write<Manifest>());
            }
        }
#endif
    }
}
