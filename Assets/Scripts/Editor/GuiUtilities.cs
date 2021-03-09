namespace Assets.Scripts.Utils {
    using System;
    using System.Linq;
    using System.Reflection;
    using Attributes;
    using Attributes.Custom;
    using Editor.CustomPropertyDrawers;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

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

        public static void Field(object value, string label, bool canWrite = true) {
            using (new EditorGUI.DisabledScope(!canWrite)) {
                var objType = value.GetType();
                
                if (objType == typeof(bool))
                {
                    EditorGUILayout.Toggle(label, (bool)value);
                }
                else if (objType == typeof(int))
                {
                    EditorGUILayout.IntField(label, (int)value);
                }
                else if (objType == typeof(long))
                {
                    EditorGUILayout.LongField(label, (long)value);
                }
                else if (objType == typeof(float))
                {
                    EditorGUILayout.FloatField(label, (float)value);
                }
                else if (objType == typeof(double))
                {
                    EditorGUILayout.DoubleField(label, (double)value);
                }
                else if (objType == typeof(string))
                {
                    EditorGUILayout.TextField(label, (string)value);
                }
                else if (objType == typeof(Vector2))
                {
                    EditorGUILayout.Vector2Field(label, (Vector2)value);
                }
                else if (objType == typeof(Vector3))
                {
                    EditorGUILayout.Vector3Field(label, (Vector3)value);
                }
                else if (objType == typeof(Vector4))
                {
                    EditorGUILayout.Vector4Field(label, (Vector4)value);
                }
                else if (objType == typeof(Color))
                {
                    EditorGUILayout.ColorField(label, (Color)value);
                }
                else if (objType == typeof(Bounds))
                {
                    EditorGUILayout.BoundsField(label, (Bounds)value);
                }
                else if (objType == typeof(Rect))
                {
                    EditorGUILayout.RectField(label, (Rect)value);
                }
                else if (typeof(Object).IsAssignableFrom(objType))
                {
                    EditorGUILayout.ObjectField(label, (Object)value, objType, true);
                }
                else if (objType.BaseType == typeof(Enum))
                {
                    EditorGUILayout.EnumPopup(label, (Enum)value);
                }
                else if (objType.BaseType == typeof(TypeInfo))
                {
                    EditorGUILayout.TextField(label, value.ToString());
                }
            }
        }
        #endregion
    }
}