#if UNITY_EDITOR && SORTIFY
using UnityEditor;
using UnityEngine;

namespace Sortify
{
    [InitializeOnLoad]
    public static class SortifyInitializer
    {
        static SortifyInitializer()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (obj == null)
                return;

            SortifyDrawer.Draw(instanceID, selectionRect, obj);
        }
    }
}
#endif
