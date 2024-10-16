#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EditorTools
{

    public class ButtonDrawer : BaseDrawer
    {
        public override void OnInspectorGUI(Editor editor, AttributeGroup attributes)
        {

            foreach ((BaseAttribute, MemberInfo) attribute in attributes)
            {
                ButtonAttribute buttonAttribute = attribute.Item1 as ButtonAttribute;
                MethodInfo methodInfo = attribute.Item2 as MethodInfo;
                string buttonName = buttonAttribute.ButtonName ?? methodInfo.Name;
                if (GUILayout.Button(buttonName))
                {
                    methodInfo.Invoke(editor.target, null);
                }
            }
        }

        public override bool CanDraw(Type type)
        {
            return type == typeof(ButtonAttribute);
        }


    }


}



#endif