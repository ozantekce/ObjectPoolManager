using UnityEngine;
using UnityEditor;

namespace EditorTools
{

#if UNITY_EDITOR

    [CanEditMultipleObjects]
    [CustomPropertyDrawer(typeof(ReadOnlyInPlayModeAttribute))]
    public class ReadOnlyInPlayModeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool wasEnabled = GUI.enabled;
            GUI.enabled = !Application.isPlaying; // Disable if in play mode
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = wasEnabled; // Restore GUI.enabled to its original state
        }
    }
#endif


    public class ReadOnlyInPlayModeAttribute : PropertyAttribute
    {
    }

}
