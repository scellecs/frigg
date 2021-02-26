namespace Assets.Scripts.Editor.PropertyDrawers {
    using Attributes;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public class ButtonDrawer : BaseDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndProperty();
        }
    }
}