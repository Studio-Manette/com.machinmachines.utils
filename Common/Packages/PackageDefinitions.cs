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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.Profiling;

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

    /// <summary>
    /// Base class for scriptable objects with package dependencies field:
    /// manifest.json, package.json
    ///
    /// It handles serialising package dependencies within a regularly,
    /// JSON serialised scriptable object
    /// </summary>
    public abstract class PackageDependencyHolder : ScriptableObject
    {
        // Custom format: private, not automatically serialised
        private string _dependenciesStr;
        // Cache of the above
        private PackageDependency[] _dependencies = null;

        /// <summary>
        /// All listed dependencies
        /// </summary>
        public PackageDependency[] Dependencies
        {
            get
            {
                if (_dependencies == null)
                {
                    _dependencies = PackageDependency.FromString(_dependenciesStr);
                }
                return _dependencies;
            }
            set
            {
                _dependenciesStr = PackageDependency.ToString(value);
            }
        }

        private static readonly Regex kDependenciesStartRegex = new Regex("^[ ]*\"dependencies\": .*$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex kClosingBracketRegex = new Regex("^[ ]*}.*$", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// Custom serialisation from a JSON string
        /// </summary>
        static public T Read<T>(string data) where T : PackageDependencyHolder
        {
            Profiler.BeginSample("MachinMachines - Package - Read");
            T result = ScriptableObject.CreateInstance<T>();
            try
            {
                JsonUtility.FromJsonOverwrite(data, result);
            }
            catch (System.Exception exception)
            {
                Debug.LogError($"PackageDependency - Error on import for {data}: exception '{exception.Message}'");

                Profiler.EndSample();
                return result;
            }
            string[] lines = data.Split('\n');
            int startLineIdx = 0;
            int endLineIdx = 0;
            for (int i = 0; i < lines.Length; ++i)
            {
                if (kDependenciesStartRegex.Match(lines[i]).Success)
                {
                    startLineIdx = i;
                    // Now look till the end of this section
                    for (int j = i; j < lines.Length; ++j)
                    {
                        if (kClosingBracketRegex.Match(lines[j]).Success)
                        {
                            endLineIdx = j + 1;
                            break;
                        }
                    }
                    break;
                }
            }
            StringBuilder strBuilder = new StringBuilder();
            for (int i = startLineIdx; i < endLineIdx - 1; ++i)
            {
                strBuilder.AppendLine(lines[i]);
            }
            // Strip potential trailing comma
            strBuilder.AppendLine(lines[endLineIdx - 1].Trim(','));

            if (strBuilder.Length > 0)
            {
                // This is considered as the only entry point for this field
                result._dependenciesStr = strBuilder.ToString();
            }

            Profiler.EndSample();
            return result;
        }

        /// <summary>
        /// Custom serialisation to a JSON string
        /// </summary>
        public string Write()
        {
            Profiler.BeginSample("MachinMachines - Package - Write");

            string packageStr = JsonUtility.ToJson(this, true);
            string result;
            if (!string.IsNullOrEmpty(_dependenciesStr))
            {
                // Remove the last line after checking it
                List<string> lines = packageStr.Split('\n').ToList();
                bool shouldAddComma = true;
                // Handling the special case for an empty structure
                if (lines.Count == 1)
                {
                    if (lines[0] == "{}")
                    {
                        lines[0] = "{\n";
                        lines.Add("}\n");
                        shouldAddComma = false;
                    }
                }
                for (int i = lines.Count - 1; i > 0; --i)
                {
                    if (kClosingBracketRegex.Match(lines[i]).Success)
                    {
                        if (shouldAddComma)
                        {
                            lines[i - 1] += ',';
                        }
                        // This is where we can insert the dependencies data
                        lines.Insert(i, _dependenciesStr);
                    }
                }
                result = string.Join('\n', lines);
            }
            else
            {
                result = packageStr;
            }

            Profiler.EndSample();
            return result;
        }
    }
}
