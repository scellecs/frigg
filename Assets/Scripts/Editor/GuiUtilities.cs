namespace Assets.Scripts.Editor {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Attributes;
    using Attributes.Custom;
    using CustomPropertyDrawers;
    using UnityEditor;
    using UnityEngine;
    using Utils;
    using Object = UnityEngine.Object;

    public static class GuiUtilities {

        public static List<Type> reorderableAllowed = new List<Type> {
            typeof(int),
            typeof(string),
            typeof(float),
            typeof(long),
            typeof(double),
            typeof(bool)
        };

        #region property implementations
        public static void PropertyField(SerializedProperty property, bool includeChildren) {
            DrawPropertyField(new Rect(), property, new GUIContent(property.name), includeChildren);
        }
        
        private static void DrawPropertyField(Rect rect, SerializedProperty property, 
            GUIContent label, bool includeChildren) {

            if (HandleCustomProperty(rect, property))
                return;
            
            //If there weren't any custom attributes - we need to draw default fields
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(property, label, includeChildren);
            EditorGUI.EndChangeCheck();
            
            property.serializedObject.ApplyModifiedProperties();
        }
        #endregion

        private static bool HandleCustomProperty(Rect rect, SerializedProperty property) {
            if (!CoreUtilities.TryGetAttributes<CustomAttribute>(property).Any()) {

                if (property.isArray) {
                    Debug.Log(property.type);
                    new ReorderableListAttribute().GetCustomDrawer().OnGUI(rect, property);
                    return true;
                }

                return false;
            }

            var customAttr = CoreUtilities.TryGetAttribute<CustomAttribute>(property); //reorderable list if there is an attribute
            customAttr?.GetCustomDrawer()?.OnGUI(rect, property);
            return true;
        }
        
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

        public static object Field(object value, string label, bool canWrite = true) {
            using (new EditorGUI.DisabledScope(!canWrite)) {
                
                var objType = value.GetType();

                if (objType == typeof(bool))
                {
                    return EditorGUILayout.Toggle(label, (bool)value);
                }
                if (objType == typeof(int))
                {
                    return EditorGUILayout.IntField(label, (int)value);
                }
                if (objType == typeof(long))
                {
                    return EditorGUILayout.LongField(label, (long)value);
                }
                if (objType == typeof(float))
                {
                    return EditorGUILayout.FloatField(label, (float)value);
                }
                if (objType == typeof(double))
                {
                    return EditorGUILayout.DoubleField(label, (double)value);
                }
                if (objType == typeof(string))
                {
                    return EditorGUILayout.TextField(label, (string)value);
                }
                if (objType == typeof(Vector2))
                {
                    return EditorGUILayout.Vector2Field(label, (Vector2)value);
                }
                if (objType == typeof(Vector3))
                {
                    return EditorGUILayout.Vector3Field(label, (Vector3)value);
                }
                if (objType == typeof(Vector4))
                {
                    return EditorGUILayout.Vector4Field(label, (Vector4)value);
                }
                if (objType == typeof(Color))
                {
                    return EditorGUILayout.ColorField(label, (Color)value);
                }
                if (objType == typeof(Bounds))
                {
                    return EditorGUILayout.BoundsField(label, (Bounds)value);
                }
                if (objType == typeof(Rect))
                {
                    return EditorGUILayout.RectField(label, (Rect)value);
                }
                if (typeof(Object).IsAssignableFrom(objType))
                {
                    return EditorGUILayout.ObjectField(label, (Object)value, objType, true);
                }
                if (objType.BaseType == typeof(Enum))
                {
                    return EditorGUILayout.EnumPopup(label, (Enum)value);
                }
                if (objType.BaseType == typeof(TypeInfo))
                {
                    return EditorGUILayout.TextField(label, value.ToString());
                }
            }

            return null;
        }
        #endregion
    }
}