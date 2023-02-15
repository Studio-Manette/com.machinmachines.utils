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

using MachinMachines.Utils;

using UnityEngine;

namespace MachinMachines.Common.Repository
{
    public enum RepositoryUsage
    {
        Texture = 0,
        Material = 1
    }

    /// <summary>
    /// This helper class manages a list of "files repository",
    /// ie locations on disk to look for when looking for partial file paths
    /// </summary>
    [System.Serializable]
    public class FilesRepository : ScriptableObject
    {
        public RepositoryUsage Usage;

        /// <summary>
        /// A file repository is a simple absolute path and a priority
        /// </summary>
        [System.Serializable]
        public class Repository
        {
            [IsFilepath(false, "")]
            public string Path;
            public int Priority;
        }

        /// <summary>
        /// All repositories are stored here and always maintained sorted by priority order
        /// </summary>
        public Repository[] _repositories;

        private void Awake()
        {
            // Register itself into the main manager
            FilesRepositoryManager.Instance.AddRepository(this);
        }

        //#if UNITY_EDITOR
        //                [UnityEditor.MenuItem("Assets/Create/Studio Manette/FileRepositoryTest")]
        //                static void CreateTestFile()
        //                {
        //                    FilesRepository data = CreateInstance<FilesRepository>();
        //                    data.Usage = RepositoryUsage.Material;
        //                    data._repositories = new Repository[] { new Repository { Path = "TOTO", Priority = 1 } };
        //                    using (System.IO.StreamWriter stream = new StreamWriter("Assets/toto.filesrepository"))
        //                    {
        //                        stream.Write(JsonUtility.ToJson(data, true));
        //                    }
        //                }
        //#endif
    }
}
