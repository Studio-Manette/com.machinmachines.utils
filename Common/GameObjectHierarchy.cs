using System.Collections.Generic;

using UnityEngine;

namespace StudioManette
{
    namespace Utils
    {
        public static class GameObjectHierarchy
        {
            // Browse all game objects children of the given root game object (not included in the results)
            public static IEnumerable<GameObject> BrowseChildHierarchy(GameObject root)
            {
                // Skipping the root
                foreach (Transform childTransform in root.transform)
                {
                    foreach (GameObject child in BrowseChildHierarchy_r(childTransform.gameObject))
                    {
                        yield return child;
                    }
                }
            }

            // Helper of the above BrowseChildHierarchy() returning the root game object
            public static IEnumerable<GameObject> BrowseChildHierarchy_r(GameObject root)
            {
                yield return root;
                foreach (Transform childTransform in root.transform)
                {
                    foreach (GameObject child in BrowseChildHierarchy_r(childTransform.gameObject))
                    {
                        yield return child;
                    }
                }
            }

            // Get to the root of the given leaf game object (included in the results)
            public static IEnumerable<GameObject> BrowseParentHierarchy(GameObject leaf, GameObject topObject = null)
            {
                // This time the leaf is not skipped
                yield return leaf;
                Transform parentTransform = leaf.transform.parent;
                while ((parentTransform != null) && (topObject == null || parentTransform != topObject.transform))
                {
                    GameObject current = parentTransform.gameObject;
                    yield return current;
                    parentTransform = current.transform.parent;
                }
            }
        }
    }
}
