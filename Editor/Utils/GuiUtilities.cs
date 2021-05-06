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
        private static int   calledTimes        = 0;
        
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

            //Check if Editor is in wide mode so we won't wrap any properties to the next line
            if (!EditorGUIUtility.wideMode)
            {
                EditorGUIUtility.wideMode   = true;
                EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - PROPERTY_MIN_WIDTH ;
            }

            var isVisible = CoreUtilities.IsPropertyVisible(property);
            if (!isVisible) {
                return;
            }
            
            var isEnabled = CoreUtilities.IsPropertyEnabled(property);

            //finally - draw PropertyField
            EditorGUI.BeginChangeCheck();
            
            using (new EditorGUI.DisabledScope(!isEnabled))
            {
                EditorGUILayout.PropertyField(property, label, includeChildren);
            }
            
            if(EditorGUI.EndChangeCheck())
                CoreUtilities.OnDataChanged(property);
        }
        #endregion

        public static int AmountOfDecorators(SerializedProperty property) {
            var attr = CoreUtilities.TryGetAttributes<BaseDecoratorAttribute>(property).ToList();
            return !attr.Any() ? 0 : attr.Count;
        }
        
        //Return total allocated height
        public static float GetDecoratorsHeight(MemberInfo element) {
            var attr = element.GetCustomAttributes<BaseDecoratorAttribute>().ToList();
            if (!attr.Any()) {
                return 0;
            }

            var height = 0f;
            foreach (var obj in attr) {
                height += obj.Height;
            }

            return height;
        }
        
        public static float GetDecoratorsHeight(SerializedProperty element) {
            var attr = CoreUtilities.TryGetAttributes<BaseDecoratorAttribute>(element).ToList();
            if (!attr.Any()) {
                return 0;
            }

            var height = 0f;
            foreach (var obj in attr) {
                var drawer = DecoratorDrawerUtils.GetDecorator(obj.GetType());
                if (drawer.IsVisible(element))
                   height += obj.Height;
            }

            return height;
        }
            

        public static void HandleDecorators(Type targetType, Rect rect = default) {
            if (!targetType.IsDefined(typeof(BaseDecoratorAttribute)))
                return;
            
            var attr       = (BaseDecoratorAttribute[]) Attribute.GetCustomAttributes(targetType, typeof(BaseDecoratorAttribute));

            foreach (var obj in attr) {
                DecoratorDrawerUtils.GetDecorator(obj.GetType()).OnGUI(rect == default ?
                    EditorGUILayout.GetControlRect(true, 0) : rect, targetType, obj);
            }
        }
        
        
        public static void HandleDecorators(MemberInfo element, Rect rect = default, bool isArray = false) {
            if (!element.IsDefined(typeof(BaseDecoratorAttribute)))
                return;
            
            var attr = element.GetCustomAttributes<BaseDecoratorAttribute>();

            foreach (var obj in attr) {
                if (rect == default) {
                    DecoratorDrawerUtils.GetDecorator(obj.GetType()).OnGUI(EditorGUILayout.GetControlRect(true, 0), element, obj, isArray);
                    continue;
                }
                    
                DecoratorDrawerUtils.GetDecorator(obj.GetType()).OnGUI(rect, element, obj);
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
        
        public static object MultiField(Rect rect, MemberInfo info, object value, GUIContent content) {
            var drawer = CustomAttributeExtensions.GetCustomDrawer(typeof(InlinePropertyAttribute));
            return drawer.OnGUI(rect, value, info, content);
        }
        
        public static object MultiFieldLayout(MemberInfo info, object value, GUIContent content, bool canWrite = true) {
            var drawer = CustomAttributeExtensions.GetCustomDrawer(typeof(InlinePropertyAttribute));
            return drawer.OnGUI(Rect.zero, value, info, content);
        }

        
        public static object FieldLayout(Type objType, object value, GUIContent content, bool canWrite = true) {
            using (new EditorGUI.DisabledScope(!canWrite)) {
                
                if (!CoreUtilities.IsPrimitiveUnityType(objType)) {
                    return null;
                }
                
                if (objType == typeof(bool)) {
                    return EditorGUILayout.Toggle(content, (bool) value);
                }

                if (objType == typeof(int)) {
                    return EditorGUILayout.IntField(content, (int) value);
                }

                if (objType == typeof(long)) {
                    return EditorGUILayout.LongField(content, (long) value);
                }

                if (objType == typeof(float)) {
                    return EditorGUILayout.FloatField(content, (float) value);
                }

                if (objType == typeof(double)) {
                    return EditorGUILayout.DoubleField(content, (double) value);
                }

                if (objType == typeof(string)) {
                    return EditorGUILayout.TextField(content, (string) value);
                }

                if (objType == typeof(Vector2)) {
                    return EditorGUILayout.Vector2Field(content, (Vector2) value);
                }

                if (objType == typeof(Vector3)) {
                    return EditorGUILayout.Vector3Field(content, (Vector3) value);
                }

                if (objType == typeof(Vector4)) {
                    return EditorGUILayout.Vector4Field(content, (Vector4) value);
                }

                if (objType == typeof(Color)) {
                    return EditorGUILayout.ColorField(content, (Color) value);
                }

                if (objType == typeof(Bounds)) {
                    return EditorGUILayout.BoundsField(content, (Bounds) value);
                }

                if (objType == typeof(Rect)) {
                    return EditorGUILayout.RectField(content, (Rect) value);
                }

                if (typeof(Object).IsAssignableFrom(objType)) {
                    return EditorGUILayout.ObjectField(content, (Object) value, objType, true);
                }

                if (typeof(Enum).IsAssignableFrom(objType)) {
                    return EditorGUILayout.EnumPopup(content, (Enum) value);
                }

                if (objType == typeof(TypeInfo)) {
                    return EditorGUILayout.TextField(content, value.ToString());
                }

                return null;
            }
        }

        public static object Field(Type objType, object value, Rect rect, GUIContent content, bool canWrite = true) {
            using (new EditorGUI.DisabledScope(!canWrite)) {
                if (objType == typeof(bool))
                {
                    return EditorGUI.Toggle(rect, content, (bool)value);
                }
                if (objType == typeof(int))
                {
                    return EditorGUI.IntField(rect, content, (int)value);
                }
                if (objType == typeof(long))
                {
                    return EditorGUI.LongField(rect, content, (long)value);
                }
                if (objType == typeof(float))
                {
                    return EditorGUI.FloatField(rect, content, (float)value);
                }
                if (objType == typeof(double))
                {
                    return EditorGUI.DoubleField(rect, content, (double)value);
                }
                if (objType == typeof(string))
                {
                    return EditorGUI.TextField(rect, content, (string)value);
                }
                if (objType == typeof(Vector2))
                {
                    return EditorGUI.Vector2Field(rect, content, (Vector2)value);
                }
                if (objType == typeof(Vector3))
                {
                    return EditorGUI.Vector3Field(rect, content, (Vector3)value);
                }
                if (objType == typeof(Vector4))
                {
                    return EditorGUI.Vector4Field(rect, content, (Vector4)value);
                }
                if (objType == typeof(Color))
                {
                    return EditorGUI.ColorField(rect, content, (Color)value);
                }
                if (objType == typeof(Bounds))
                {
                    return EditorGUI.BoundsField(rect, content, (Bounds)value);
                }
                if (objType == typeof(Rect))
                {
                    return EditorGUI.RectField(rect, content, (Rect)value);
                }
                if (typeof(Object).IsAssignableFrom(objType))
                {
                    return EditorGUI.ObjectField(rect, content, (Object) value, objType, true);
                }
                if (typeof(Enum).IsAssignableFrom(objType))
                {
                    return EditorGUI.EnumPopup(rect, content, (Enum)value);
                }
                if (typeof(TypeInfo).IsAssignableFrom(objType))
                {
                    return EditorGUI.TextField(rect, content, value.ToString());
                }
            }

            return null;
        }

        private static bool IsInlineProperty(MemberInfo type) {
             var attr = type.GetCustomAttribute<InlinePropertyAttribute>();
             return attr != null;
         }
        #endregion
    }
}