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

using MachinMachines.Algorithms;

using UnityEditor.IMGUI.Controls;

using UnityEngine;

namespace MachinMachines.Utils
{
    /// <summary>
    /// Generic tree view for easy visualisation of graph items
    /// </summary>
    class GenericHierarchicalTreeViewItem : TreeViewItem
    {
        public GenericHierarchicalTreeViewItem(int id, int depth, string displayName)
            : base(id, depth, displayName)
        {
        }

        public string propertyPath { get; set; }
    }

    /// <summary>
    /// Generic tree view for easy visualisation of graph items
    /// </summary>
    public class GenericHierarchicalTreeView<T> : TreeView where T : class
    {
        private HierarchicalTreeItem<T> root_;
        private int id_;

        public GenericHierarchicalTreeView(HierarchicalTreeItem<T> root,
                                           TreeViewState state)
            : base(state)
        {
            root_ = root;
        }

        protected override TreeViewItem BuildRoot()
        {
            Debug.Assert(root_ != null, "Missing a valid root");

            var newItem = new GenericHierarchicalTreeViewItem
            (
                0,
                -1,
                root_.Name
            );
            id_ = 1;
            AddTreeViewItem_r(newItem,
                              root_,
                              ref id_);

            return newItem;
        }

        void AddTreeViewItem_r(TreeViewItem parentItem,
                               HierarchicalTreeItem<T> hierarchicalTreeItem,
                               ref int id)
        {
            var newItem = new GenericHierarchicalTreeViewItem
            (
                id++,
                parentItem.depth + 1,
                hierarchicalTreeItem.Name
            );
            foreach (var child in hierarchicalTreeItem.children)
            {
                AddTreeViewItem_r(newItem, child, ref id);
            }
            parentItem.AddChild(newItem);
        }
    }
}
