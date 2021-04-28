namespace Frigg.Editor {
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    public class SerializablePropertyDrawer : CustomPropertyDrawer {
        protected override void CreateAndDraw(Rect rect, SerializedProperty property, GUIContent label) {
            
        }

        //TODO: Handle foldouts for properties
        protected override object CreateAndDraw(Rect rect, MemberInfo member, object target, GUIContent label) {
            return EditorGUILayout.Foldout(true, label);
        }
    }
}