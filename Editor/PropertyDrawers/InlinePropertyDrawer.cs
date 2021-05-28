namespace Frigg.Editor {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public class InlinePropertyDrawer : FriggPropertyDrawer {
        public static readonly InlinePropertyDrawer instance = new InlinePropertyDrawer();
        public                 int                  labelWidth;

        public override void DrawLayout() {
            throw new NotImplementedException();
        }
        /*protected override void CreateAndDrawLayout(SerializedProperty property, GUIContent label) {
            throw new NotImplementedException();
        }

        protected override void CreateAndDraw(SerializedProperty property, GUIContent label) {
            var cachedWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = this.labelWidth;
            
            EditorGUILayout.BeginHorizontal();
            if(label != GUIContent.none)
               EditorGUILayout.LabelField(label);

            var copy       = property.Copy(); //we need to work with a property copy
            var enumerator = copy.GetEnumerator();
            
            while (enumerator.MoveNext()) {
                var prop = (SerializedProperty) enumerator.Current;
                EditorGUILayout.PropertyField(prop);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = cachedWidth;
        }

        protected override object CreateAndDrawLayout(MemberInfo memberInfo, object target, GUIContent content) {
            if (target == null) {
                return null;
            }

            /*var       objType    = target.GetType();
            var       members    = new List<Member>();
            target.TryGetMembers(members);
            var enumerator = members.GetEnumerator();

            while (enumerator.MoveNext()) {
                var member = enumerator.Current;
                if (member == null)
                    return null;
                
                var writable = CoreUtilities.IsWritable(member.memberInfo);
                var field    = objType.GetField(member.memberInfo.Name, CoreUtilities.FLAGS);

                if (field != null) {
                    if (typeof(IList).IsAssignableFrom(field.FieldType) && field.FieldType != typeof(string)) {
                        var drawer = (ReorderableListDrawer) CustomAttributeExtensions.GetCustomDrawer(typeof(ReorderableListAttribute));
                        var value  = drawer.OnGUI(Rect.zero, member.target, field, content);
                        field.SetValue(target, value);
                        continue;
                    }
                    
                    if (!CoreUtilities.IsPrimitiveUnityType(field.FieldType)) {
                        var value = field.GetValue(member.target);
                        field.SetValue(member.target, GuiUtilities.MultiFieldLayout(field, value, content));
                        continue;
                    }
                    
                    var objValue = field.GetValue(target);
                    field.SetValue(target, GuiUtilities.FieldLayout(field.FieldType, objValue, CoreUtilities.GetGUIContent(member.memberInfo), writable));
                    continue;
                }
                        
                var property = objType.GetProperty(member.memberInfo.Name, CoreUtilities.FLAGS);
                if (property != null) {
                    if (typeof(IList).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string)) {
                        var drawer = (ReorderableListDrawer) CustomAttributeExtensions.GetCustomDrawer(typeof(ReorderableListAttribute));
                        var value  = drawer.OnGUI(Rect.zero, member.target, property, content);
                        property.SetValue(target, value);
                        continue;
                    }
                    
                    if (!CoreUtilities.IsPrimitiveUnityType(property.PropertyType)) {
                        var value = property.GetValue(member.target);
                        property.SetValue(member.target, GuiUtilities.MultiFieldLayout(property, value, content));
                        continue;
                    }

                    var objValue = property.GetValue(target);
                    if (!property.CanWrite) {
                        GuiUtilities.FieldLayout(property.PropertyType, objValue, CoreUtilities.GetGUIContent(member.memberInfo), writable);
                        continue;
                    }
                    
                    property.SetValue(target, GuiUtilities.FieldLayout(property.PropertyType, objValue, CoreUtilities.GetGUIContent(member.memberInfo), writable));
                }
            }
            
            enumerator.Dispose();#1#

            return target;
        }

        //TODO: Refactor
        protected override object CreateAndDraw(Rect rect, MemberInfo memberInfo, object target, GUIContent content) {
            /*if (target == null)
                return null;

            var objType = target.GetType();
            var members = new List<Member>();
            target.TryGetMembers(members);
            
            var enumerator = members.GetEnumerator();
            while (enumerator.MoveNext()) {
                var member = enumerator.Current;
                if (member == null)
                    return null;
                
                var writable = CoreUtilities.IsWritable(member.memberInfo);
                var field    = objType.GetField(member.memberInfo.Name, CoreUtilities.FLAGS);

                if (field != null) {
                    if (typeof(IList).IsAssignableFrom(field.FieldType) && field.FieldType != typeof(string)) {
                        var drawer = (ReorderableListDrawer) CustomAttributeExtensions.GetCustomDrawer(typeof(ReorderableListAttribute));
                        var value  = drawer.OnGUI(rect, member.target, field, content);
                        field.SetValue(target, value);
                        continue;
                    }
                    
                    if (!CoreUtilities.IsPrimitiveUnityType(field.FieldType)) {
                        var value = field.GetValue(member.target);
                        field.SetValue(member.target, GuiUtilities.MultiField(rect, field, value, content));
                        continue;
                    }
                    
                    var objValue = field.GetValue(target);
                    field.SetValue(target, GuiUtilities.Field(field.FieldType, objValue, rect, CoreUtilities.GetGUIContent(member.memberInfo), writable));
                    rect.y      += (EditorGUIUtility.singleLineHeight + GuiUtilities.SPACE);
                    continue;
                }
                        
                var property = objType.GetProperty(member.memberInfo.Name, CoreUtilities.FLAGS);
                if (property != null) {
                    if (typeof(IList).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string)) {
                        var drawer = (ReorderableListDrawer) CustomAttributeExtensions.GetCustomDrawer(typeof(ReorderableListAttribute));
                        var value  = drawer.OnGUI(rect, member.target, property, content);
                        property.SetValue(target, value);
                        continue;
                    }
                    
                    if (!CoreUtilities.IsPrimitiveUnityType(property.PropertyType)) {
                        var value = property.GetValue(member.target);
                        property.SetValue(member.target, GuiUtilities.MultiField(rect, property, value, content));
                        continue;
                    }

                    var objValue = property.GetValue(target);
                    if (!property.CanWrite) {
                        GuiUtilities.Field(property.PropertyType, objValue, rect,  CoreUtilities.GetGUIContent(member.memberInfo), writable);
                        rect.y += (EditorGUIUtility.singleLineHeight + GuiUtilities.SPACE);
                        continue;
                    }
                    
                    property.SetValue(target, GuiUtilities.Field(property.PropertyType, objValue, rect, CoreUtilities.GetGUIContent(member.memberInfo), writable));
                    rect.y += (EditorGUIUtility.singleLineHeight + GuiUtilities.SPACE);
                }
            }
            
            enumerator.Dispose();
            #1#

            return target;*/
    }
}