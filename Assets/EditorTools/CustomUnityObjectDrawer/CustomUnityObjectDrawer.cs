#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;


namespace EditorTools
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(UnityEngine.Object), true, isFallback = true)]
    public class CustomUnityObjectDrawer : Editor
    {

        private static readonly List<BaseDrawer> Drawers = new List<BaseDrawer>
        {
            new ButtonDrawer(),
            new TabButtonsDrawer(),
        };

        public override void OnInspectorGUI()
        {

            base.OnInspectorGUI();

            MemberInfo[] members = target.GetType().GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            /*
            using (new LocalizationGroup(target))
            {
                EditorGUI.BeginChangeCheck();
                serializedObject.UpdateIfRequiredOrScript();
                SerializedProperty ýterator = serializedObject.GetIterator();
                bool enterChildren = true;
                while (ýterator.NextVisible(enterChildren))
                {
                    using (new EditorGUI.DisabledScope("m_Script" == ýterator.propertyPath))
                    {
                        Debug.Log(ýterator.propertyPath);
                        EditorGUILayout.PropertyField(ýterator, true);
                    }

                    enterChildren = false;
                }

                serializedObject.ApplyModifiedProperties();
                EditorGUI.EndChangeCheck();
            }
            */

            List<(BaseAttribute, MemberInfo)> attributes = new List<(BaseAttribute, MemberInfo)>();

            foreach (MemberInfo member in members)
            {
                Attribute attributeOfMember = member.GetCustomAttribute(typeof(BaseAttribute), true);
                if (attributeOfMember != null)
                {
                    attributes.Add(((BaseAttribute)attributeOfMember, member));
                }
            }

            List<AttributeGroup> groups = CreateAttributeGroups(attributes);
            groups.Sort();


            foreach (AttributeGroup group in groups)
            {
                Type attributeType = group.AttributeType;

                if (attributeType == null)
                    continue;

                foreach (BaseDrawer drawer in Drawers)
                {
                    if (drawer.CanDraw(attributeType))
                    {
                        drawer.OnInspectorGUI(this, group);
                        break;
                    }
                }
            }

        }


        private List<AttributeGroup> CreateAttributeGroups(List<(BaseAttribute, MemberInfo)> attributes)
        {
            Dictionary<string, AttributeGroup> idToAttributeGroup = new Dictionary<string, AttributeGroup>();
            for (int i = 0; i < attributes.Count; i++)
            {
                string id = attributes[i].Item1.ID;
                AttributeGroup attributeGroup = idToAttributeGroup.GetValueOrDefault(id, new AttributeGroup());
                attributeGroup.Add(attributes[i].Item1, attributes[i].Item2);
                idToAttributeGroup[id] = attributeGroup;
            }

            List<AttributeGroup> attributeGroups = new List<AttributeGroup>(idToAttributeGroup.Values);
            return attributeGroups;
        }

    }




}




#endif



