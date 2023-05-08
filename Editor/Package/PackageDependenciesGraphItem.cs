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

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

using UnityEngine.Profiling;

using MachinMachines.Algorithms;
using MachinMachines.EditorTools;

namespace MachinMachines.Packages
{
    /// <summary>
    /// Package dependencies organised into a graph
    /// </summary>
    [Serializable]
    public class PackageDependenciesGraphItem : HierarchicalTreeItem<PackageDependenciesGraphItem>
    {
        // Used below for packages collection requests
        private struct PendingOperation
        {
            public ListRequest Request;
            public Action<PackageCollection> OnCompletionCallback;
        }

        public string Item;

        public override string Name
        {
            get
            {
                return string.IsNullOrEmpty(Item) ? "" : Item;
            }
        }

        /// <summary>
        /// Main tree creation function
        /// Async, the completion callback will be called when it is done
        /// </summary>
        public static void BuildChildrenTree(PackageDependencies data,
                                             Action<PackageDependenciesGraphItem> onCompletionCallback,
                                             bool useOfflineMode = true)
        {
            // Trigger retrieval of all packages dependencies
            ListRequest request = Client.List(useOfflineMode, true);
            DelayedProcess<PendingOperation>.onNextUpdate = OnListPackagesCallback;
            DelayedProcess<PendingOperation>.Push(new PendingOperation
            {
                Request = request,
                OnCompletionCallback = packageCollection => BuildChildrenTree(packageCollection, data, onCompletionCallback)
            });
        }

        /// <summary>
        /// Our internal callback for regularly checking if the packages listing is done
        /// </summary>
        private static void OnListPackagesCallback(PendingOperation pending)
        {
            if (pending.Request != null)
            {
                if (pending.Request.IsCompleted)
                {
                    pending.OnCompletionCallback?.Invoke(pending.Request.Result);
                }
            }
            else
            {
                // Re-trigger it
                DelayedProcess<PendingOperation>.Push(pending);
            }
        }

        private static void BuildChildrenTree(PackageCollection packageCollection,
                                              PackageDependencies data,
                                              Action<PackageDependenciesGraphItem> onCompletionCallback)
        {
            Profiler.BeginSample("MachinMachines - PackageDependencies - BuildChildrenTree");

            HashSet<string> listedDependencies = new(2 * data.Dependencies.Length);
            PackageDependenciesGraphItem[] children = data.Dependencies
                .Select(item => BuildChildrenTree_r(packageCollection,
                                                    item.packageName,
                                                    ref listedDependencies))
                .ToArray();

            PackageDependenciesGraphItem result = new PackageDependenciesGraphItem
            {
                Item = data.name,
                children = children
            };

            Profiler.EndSample();
            onCompletionCallback?.Invoke(result);
        }

        /// <summary>
        /// Actual function building the graph
        /// </summary>
        /// <param name="packageCollection"></param>
        /// <param name="data"></param>
        private static PackageDependenciesGraphItem BuildChildrenTree_r(PackageCollection packageCollection,
                                                                        string packageName,
                                                                        ref HashSet<string> listedDependencies)
        {
            PackageDependenciesGraphItem result = new PackageDependenciesGraphItem
            {
                Item = packageName
            };
            IEnumerable<DependencyInfo> dependencies = packageCollection.Where(item => item.name == packageName)
                                                                        .SelectMany(item => item.dependencies);
            List<PackageDependenciesGraphItem> children = new(dependencies.Count());
            foreach (DependencyInfo dependency in dependencies)
            {
                if (!listedDependencies.Contains(dependency.name))
                {
                    listedDependencies.Add(dependency.name);
                    children.Add(BuildChildrenTree_r(packageCollection, dependency.name, ref listedDependencies));
                }
            }
            result.children = children.ToArray();

            return result;
        }
    }
}
