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
using System.Text.RegularExpressions;

namespace MachinMachines.Packages
{
    /// <summary>
    /// Packages versions have to be a semantic version, hence this helper class
    ///
    /// Notice that fields are actual ints so no custoom versions such as "1.0.6-dev" can be used for now
    /// </summary>
    [System.Serializable]
    public struct SemanticVersion
    {
        public int major;
        public int minor;
        public int patch;

        public static SemanticVersion FromString(string input)
        {
            string[] tokens = input.Split('.');
            return new SemanticVersion { major = int.Parse(tokens[0]), minor = int.Parse(tokens[1]), patch = int.Parse(tokens[2]) };
        }

        public override string ToString()
        {
            return $"{major}.{minor}.{patch}";
        }
    }

    /// <summary>
    /// Describes a package dependency as listed in the package manifest
    /// </summary>
    [System.Serializable]
    public class PackageDependency
    {
        public string packageName;
        public string packageVersion;

        private static readonly Regex kRegex = new Regex("\"(.*)\": \"(.*)\"", RegexOptions.Compiled | RegexOptions.Singleline);

        // For both functions below, let's take this as an example:
        // {
        //  "com.studiomanette.bob": "0.12.78",
        //  "com.machinmachines.utils": "1.3.18",
        //  "com.unity.timeline": "1.4.8",
        //  "com.unity.recorder": "3.0.3"
        // }

        public static PackageDependency[] FromString(string input)
        {
            List<PackageDependency> result = new List<PackageDependency>();
            if (!string.IsNullOrEmpty(input))
            {
                string[] lines = input.Split('\n');
                for (int i = 0; i < lines.Length; ++i)
                {
                    Match match = kRegex.Match(lines[i]);
                    if (match.Success)
                    {
                        PackageDependency packageDependency = new PackageDependency
                        {
                            packageName = match.Groups[1].Value,
                            packageVersion = match.Groups[2].Value
                        };
                        result.Add(packageDependency);
                    }
                }
            }
            return result.ToArray();
        }

        public static string ToString(PackageDependency[] input)
        {
            string result = "\"dependencies\": {\n";
            for (int i = 0; i < input.Length; ++i)
            {
                PackageDependency packageDependency = input[i];
                result += $"  \"{packageDependency.packageName}\": \"{packageDependency.packageVersion}\"";
                if (i < input.Length - 1)
                {
                    result += ",";
                }
                result += "\n";
            }
            result += "}";
            return result;
        }
    }
}
