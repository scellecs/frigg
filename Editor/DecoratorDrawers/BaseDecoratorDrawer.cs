namespace Assets.Scripts.Editor.DecoratorDrawers {
    using System;
    using System.Reflection;
    using Packages.Frigg.Attributes;
    using UnityEditor;
    using UnityEngine;

    public abstract class BaseDecoratorDrawer {
        public       BaseDecoratorAttribute attribute;
        public const float                  SPACE_AMOUNT = 8.0f;

        public SerializedProperty property     = null;
        public MethodInfo         methodInfo   = null;
        public FieldInfo          fieldInfo    = null;
        public PropertyInfo       propertyInfo = null;

        public void OnGUI(Rect rect, object target, IDecoratorAttribute attr) {
            var type = target.GetType();

            if (type == typeof(SerializedProperty)) {
                this.property = (SerializedProperty) target;

                if (attr is RequiredAttribute) {
                    if (this.property.objectReferenceValue != null) {
                        return;
                    }
                }
            }

            if (type == typeof(MethodInfo))
                this.methodInfo = (MethodInfo) target;
            
            if (type == typeof(FieldInfo))
                this.fieldInfo = (FieldInfo) target;
            
            if (type == typeof(PropertyInfo))
                this.propertyInfo = (PropertyInfo) target;

            this.attribute = (BaseDecoratorAttribute) attr;
            
            this.DrawDecorator(rect, target);
        }

        protected abstract float GetHeight(Rect rect);

        protected abstract void DrawDecorator(Rect rect, object target);
    }
}