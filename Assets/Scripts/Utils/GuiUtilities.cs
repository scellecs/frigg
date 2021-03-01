namespace Assets.Scripts.Utils {
    using System.Reflection;
    using Attributes;
    using UnityEditor;
    using UnityEngine;

    public class GuiUtilities {
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
    }
}