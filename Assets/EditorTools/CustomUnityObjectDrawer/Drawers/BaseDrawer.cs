#if UNITY_EDITOR
using System;
using UnityEditor;

namespace EditorTools
{
    public abstract class BaseDrawer
    {
        public abstract void OnInspectorGUI(Editor editor, AttributeGroup attributes);

        public abstract bool CanDraw(Type type);

    }


}

#endif