namespace Frigg.Editor {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using Utils;
    using Object = UnityEngine.Object;

    public static class GuiUtilities {
        public const   float SPACE              = 5.0f;
        private const  int   PROPERTY_MIN_WIDTH = 212;

        #region property implementations
        
        public static void DrawTree(PropertyTree tree) {
            tree.Draw();
        }
        
        public static void PropertyField(SerializedProperty property, bool includeChildren) {
            /*var content = CoreUtilities.GetGUIContent(property);
            
            DrawPropertyField(new Rect(), property, content, includeChildren);*/
        }
        
        //Draw single property field
        private static void DrawPropertyField(Rect rect, SerializedProperty property, 
            GUIContent label, bool includeChildren) {

            //Check if there are any custom attributes on this property. If true - handle it using custom drawer and then return.
            /*if (HandleCustomDrawer(rect, property))
                return;*/
            //If there weren't any custom attributes - we need to draw default property field
            
            //Check if we need to hide label
            /*if(CoreUtilities.TryGetAttribute<HideLabelAttribute>(property) != null)
                label.text = string.Empty;*/

            //Check if Editor is in wide mode so we won't wrap any properties to the next line
            if (!EditorGUIUtility.wideMode)
            {
                EditorGUIUtility.wideMode   = true;
                EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - PROPERTY_MIN_WIDTH ;
            }
            
            EditorGUILayout.PropertyField(property, label, includeChildren);

            /*var isVisible = CoreUtilities.IsPropertyVisible(property);
            if (!isVisible) {
                return;
            }#1#
            
            //var isEnabled = CoreUtilities.IsPropertyEnabled(property);

            //finally - draw PropertyField
            EditorGUI.BeginChangeCheck();
            
            /*using (new EditorGUI.DisabledScope(!isEnabled))
            {
                EditorGUILayout.PropertyField(property, label, includeChildren);
            }#1#
            
            if(EditorGUI.EndChangeCheck())
                CoreUtilities.OnDataChanged(property);
        }*/
        #endregion

        /*public static void HandleDecorators(Type targetType, Rect rect = default) {
            if (!targetType.IsDefined(typeof(BaseDecoratorAttribute)))
                return;
            
            var attr       = (BaseDecoratorAttribute[]) Attribute.GetCustomAttributes(targetType, typeof(BaseDecoratorAttribute));

            foreach (var obj in attr) {
                DecoratorDrawerUtils.GetDecorator(obj.GetType()).OnGUI(rect == default ?
                    EditorGUILayout.GetControlRect(true, 0) : rect, targetType, obj);
            }
        }

        public static void HandleDecorators(SerializedProperty element, Rect rect = default, bool isArray = false) {
            var attr = CoreUtilities.TryGetAttributes<BaseDecoratorAttribute>(element);
            
            foreach (var obj in attr) {
                if (rect == default) {
                    DecoratorDrawerUtils.GetDecorator(obj.GetType()).OnGUI(EditorGUILayout.GetControlRect(true, 0), element, obj, isArray);
                    continue;
                }
                
                DecoratorDrawerUtils.GetDecorator(obj.GetType()).OnGUI(rect, element, obj, isArray);
                rect.y += obj.Height + SPACE;
            }
        }
        
        private static bool HandleCustomDrawer(Rect rect, SerializedProperty property) {
            
            /Check for custom attributes
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
            */
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
        
        #endregion
    }
}