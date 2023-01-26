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
    /// Really simple abstraction of a package manifest file e.g. package.json
    /// An additional field "additionalData" allows to store custom informations
    /// </summary>
    [System.Serializable]
    public class PackageManifest : ScriptableObject
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
        // All dependencies this packages require
        // Custom format: private, not automatically serialised - look at get/write methods
        private string dependencies;

        private static readonly Regex kDependenciesStartRegex = new Regex("^[ ]*\"dependencies\": .*$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex kClosingBracketRegex = new Regex("^[ ]*}.*$", RegexOptions.Compiled | RegexOptions.Singleline);

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

        public PackageDependency[] Dependencies
        {
            get
            {
                return PackageDependency.FromString(dependencies);
            }
            set
            {
                dependencies = PackageDependency.ToString(value);
            }
        }

        public static PackageManifest GetAtPath(string manifestPath)
        {
            PackageManifest result = ScriptableObject.CreateInstance<PackageManifest>();
            using (StreamReader stream = new StreamReader(manifestPath))
            {
                string data = stream.ReadToEnd();
                try
                {
                    JsonUtility.FromJsonOverwrite(data, result);
                }
                catch (System.Exception exception)
                {
                    Debug.LogError($"PackageManifest - Error on import for {manifestPath}: exception '{exception.Message}'");
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
                for (int i = startLineIdx; i < endLineIdx; ++i)
                {
                    strBuilder.AppendLine(lines[i]);
                }
                if (strBuilder.Length > 0)
                {
                    result.Dependencies = PackageDependency.FromString(strBuilder.ToString());
                }
            }
            return result;
        }

        public void WriteAtPath(string filepath)
        {
            string packageStr = JsonUtility.ToJson(this, true);
            string actualPackageStr = packageStr;
            if (Dependencies.Length > 0)
            {
                // Remove the last line after checking it
                List<string> lines = packageStr.Split('\n').ToList();
                for (int i = lines.Count - 1; i > 0; --i)
                {
                    if (kClosingBracketRegex.Match(lines[i]).Success)
                    {
                        // This is where we can insert the dependencies data
                        lines[i - 1] += ',';
                        lines.Insert(i, dependencies);
                    }
                }
                actualPackageStr = string.Join('\n', lines);
            }
            File.WriteAllText(filepath, actualPackageStr);
        }
    }
}
