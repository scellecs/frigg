namespace Assets.Scripts.Editor.PropertyDrawers {
    using Attributes;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ReadonlyAttribute))]
    public class ReadonlyPropertyDrawer : BaseDrawer {
        protected override void OnDrawerGUI(Rect rect, SerializedProperty prop, GUIContent label) {
            using (new EditorGUI.DisabledScope(true)) {
                
                EditorGUI.PropertyField(rect, prop, label);
            }
        }
    }
}