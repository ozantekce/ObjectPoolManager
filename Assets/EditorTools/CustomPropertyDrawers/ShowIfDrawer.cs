using UnityEngine;
using UnityEditor;
using System;

namespace EditorTools
{
#if UNITY_EDITOR

    /// <summary>
    /// Custom Property Drawer to show/hide a field based on a boolean condition.
    /// </summary>
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    [CanEditMultipleObjects]
    public class ShowIfDrawer : PropertyDrawer
    {
        /// <summary>
        /// Renders the property in the Inspector.
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute showIfAttribute = (ShowIfAttribute)attribute;
            SerializedProperty boolProperty = GetBooleanProperty(property, showIfAttribute.BooleanFieldName);

            if (boolProperty != null && boolProperty.propertyType == SerializedPropertyType.Boolean)
            {
                if (boolProperty.boolValue)
                {
                    EditorGUI.PropertyField(position, property, label, true);
                }
            }
            else
            {
                // If the boolean property is not found or not a boolean, display the property normally
                EditorGUI.PropertyField(position, property, label, true);

                if (!string.IsNullOrEmpty(showIfAttribute.BooleanFieldName))
                {
                    Debug.LogWarning($"ShowIf: Could not find boolean property '{showIfAttribute.BooleanFieldName}' for field '{property.displayName}'. Ensure it is serialized and correctly named.");
                }
            }
        }

        /// <summary>
        /// Determines the height of the property based on whether it should be shown.
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute showIfAttribute = (ShowIfAttribute)attribute;
            SerializedProperty boolProperty = GetBooleanProperty(property, showIfAttribute.BooleanFieldName);

            if (boolProperty != null && boolProperty.propertyType == SerializedPropertyType.Boolean)
            {
                return boolProperty.boolValue ? EditorGUI.GetPropertyHeight(property, label, true) : 0f;
            }

            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        /// <summary>
        /// Attempts to find the boolean property based on the provided field name.
        /// Handles nested properties by traversing the property path.
        /// </summary>
        /// <param name="property">The current serialized property.</param>
        /// <param name="booleanFieldName">The name of the boolean field to reference.</param>
        /// <returns>The found SerializedProperty or null if not found.</returns>
        private SerializedProperty GetBooleanProperty(SerializedProperty property, string booleanFieldName)
        {
            // Split the current property's path to handle nested properties
            string[] pathParts = property.propertyPath.Split('.');
            if (pathParts.Length == 0)
            {
                Debug.LogWarning("ShowIf: Property path is empty.");
                return null;
            }

            // Remove the last part to get the parent path
            string parentPath = "";
            if (pathParts.Length > 1)
            {
                parentPath = string.Join(".", pathParts, 0, pathParts.Length - 1);
            }

            // Try to find the boolean property relative to the parent
            SerializedProperty parentProperty = string.IsNullOrEmpty(parentPath)
                ? property.serializedObject.FindProperty(booleanFieldName)
                : property.serializedObject.FindProperty($"{parentPath}.{booleanFieldName}");

            if (parentProperty != null && parentProperty.propertyType == SerializedPropertyType.Boolean)
            {
                return parentProperty;
            }

            // If not found, attempt to find it globally
            SerializedProperty globalBoolProperty = property.serializedObject.FindProperty(booleanFieldName);
            if (globalBoolProperty != null && globalBoolProperty.propertyType == SerializedPropertyType.Boolean)
            {
                return globalBoolProperty;
            }

            // Attempt to find it as a backing field (for auto-properties)
            string backingFieldName = $"<{booleanFieldName}>k__BackingField";
            SerializedProperty backingBoolProperty = property.serializedObject.FindProperty(backingFieldName);
            if (backingBoolProperty != null && backingBoolProperty.propertyType == SerializedPropertyType.Boolean)
            {
                return backingBoolProperty;
            }

            // Property not found
            Debug.LogWarning($"ShowIf: Could not find boolean property '{booleanFieldName}' for field '{property.displayName}'.");
            return null;
        }
    }
#endif

    /// <summary>
    /// Attribute to conditionally show a field in the Inspector based on a boolean field's value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
        AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = true)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string BooleanFieldName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowIfAttribute"/> class.
        /// </summary>
        /// <param name="booleanFieldName">The name of the boolean field to reference.</param>
        public ShowIfAttribute(string booleanFieldName)
        {
            BooleanFieldName = booleanFieldName;
        }
    }
}
