namespace Assets.Scripts.Editor.CustomPropertyDrawers {
    using System;
    using System.Collections.Generic;
    using Attributes.Custom;
    using UnityEditor;
    using UnityEngine;

    public abstract class CustomPropertyDrawer {
        public void OnGUI(Rect rect, SerializedProperty property) {
            EditorGUI.BeginChangeCheck();
            
            this.CreateAndDraw(rect, property, new GUIContent(property.name));

            EditorGUI.EndChangeCheck();

            property.serializedObject.Update();
            property.serializedObject.ApplyModifiedProperties();
            
            Undo.RecordObject(property.serializedObject.targetObject, "reorderableList");
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