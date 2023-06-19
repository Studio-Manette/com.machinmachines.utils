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

using UnityEngine;

namespace MachinMachines.GameObjectHierarchy
{
    public static class GameObjectVisitor
    {
        /// <summary>
        /// Recursively visit every child game object of the given root,
        /// calling "func" for every one of them
        /// </summary>
        public static void Visit_r(GameObject root,
                                   Action<GameObject, object> func,
                                   object payload,
                                   bool includeRoot = true)
        {
            foreach (GameObject go in GameObjectHierarchyOperations.BrowseChildHierarchy(root, includeRoot))
            {
                func(go, payload);
            }
        }

        /// <summary>
        /// Recursively visit every child game object of the given root,
        /// calling "func" for every one of them,
        /// unless invoking "func" yields "false":
        /// in that case there is no recursion into the children but move on to the next sibling
        /// </summary>
        public static void VisitAllowEarlyOut_r(GameObject root,
                                                Func<GameObject, object, bool> func,
                                                object payload,
                                                bool includeRoot = true)
        {
            foreach (GameObject go in GameObjectHierarchyOperations.BrowseChildHierarchy(root, includeRoot))
            {
                func(go, payload);
            }
        }

        /// <summary>
        /// Recursively visit every child game object of both given roots,
        /// making sure to "get down one generation" in both hierarchies at the same time
        /// if one generation has more members then the default "null" will be used
        /// to fill up the other side
        /// Same if one side goes deeper than the other
        /// </summary>
        public static void ZipVisit_r(GameObject lhsRoot,
                                      GameObject rhsRoot,
                                      Action<GameObject, GameObject, object> func,
                                      object payload)
        {
            // Early exit in case one side has no match
            if (lhsRoot == null || rhsRoot == null)
            {
                return;
            }
            // Visit both root
            func(lhsRoot, rhsRoot, payload);
            // Iterate over both sets of children
            // Storing both sets of children for later re-iteration
            var children = new List<(GameObject, GameObject)>(lhsRoot.transform.childCount);
            int childCount = Math.Max(lhsRoot.transform.childCount, rhsRoot.transform.childCount);
            for (int childIdx = 0; childIdx < childCount; ++childIdx)
            {
                GameObject currentLHS = lhsRoot.transform.childCount <= childIdx
                                        ? null
                                        : lhsRoot.transform.GetChild(childIdx).gameObject;
                GameObject currentRHS = rhsRoot.transform.childCount <= childIdx
                                        ? null
                                        : rhsRoot.transform.GetChild(childIdx).gameObject;
                func(currentLHS, currentRHS, payload);
                children.Add((currentLHS, currentRHS));
            }
            // Re-iterate, this time for recursion into the level below
            foreach (var item in children)
            {
                ZipVisit_r(item.Item1, item.Item2, func, payload);
            }
        }
    }
}
