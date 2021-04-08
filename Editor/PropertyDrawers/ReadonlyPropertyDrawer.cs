namespace Frigg.Editor {
    using UnityEditor;
    using UnityEngine;

    [UnityEditor.CustomPropertyDrawer(typeof(ReadonlyAttribute))]
    public class ReadonlyPropertyDrawer : BaseDrawer {
        protected override void OnDrawerGUI(Rect rect, SerializedProperty prop, GUIContent label) {
            using (new EditorGUI.DisabledScope(true)) {
                
                EditorGUI.PropertyField(rect, prop, label);
            }
        }
    }
}