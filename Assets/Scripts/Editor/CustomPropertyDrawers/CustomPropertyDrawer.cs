namespace Assets.Scripts.Editor.CustomPropertyDrawers {
    using System;
    using System.Collections.Generic;
    using Attributes;
    using Attributes.Custom;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public abstract class CustomPropertyDrawer {
        public void OnGUI(Rect rect, SerializedProperty property) {

            var isVisible = CoreUtilities.TryGetAttribute<ReadonlyAttribute>(property);

            var isDisabled = isVisible == null;

            //We need this to handle CustomProperty for Readonly behaviour
            using(new EditorGUI.DisabledScope(!isDisabled)){
                EditorGUI.BeginChangeCheck();

                this.CreateAndDraw(rect, property, new GUIContent(property.name));

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
                [typeof(ReorderableListAttribute)] = ReorderableListDrawer.instance
            };

        public static CustomPropertyDrawer GetCustomDrawer(this CustomAttribute attribute)
            => drawers.TryGetValue(attribute.GetType(), out var drawer) ? drawer : null;
    }
}