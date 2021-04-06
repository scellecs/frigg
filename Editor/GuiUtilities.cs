using Assets.Scripts.Attributes.Meta;

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
    using CustomAttributeExtensions = CustomPropertyDrawers.CustomAttributeExtensions;
    using CustomPropertyDrawer = UnityEditor.CustomPropertyDrawer;
    using Object = UnityEngine.Object;

    public static class GuiUtilities {
        private const int PROPERTY_MIN_WIDTH = 212;
        
        #region property implementations
        public static void PropertyField(SerializedProperty property, bool includeChildren) {
            var content = CoreUtilities.GetGUIContent(property);
            
            DrawPropertyField(new Rect(), property, content, includeChildren);
        }
        
        //Draw single property field
        private static void DrawPropertyField(Rect rect, SerializedProperty property, 
            GUIContent label, bool includeChildren) {

            //Check if there are any custom attributes on this property. If true - handle it using custom drawer and then return.
            if (HandleCustomDrawer(rect, property))
                return;
            //If there weren't any custom attributes - we need to draw default property field
            
            //Check if we need to hide label
            if(CoreUtilities.TryGetAttribute<HideLabelAttribute>(property) != null)
                label.text = string.Empty;

            //finally - draw PropertyField
            EditorGUI.BeginChangeCheck();
            
            //Check if Editor is in wide mode so we won't wrap any properties to the next line
            if (!EditorGUIUtility.wideMode)
            {
                EditorGUIUtility.wideMode   = true;
                EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - PROPERTY_MIN_WIDTH ;
            }

            EditorGUILayout.PropertyField(property, label, includeChildren);
            
            if(EditorGUI.EndChangeCheck())
                CoreUtilities.OnDataChanged(property);
        }
        #endregion
        
        private static bool HandleCustomDrawer(Rect rect, SerializedProperty property) {
            //Check for custom attributes
            var attributes = CoreUtilities.TryGetAttributes<CustomAttribute>(property);
            
            //If there is at least one property with applied attribute - Get it's drawer and draw elements
            if (attributes.Any()) {
                CustomAttributeExtensions.GetCustomDrawer(attributes[0]).OnGUI(rect, property);
                return true;
            }
            
            var type = CoreUtilities.GetPropertyType(property);
            if (type == null)
                return false;

            //Not used directly on the property
            if (type.IsSerializable) {
                var attr = (CustomAttribute) Attribute.GetCustomAttribute(type,
                    typeof(InlinePropertyAttribute));

                if (attr != null) {
                    CustomAttributeExtensions.GetCustomDrawer(attr).OnGUI(rect, property);
                    return true;
                }
            }

            if (!property.isArray) {
                return false;
            }

            if (type == typeof(string)) {
                return false;
            }

            new ReorderableListDrawer().OnGUI(rect, property);
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
            GUIContent label, int selectedIndex, GUIContent[] options, object[] values)
        {
            EditorGUI.BeginChangeCheck();

            var newIndex = EditorGUI.Popup(rect, label, selectedIndex, options);

            if (!EditorGUI.EndChangeCheck())
                return;

            Undo.RecordObject(serializedObject.targetObject, "Dropdown");
            field.SetValue(target, values[newIndex]);
        }

        public static object Field(object value, GUIContent content, bool canWrite = true) {
            using (new EditorGUI.DisabledScope(!canWrite)) {
                var objType = value.GetType();

                if (objType == typeof(bool))
                {
                    return EditorGUILayout.Toggle(content, (bool)value);
                }
                if (objType == typeof(int))
                {
                    return EditorGUILayout.IntField(content, (int)value);
                }
                if (objType == typeof(long))
                {
                    return EditorGUILayout.LongField(content, (long)value);
                }
                if (objType == typeof(float))
                {
                    return EditorGUILayout.FloatField(content, (float)value);
                }
                if (objType == typeof(double))
                {
                    return EditorGUILayout.DoubleField(content, (double)value);
                }
                if (objType == typeof(string))
                {
                    return EditorGUILayout.TextField(content, (string)value);
                }
                if (objType == typeof(Vector2))
                {
                    return EditorGUILayout.Vector2Field(content, (Vector2)value);
                }
                if (objType == typeof(Vector3))
                {
                    return EditorGUILayout.Vector3Field(content, (Vector3)value);
                }
                if (objType == typeof(Vector4))
                {
                    return EditorGUILayout.Vector4Field(content, (Vector4)value);
                }
                if (objType == typeof(Color))
                {
                    return EditorGUILayout.ColorField(content, (Color)value);
                }
                if (objType == typeof(Bounds))
                {
                    return EditorGUILayout.BoundsField(content, (Bounds)value);
                }
                if (objType == typeof(Rect))
                {
                    return EditorGUILayout.RectField(content, (Rect)value);
                }
                if (typeof(Object).IsAssignableFrom(objType))
                {
                    return EditorGUILayout.ObjectField(content, (Object)value, objType, true);
                }
                if (objType.BaseType == typeof(Enum))
                {
                    return EditorGUILayout.EnumPopup(content, (Enum)value);
                }
                if (objType.BaseType == typeof(TypeInfo))
                {
                    return EditorGUILayout.TextField(content, value.ToString());
                }
            }

            return null;
        }
        #endregion
    }
}