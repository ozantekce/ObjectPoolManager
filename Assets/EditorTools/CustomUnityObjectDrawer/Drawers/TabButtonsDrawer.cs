#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace EditorTools
{

    public class TabButtonsDrawer : BaseDrawer
    {


        public override void OnInspectorGUI(Editor editor, AttributeGroup attributes)
        {
            var first = attributes.First;
            TabButtonAttribute tabButtonAttribute = first.Item1 as TabButtonAttribute;
            string tabMenuName = tabButtonAttribute.TabMenuName;

            string[] displayMethodNames = attributes.Select(g => ((TabButtonAttribute)g.Item1).TabButtonName ?? g.Item2.Name).ToArray();

            string[] methodNames = attributes.Select(g => g.Item2.Name).ToArray();


            EditorGUILayout.BeginVertical("box");
            GUILayout.Label(tabMenuName, EditorStyles.boldLabel);

            int selectedMethodIndex = GUILayout.Toolbar(-1, displayMethodNames, GUILayout.ExpandWidth(true));
            string selectedMethodName = null;
            if (selectedMethodIndex >= 0)
            {
                selectedMethodName = methodNames[selectedMethodIndex];
                foreach (var item in attributes)
                {
                    MethodInfo methodInfo = item.Item2 as MethodInfo;
                    if (methodInfo.Name == selectedMethodName)
                    {
                        methodInfo.Invoke(editor.target, null);
                        break;
                    }
                }
            }

            EditorGUILayout.EndVertical();


        }

        public override bool CanDraw(Type type)
        {
            return type == typeof(TabButtonAttribute);
        }
    }

}


#endif