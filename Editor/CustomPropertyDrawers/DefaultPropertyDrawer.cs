namespace Frigg.Editor {
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    public class DefaultPropertyDrawer : CustomPropertyDrawer {
        protected override void CreateAndDrawLayout(SerializedProperty property, GUIContent label) {
            throw new System.NotImplementedException();
        }

        protected override object CreateAndDrawLayout(MemberInfo member, object target, GUIContent label) => throw new System.NotImplementedException();

        protected override void CreateAndDraw(SerializedProperty property, GUIContent label) {
            throw new System.NotImplementedException();
        }

        protected override object CreateAndDraw(Rect rect, MemberInfo member, object target, GUIContent label) {
            return null;
        }
    }
}