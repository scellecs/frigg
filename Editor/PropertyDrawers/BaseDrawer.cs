namespace Assets.Scripts.Editor.PropertyDrawers {
    using UnityEngine;
    using UnityEditor;
    using Utils;

    public abstract class BaseDrawer : PropertyDrawer {
        public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginChangeCheck();

            this.OnDrawerGUI(position, property, label);
            
            if (EditorGUI.EndChangeCheck()) {
                CoreUtilities.OnDataChanged(property);
            }
        }

        protected abstract void OnDrawerGUI(Rect rect, SerializedProperty prop, GUIContent label);
    }
}