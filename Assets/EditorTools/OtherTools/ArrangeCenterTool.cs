#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


namespace EditorTools
{

    public static class ArrangeCenterTool
    {
        // Adds a menu item named "Arrange Center" to the Tools menu in the Unity Editor
        [MenuItem("Tools/Arrange Center")]
        public static void ArrangeCenter()
        {
            // Check if a GameObject is selected in the editor
            if (Selection.activeGameObject != null)
            {
                // Call the ArrangeCenter method on the selected GameObject
                Selection.activeGameObject.ArrangeCenter();
                Selection.activeGameObject.SaveObject_Editor();
            }
            else
            {
                // Display a warning if no GameObject is selected
                Debug.LogWarning("No GameObject selected. Please select a GameObject with children.");
            }
        }

        // This function checks if the "Arrange Center" menu item is enabled or disabled.
        // It will only be enabled when a GameObject is selected.
        [MenuItem("Tools/Arrange Center", true)]
        public static bool ArrangeCenterValidation()
        {
            // Enable the menu item only if a GameObject is selected and it has children
            return Selection.activeGameObject != null && Selection.activeGameObject.transform.childCount > 0;
        }

        private static void ArrangeCenter(this GameObject obj)
        {
            if (obj == null)
            {
                Debug.LogWarning("GameObject is null.");
                return;
            }

            if (obj.transform.childCount == 0)
            {
                Debug.LogWarning("GameObject has no children.");
                return;
            }

            Bounds bounds = new Bounds(obj.transform.GetChild(0).position, Vector3.zero);

            // Expand the bounds to include all children
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Transform child = obj.transform.GetChild(i);
                bounds.Encapsulate(child.position); // Expand the bounds to include the child's position
            }

            // The center of the bounds is now the centroid of all child objects
            Vector3 centroid = bounds.center;

            // Calculate the offset needed to move the parent to the centroid
            Vector3 offset = centroid - obj.transform.position;

            // Move the parent GameObject to the centroid
            obj.transform.position = centroid;

            // Adjust all children to keep them in the same place relative to the world
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Transform child = obj.transform.GetChild(i);
                child.position -= offset;
            }
        }

    }




}
#endif