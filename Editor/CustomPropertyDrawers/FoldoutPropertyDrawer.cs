namespace Frigg.Editor {
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    public class FoldoutPropertyDrawer : CustomPropertyDrawer {
        public static readonly FoldoutPropertyDrawer     instance      = new FoldoutPropertyDrawer();
        public                 Dictionary<string, bool> foldoutValues = new Dictionary<string, bool>();

        protected override void CreateAndDrawLayout(SerializedProperty property, GUIContent label) {
            throw new System.NotImplementedException();
        }

        protected override object CreateAndDrawLayout(MemberInfo member, object target, GUIContent label) => throw new System.NotImplementedException();

        protected override void CreateAndDraw(SerializedProperty property, GUIContent content) {
            //throw new System.NotImplementedException();
        }

        protected override object CreateAndDraw(Rect rect, MemberInfo member, object target, GUIContent content) {
            if (!instance.foldoutValues.ContainsKey(member.Name)) {
                instance.foldoutValues.Add(member.Name, false);
            }
            
            instance.foldoutValues[member.Name] = EditorGUILayout.BeginFoldoutHeaderGroup
                (instance.foldoutValues[member.Name], content);
            
            EditorGUILayout.EndFoldoutHeaderGroup();

            return target;
        }
    }
}