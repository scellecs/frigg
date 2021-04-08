namespace Frigg.Editor {
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public abstract class CustomPropertyDrawer {
        public void OnGUI(Rect rect, SerializedProperty property) {

            var isVisible = CoreUtilities.IsPropertyVisible(property);
            if (!isVisible)
                return;

            var isEnabled = CoreUtilities.IsPropertyEnabled(property);
            
            var content  = CoreUtilities.GetGUIContent(property);
            
            var hideAttr = CoreUtilities.TryGetAttribute<HideLabelAttribute>(property);
            if (hideAttr != null) {
                content.text = string.Empty;
            }
            
            //We need this to handle CustomProperty for Readonly & Validator behaviour
            using(new EditorGUI.DisabledScope(!isEnabled)){
                EditorGUI.BeginChangeCheck();

                if (this.GetType() == typeof(InlinePropertyDrawer)) {
                    var drawer = (InlinePropertyDrawer)
                        CustomAttributeExtensions.GetCustomDrawer(typeof(InlinePropertyAttribute));

                    var attr = CoreUtilities.TryGetAttribute<InlinePropertyAttribute>
                        (property);

                    if (attr != null) {
                        drawer.labelWidth = attr.LabelWidth;
                    }

                    else {
                        var type = CoreUtilities.GetPropertyType(property);
                        attr = (InlinePropertyAttribute) Attribute.GetCustomAttribute(type,
                            typeof(InlinePropertyAttribute));
                        drawer.labelWidth = attr.LabelWidth;
                    }

                    this.CreateAndDraw(rect, property, content);
                }

                if (this.GetType() == typeof(ReorderableListDrawer)) {
                    this.CreateAndDraw(rect, property, content);
                }

                if (EditorGUI.EndChangeCheck()) {
                    CoreUtilities.OnDataChanged(property);
                }
            }
        }
        
        protected abstract void CreateAndDraw(Rect rect, SerializedProperty property, GUIContent label);
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