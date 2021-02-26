namespace Assets.Scripts.Editor.PropertyDrawers {
    using UnityEngine;
    using UnityEditor;

    public class BaseDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            //TODO: validations
            base.OnGUI(position, property, label);
        }
    }
}