#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace EditorTools
{
    public static class RenameChildrenTool
    {
        private const string MenuName = "Tools/Rename Children";

        // Adds a menu item named "Rename Children" to the Tools menu in the Unity Editor
        [MenuItem(MenuName)]
        public static void RenameChildren()
        {
            // Check if a GameObject is selected in the editor
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject != null)
            {
                // Check if the selected GameObject has children
                if (selectedObject.transform.childCount == 0)
                {
                    EditorUtility.DisplayDialog("Rename Children",
                        "The selected GameObject has no children to rename.", "OK");
                    return;
                }

                // Prompt the user to enter the BaseName
                string baseName = EditorUtility.DisplayDialogComplex("Rename Children",
                    "Do you want to enter a custom Base Name?",
                    "Yes", "No", "Cancel") switch
                {
                    0 => PromptForBaseName(),
                    1 => "Child", // Default BaseName
                    _ => null
                };

                if (baseName == null)
                {
                    // User cancelled the operation
                    return;
                }

                // Register the undo operation
                Undo.RegisterCompleteObjectUndo(selectedObject, "Rename Children");

                // Loop through each child and rename them
                for (int i = 0; i < selectedObject.transform.childCount; i++)
                {
                    Transform child = selectedObject.transform.GetChild(i);
                    string newName = $"{baseName}_{i}";
                    child.name = newName;
                }

                // Mark the scene as dirty to ensure changes are saved
                EditorUtility.SetDirty(selectedObject);

                // Optionally, save the scene if desired
                // EditorSceneManager.MarkSceneDirty(selectedObject.scene);

                Debug.Log($"Children of '{selectedObject.name}' renamed successfully with base name '{baseName}'.");
            }
            else
            {
                // Display a warning if no GameObject is selected
                EditorUtility.DisplayDialog("Rename Children",
                    "No GameObject selected. Please select a GameObject with children.", "OK");
            }
        }

        // This function checks if the "Rename Children" menu item is enabled or disabled.
        // It will only be enabled when a GameObject with children is selected.
        [MenuItem(MenuName, true)]
        public static bool RenameChildrenValidation()
        {
            // Enable the menu item only if a GameObject is selected and it has children
            return Selection.activeGameObject != null &&
                   Selection.activeGameObject.transform.childCount > 0;
        }

        /// <summary>
        /// Prompts the user to enter a Base Name using a dialog.
        /// Returns the entered Base Name, or null if the user cancels.
        /// </summary>
        /// <returns>The Base Name entered by the user, or null.</returns>
        private static string PromptForBaseName()
        {
            // Create a new window to prompt for BaseName
            BaseNameWindow.Init();
            return null; // The actual renaming will be handled in the window
        }

        /// <summary>
        /// Editor Window to prompt the user for a Base Name.
        /// </summary>
        private class BaseNameWindow : EditorWindow
        {
            private string baseName = "Child";
            private System.Action<string> onComplete;

            public static void Init()
            {
                BaseNameWindow window = ScriptableObject.CreateInstance<BaseNameWindow>();
                window.titleContent = new GUIContent("Enter Base Name");
                window.position = new Rect(Screen.width / 2, Screen.height / 2, 300, 100);
                window.ShowUtility();
            }

            private void OnGUI()
            {
                GUILayout.Label("Enter Base Name for Children:", EditorStyles.boldLabel);
                baseName = EditorGUILayout.TextField("Base Name", baseName);

                GUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("OK"))
                {
                    ApplyRename(baseName);
                    Close();
                }
                if (GUILayout.Button("Cancel"))
                {
                    Close();
                }
                EditorGUILayout.EndHorizontal();
            }

            private void ApplyRename(string name)
            {
                GameObject selectedObject = Selection.activeGameObject;
                if (selectedObject == null)
                {
                    EditorUtility.DisplayDialog("Rename Children",
                        "No GameObject selected. Please select a GameObject with children.", "OK");
                    return;
                }

                // Register the undo operation
                Undo.RegisterCompleteObjectUndo(selectedObject, "Rename Children");

                // Loop through each child and rename them
                for (int i = 0; i < selectedObject.transform.childCount; i++)
                {
                    Transform child = selectedObject.transform.GetChild(i);
                    string newName = $"{name}_{i}";
                    child.name = newName;
                }

                // Mark the scene as dirty to ensure changes are saved
                EditorUtility.SetDirty(selectedObject);

                Debug.Log($"Children of '{selectedObject.name}' renamed successfully with base name '{name}'.");
            }
        }
    }
}
#endif
