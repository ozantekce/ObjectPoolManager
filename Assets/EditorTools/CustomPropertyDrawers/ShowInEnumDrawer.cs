using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace EditorTools
{
#if UNITY_EDITOR
    /// <summary>
    /// Base class for conditional property drawers.
    /// Handles common logic for showing/hiding properties based on conditions.
    /// </summary>
    public abstract class ConditionalPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Renders the property in the Inspector.
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldShow(property))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        /// <summary>
        /// Determines the height of the property based on whether it should be shown.
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (ShouldShow(property))
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
            else
            {
                // Return zero height to hide the property
                return 0f;
            }
        }

        /// <summary>
        /// Determines whether the property should be shown.
        /// Must be implemented by derived classes.
        /// </summary>
        protected abstract bool ShouldShow(SerializedProperty property);

        /// <summary>
        /// Finds a serialized property or its backing field based on the provided property name.
        /// </summary>
        /// <param name="property">The current serialized property.</param>
        /// <param name="propertyName">The name of the property to find.</param>
        /// <returns>The found SerializedProperty, or null if not found.</returns>
        protected SerializedProperty FindSerializedPropertyOrBackingField(SerializedProperty property, string propertyName)
        {
            // Attempt to find the property directly
            SerializedProperty foundProperty = property.serializedObject.FindProperty(propertyName);

            if (foundProperty == null)
            {
                // Attempt to find it as a backing field (e.g., <PropertyName>k__BackingField)
                string backingFieldName = GetBackingFieldName(propertyName);
                foundProperty = property.serializedObject.FindProperty(backingFieldName);
            }

            // If still not found, attempt to find it relative to the current property path
            if (foundProperty == null)
            {
                string propertyPath = property.propertyPath;
                int lastDotIndex = propertyPath.LastIndexOf('.');

                if (lastDotIndex >= 0)
                {
                    string parentPath = propertyPath.Substring(0, lastDotIndex);
                    string relativePath = $"{parentPath}.{propertyName}";
                    foundProperty = property.serializedObject.FindProperty(relativePath);

                    if (foundProperty == null)
                    {
                        // Attempt to find the backing field in the relative path
                        string relativeBackingFieldName = GetBackingFieldName(propertyName);
                        relativePath = $"{parentPath}.{relativeBackingFieldName}";
                        foundProperty = property.serializedObject.FindProperty(relativePath);
                    }
                }
            }

            return foundProperty;
        }

        /// <summary>
        /// Converts a property name to its backing field name.
        /// </summary>
        /// <param name="propertyName">The original property name.</param>
        /// <returns>The backing field name.</returns>
        private string GetBackingFieldName(string propertyName)
        {
            return $"<{propertyName}>k__BackingField";
        }
    }

    /// <summary>
    /// Drawer that shows a property only if a specified enum field has certain enum names.
    /// </summary>
    [CustomPropertyDrawer(typeof(ShowInEnumAttribute))]
    public class ShowInEnumDrawer : ConditionalPropertyDrawer
    {
        protected override bool ShouldShow(SerializedProperty property)
        {
            ShowInEnumAttribute showInAttribute = attribute as ShowInEnumAttribute;

            if (showInAttribute == null)
                return true; // Show by default if attribute is not correctly applied

            SerializedProperty enumProperty = FindSerializedPropertyOrBackingField(property, showInAttribute.EnumFieldName);

            if (enumProperty != null && enumProperty.propertyType == SerializedPropertyType.Enum)
            {
                string selectedEnumName = enumProperty.enumNames[enumProperty.enumValueIndex];
                if (showInAttribute.EnumNames.Contains(selectedEnumName))
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Drawer that shows a property only if a specified enum field has certain enum values.
    /// </summary>
    [CustomPropertyDrawer(typeof(ShowInEnumValueAttribute))]
    public class ShowInEnumValueDrawer : ConditionalPropertyDrawer
    {
        protected override bool ShouldShow(SerializedProperty property)
        {
            ShowInEnumValueAttribute showInValueAttribute = attribute as ShowInEnumValueAttribute;

            if (showInValueAttribute == null)
                return true; // Show by default if attribute is not correctly applied

            SerializedProperty enumProperty = FindSerializedPropertyOrBackingField(property, showInValueAttribute.EnumFieldName);

            if (enumProperty != null && enumProperty.propertyType == SerializedPropertyType.Enum)
            {
                int selectedEnumValue = enumProperty.enumValueIndex;
                if (showInValueAttribute.EnumValues.Contains(selectedEnumValue))
                {
                    return true;
                }
            }

            return false;
        }
    }
#endif

    /// <summary>
    /// Attribute to conditionally show a field based on specified enum names.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ShowInEnumAttribute : PropertyAttribute
    {
        public string[] EnumNames { get; }
        public string EnumFieldName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowInEnumAttribute"/> class.
        /// </summary>
        /// <param name="enumFieldName">The name of the enum field to evaluate.</param>
        /// <param name="enumNames">The enum names that trigger the property to be shown.</param>
        public ShowInEnumAttribute(string enumFieldName, params string[] enumNames)
        {
            EnumFieldName = enumFieldName;
            EnumNames = enumNames;
        }
    }

    /// <summary>
    /// Attribute to conditionally show a field based on specified enum values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ShowInEnumValueAttribute : PropertyAttribute
    {
        public int[] EnumValues { get; }
        public string EnumFieldName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowInEnumValueAttribute"/> class.
        /// </summary>
        /// <param name="enumFieldName">The name of the enum field to evaluate.</param>
        /// <param name="enumValues">The enum values that trigger the property to be shown.</param>
        public ShowInEnumValueAttribute(string enumFieldName, params int[] enumValues)
        {
            EnumFieldName = enumFieldName;
            EnumValues = enumValues;
        }
    }
}
