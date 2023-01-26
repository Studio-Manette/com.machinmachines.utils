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
    public class PackageManifest : PackageDependencyHolder
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
        // Custom format: private, not automatically serialised
        private string dependencies;

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

        public override PackageDependency[] Dependencies
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
    }
}
