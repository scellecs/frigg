namespace Assets.Scripts.Editor.PropertyDrawers {
    using Attributes;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ReadonlyAttribute))]
    public class ReadonlyPropertyDrawer : BaseDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            using (new EditorGUI.DisabledScope(true)) {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}