using UnityEngine;
using UnityEditor;
using System;

namespace EditorTools
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(AssignIDButtonAttribute))]
    public class AssignIDButtonDrawer : PropertyDrawer
    {
        private const float ButtonHeight = 20f;
        private const float ButtonSpacing = 5f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Disable the property field to make it read-only
            GUI.enabled = false;

            // Draw the read-only property field (ID)
            EditorGUI.PropertyField(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                property,
                label
            );

            // Re-enable the GUI to allow interaction with buttons
            GUI.enabled = true;

            // Calculate positions for buttons
            float buttonWidth = (position.width - 2 * ButtonSpacing) / 3f;
            Rect buttonPosition = new Rect(
                position.x,
                position.y + EditorGUIUtility.singleLineHeight + ButtonSpacing,
                buttonWidth,
                ButtonHeight
            );

            if (string.IsNullOrEmpty(property.stringValue))
            {
                property.stringValue = Guid.NewGuid().ToString();
                property.serializedObject.ApplyModifiedProperties();
            }

            // First button: Assign ID
            if (GUI.Button(buttonPosition, "Assign Random ID"))
            {
                if (EditorUtility.DisplayDialog("Confirm Assign ID", "Are you sure you want to assign a new ID?", "Yes", "No"))
                {
                    if (property.propertyType == SerializedPropertyType.String)
                    {
                        // Generate a unique ID (in this case a GUID)
                        property.stringValue = Guid.NewGuid().ToString();
                        property.serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        Debug.LogError("AssignIDButton can only be used with string fields.");
                    }
                }
            }

            // Move to next button position
            buttonPosition.x += buttonWidth + ButtonSpacing;

            // Second button: Enter ID
            if (GUI.Button(buttonPosition, "Enter ID"))
            {
                // Open a popup window to enter a unique ID
                EnterIDPopup.Show(property);
            }

            // Move to next button position
            buttonPosition.x += buttonWidth + ButtonSpacing;

            // Third button: Name as ID
            if (GUI.Button(buttonPosition, "Name as ID"))
            {
                if (EditorUtility.DisplayDialog("Confirm Assign ID", "Are you sure you want to assign name as ID?", "Yes", "No"))
                {
                    if (property.propertyType == SerializedPropertyType.String)
                    {
                        // Assign the ID from the object's name
                        UnityEngine.Object targetObject = property.serializedObject.targetObject;
                        if (targetObject != null)
                        {
                            property.stringValue = targetObject.name;
                            property.serializedObject.ApplyModifiedProperties();
                            Debug.Log($"Assigned ID from object's name: {property.stringValue}");
                        }
                        else
                        {
                            Debug.LogError("Target object is null.");
                        }
                    }
                    else
                    {
                        Debug.LogError("AssignIDButton can only be used with string fields.");
                    }
                }

            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Increase property height to accommodate buttons
            return EditorGUIUtility.singleLineHeight + ButtonHeight + ButtonSpacing * 2;
        }
    }

    // Popup window for entering ID
    public class EnterIDPopup : EditorWindow
    {
        private string newID = "";
        private SerializedProperty property;

        public static void Show(SerializedProperty prop)
        {
            EnterIDPopup window = ScriptableObject.CreateInstance<EnterIDPopup>();
            window.titleContent = new GUIContent("Enter Unique ID");
            window.property = prop.Copy(); // Copy the property to avoid serialization issues
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 300, 80);
            window.ShowUtility();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Enter Unique ID", EditorStyles.boldLabel);
            newID = EditorGUILayout.TextField("ID:", newID);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("OK"))
            {
                if (string.IsNullOrEmpty(newID))
                {
                    EditorUtility.DisplayDialog("Invalid ID", "ID cannot be empty.", "OK");
                    return;
                }

                if (property.propertyType == SerializedPropertyType.String)
                {
                    property.stringValue = newID;
                    property.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    Debug.LogError("EnterIDPopup can only be used with string fields.");
                }
                this.Close();
            }
            if (GUILayout.Button("Cancel"))
            {
                this.Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
#endif

    public class AssignIDButtonAttribute : PropertyAttribute { }
}
