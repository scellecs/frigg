namespace Frigg.Editor {
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public class InlinePropertyDrawer : CustomPropertyDrawer {
        public static readonly InlinePropertyDrawer instance = new InlinePropertyDrawer();

        public int labelWidth;
        
        protected override void CreateAndDraw(Rect rect, SerializedProperty property, GUIContent label) {
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

        //TODO: Refactor
        protected override object CreateAndDraw(Rect rect, MemberInfo memberInfo, object target, GUIContent content) {
            var objType = target.GetType();
            var members = new List<(MemberInfo, object)>();
            target.TryGetMembers(members);

            foreach (var (info, member) in members) {
                var writable = CoreUtilities.IsWritable(info);

                var field    = objType.GetField(info.Name);
                if (field != null) {
                    if (typeof(IList).IsAssignableFrom(field.FieldType) && field.FieldType != typeof(string)) {
                        var drawer = (ReorderableListDrawer) CustomAttributeExtensions.GetCustomDrawer(typeof(ReorderableListAttribute));
                        var value  = drawer.OnGUI(member, Rect.zero, field, content);
                        field.SetValue(target, value);
                        continue;
                    }
                    
                    var objValue = field.GetValue(target);
                    if (rect == Rect.zero)
                        field.SetValue(target, GuiUtilities.LayoutField(field.FieldType, objValue, CoreUtilities.GetGUIContent(info), writable));
                    else {
                        field.SetValue(target, GuiUtilities.Field(field.FieldType, objValue, rect, CoreUtilities.GetGUIContent(info), writable));
                        rect.y += EditorGUIUtility.singleLineHeight + GuiUtilities.SPACE;
                    }
                }
                        
                var property = objType.GetProperty(info.Name);
                if (property != null) {
                    if (typeof(IList).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string)) {
                        var drawer = (ReorderableListDrawer) CustomAttributeExtensions.GetCustomDrawer(typeof(ReorderableListAttribute));
                        property.SetValue(target, drawer.OnGUI(property, Rect.zero, property, content));
                        continue;
                    }
                    
                    var objValue = property.GetValue(target);
                    if (objValue == null)
                        return null;
                    property.SetValue(target, GuiUtilities.LayoutField(property.PropertyType, objValue, CoreUtilities.GetGUIContent(info), writable));
                }
            }

            return target;
        }
    }
}