namespace Frigg.Editor {
    using UnityEngine;
    using UnityEditor;
    using Utils;

    public abstract class BaseDrawer : PropertyDrawer {
        public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginChangeCheck();

            var isVisible = CoreUtilities.IsPropertyVisible(property);
            if (!isVisible)
                return;

            var isEnabled = CoreUtilities.IsPropertyEnabled(property);

            using (new EditorGUI.DisabledScope(!isEnabled)) {
                label = CoreUtilities.GetGUIContent(property);
                var hideAttr = CoreUtilities.TryGetAttribute<HideLabelAttribute>(property);
                if (hideAttr != null)
                    label.text = string.Empty;

                this.OnDrawerGUI(position, property, label);

                if (EditorGUI.EndChangeCheck()) {
                    CoreUtilities.OnDataChanged(property);
                }
            }
        }

        protected abstract void OnDrawerGUI(Rect rect, SerializedProperty prop, GUIContent label);
    }
}