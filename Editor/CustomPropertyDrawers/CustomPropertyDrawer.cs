﻿namespace Frigg.Editor {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public abstract class CustomPropertyDrawer {
        public object OnGUI(object target, Rect rect, MemberInfo memberInfo, GUIContent content) {
            var isReadOnly = !CoreUtilities.IsWritable(memberInfo);
            using (new EditorGUI.DisabledScope(isReadOnly)) {
                return this.CreateAndDraw(rect, memberInfo, target, content);
            }
        }
        
        public void OnGUI(Rect rect, SerializedProperty property) {

            var isVisible = CoreUtilities.IsPropertyVisible(property);
            if (!isVisible)
                return;

            var isReadOnly = CoreUtilities.TryGetAttribute<ReadonlyAttribute>(property) != null;
            var isEnabled  = CoreUtilities.IsPropertyEnabled(property) && !isReadOnly;
            
            var content  = CoreUtilities.GetGUIContent(property);
            
            var hideAttr = CoreUtilities.TryGetAttribute<HideLabelAttribute>(property);
            if (hideAttr != null) {
                content.text = string.Empty;
            }

            //We need this to handle CustomProperty for Readonly & Validator behaviour
            using(new EditorGUI.DisabledScope(!isEnabled)){
                EditorGUI.BeginChangeCheck();

                this.CreateAndDraw(rect, property, content);

                if (EditorGUI.EndChangeCheck()) {
                    CoreUtilities.OnDataChanged(property);
                }
            }
        }
        
        protected abstract void CreateAndDraw(Rect rect, SerializedProperty property, GUIContent label);
        protected abstract object CreateAndDraw(Rect rect, MemberInfo member, object target, GUIContent label);
    }
    
    public static class CustomAttributeExtensions {
        private static readonly Dictionary<Type, CustomPropertyDrawer> drawers;

        static CustomAttributeExtensions() =>
            drawers = new Dictionary<Type, CustomPropertyDrawer> {
                [typeof(ReorderableListAttribute)] = ReorderableListDrawer.instance,
                [typeof(InlinePropertyAttribute)]  = InlinePropertyDrawer.instance
            };

        public static CustomPropertyDrawer GetCustomDrawer(CustomAttribute attribute)
            => drawers.TryGetValue(attribute.GetType(), out var drawer) ? drawer : null;
        
        public static CustomPropertyDrawer GetCustomDrawer(Type type)
            => drawers.TryGetValue(type, out var drawer) ? drawer : null;
    }
}