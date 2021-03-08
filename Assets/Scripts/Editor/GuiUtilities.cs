namespace Assets.Scripts.Utils {
    using System.Linq;
    using System.Reflection;
    using Attributes;
    using Attributes.Custom;
    using Editor.CustomPropertyDrawers;
    using UnityEditor;
    using UnityEngine;

    public static class GuiUtilities {

        #region property implementations
        public static void PropertyField(SerializedProperty property, bool includeChildren) {
            DrawPropertyField(new Rect(), property, new GUIContent(property.name), includeChildren);
        }
        
        private static void DrawPropertyField(Rect rect, SerializedProperty property, 
            GUIContent label, bool includeChildren) {
            
            var customAttr = CoreUtilities.TryGetAttribute<CustomAttribute>(property);
            if (customAttr != null)
                customAttr.GetCustomDrawer().OnGUI(rect, property);
            
            else {

                var attr = CoreUtilities.TryGetAttributes<BaseAttribute>(property).Any();

                if (!attr) {
                    return;
                }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(property, label, includeChildren);
                EditorGUI.EndChangeCheck();
            }
        }
        #endregion
        
        #region elements
        public static void Button(Object obj, MethodInfo info) {
            var attr = (ButtonAttribute)info.GetCustomAttributes(typeof(ButtonAttribute), true)[0];
            
            if (GUILayout.Button(attr.Text)) {
                info.Invoke(obj, new object[]{ });
            }
        }

        public static void Dropdown(
            Rect rect, SerializedObject serializedObject, object target, FieldInfo field,
            string label, int selectedIndex, string[] options, object[] values)
        {
            EditorGUI.BeginChangeCheck();

            var newIndex = EditorGUI.Popup(rect, label, selectedIndex, options);

            if (!EditorGUI.EndChangeCheck())
                return;

            Undo.RecordObject(serializedObject.targetObject, "Dropdown");
            field.SetValue(target, values[newIndex]);
        }
        #endregion
    }
}