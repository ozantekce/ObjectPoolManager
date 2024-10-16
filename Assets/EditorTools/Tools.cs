#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;


namespace EditorTools
{

    public static class Tools
    {

        public static bool IsSceneObject_Editor(this Component cmp) => cmp != null && IsSceneObject_Editor(cmp.gameObject);
        public static bool IsSceneObject_Editor(this GameObject obj)
        {
            if (obj == null)
            {
                return false; // Null reference is not a scene object
            }

            // Check if the GameObject's scene is loaded (which means it is part of an active scene)
            if (!obj.scene.IsValid() || !obj.scene.isLoaded)
            {
                return false; // If the scene is not valid or not loaded, it's definitely not part of the active scene hierarchy
            }

            // Check for objects in Prefab Mode (editing a prefab asset in isolation)
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null && prefabStage.IsPartOfPrefabContents(obj))
            {
                return false; // The object is being edited in Prefab Mode, not part of the active scene
            }

            // If the object is part of a prefab asset but not instantiated in the scene, it's not a regular scene object
            if (PrefabUtility.IsPartOfPrefabAsset(obj))
            {
                return false; // It's part of a prefab asset, not an instantiated prefab or regular scene object
            }

            // If the object is part of a prefab instance in the scene, it is considered a scene object
            if (PrefabUtility.IsPartOfPrefabInstance(obj))
            {
                return true; // It is a prefab instance in the scene
            }

            // If all checks passed, the GameObject is a regular scene object
            return true;
        }


        public static bool AnyParentSelected_Editor(this Component cmp) => cmp != null && AnyParentSelected_Editor(cmp.gameObject);
        public static bool AnyParentSelected_Editor(this GameObject obj)
        {
            if (obj == null || Selection.activeTransform == null)
            {
                return false;
            }

            Transform current = obj.transform.parent;

            while (current != null)
            {
                bool selected = Selection.activeTransform != null && Selection.activeTransform == current;
                if (selected)
                {
                    return true;
                }
                current = current.parent;
            }

            return false;
        }

        public static bool IsSelected_Editor(this GameObject obj)
        {
            if (obj == null || Selection.activeTransform == null)
            {
                return false;
            }

            foreach (var go in Selection.gameObjects)
            {
                if (go == obj)
                    return true;
            }

            return false;
        }


        public static void SaveObject_Editor(this Component component)
        {
            if (component == null)
                return;

            EditorUtility.SetDirty(component);
            EditorUtility.SetDirty(component.gameObject);
        }

        public static void SaveObject_Editor(this GameObject obj)
        {
            if (obj == null)
                return;

            EditorUtility.SetDirty(obj);
        }


    }




}
#endif