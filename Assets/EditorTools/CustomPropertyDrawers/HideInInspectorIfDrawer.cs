using UnityEngine;
using UnityEditor;
using System;

namespace EditorTools
{
#if UNITY_EDITOR

    /// <summary>
    /// Custom Property Drawer to hide a field based on a boolean condition.
    /// </summary>
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    [CanEditMultipleObjects]
    public class HideIfDrawer : PropertyDrawer
    {
        /// <summary>
        /// Renders the property in the Inspector.
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            HideIfAttribute hideIfAttribute = (HideIfAttribute)attribute;
            SerializedProperty boolProperty = GetBooleanProperty(property, hideIfAttribute.BooleanFieldName);

            if (boolProperty != null && boolProperty.propertyType == SerializedPropertyType.Boolean)
            {
                if (!boolProperty.boolValue) // Only draw if the condition is false
                {
                    EditorGUI.PropertyField(position, property, label, true);
                }
            }
            else
            {
                // If the boolean property is not found or not a boolean, display the property normally
                EditorGUI.PropertyField(position, property, label, true);

                if (!string.IsNullOrEmpty(hideIfAttribute.BooleanFieldName))
                {
                    Debug.LogWarning($"HideIf: Could not find boolean property '{hideIfAttribute.BooleanFieldName}' for field '{property.displayName}'. Ensure it is serialized and correctly named.");
                }
            }
        }

        /// <summary>
        /// Determines the height of the property based on whether it should be shown.
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            HideIfAttribute hideIfAttribute = (HideIfAttribute)attribute;
            SerializedProperty boolProperty = GetBooleanProperty(property, hideIfAttribute.BooleanFieldName);

            if (boolProperty != null && boolProperty.propertyType == SerializedPropertyType.Boolean)
            {
                return boolProperty.boolValue ? 0f : EditorGUI.GetPropertyHeight(property, label, true); // Return 0 if the condition is true
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
                Debug.LogWarning("HideIf: Property path is empty.");
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
            Debug.LogWarning($"HideIf: Could not find boolean property '{booleanFieldName}' for field '{property.displayName}'.");
            return null;
        }
    }
#endif

    /// <summary>
    /// Attribute to conditionally hide a field in the Inspector based on a boolean field's value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
        AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = true)]
    public class HideIfAttribute : PropertyAttribute
    {
        public string BooleanFieldName;

        /// <summary>
        /// Initializes a new instance of the <see cref="HideIfAttribute"/> class.
        /// </summary>
        /// <param name="booleanFieldName">The name of the boolean field to reference.</param>
        public HideIfAttribute(string booleanFieldName)
        {
            BooleanFieldName = booleanFieldName;
        }
    }
}
