namespace Frigg.Editor {
    using System;
    using System.Reflection;
    using Frigg;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public abstract class BaseDecoratorDrawer {
        public       BaseDecoratorAttribute attribute;
        public const float                  SPACE_AMOUNT = 8.0f;

        public SerializedProperty property     = null;
        public MethodInfo         methodInfo   = null;
        public FieldInfo          fieldInfo    = null;
        public PropertyInfo       propertyInfo = null;

        public abstract bool IsVisible(SerializedProperty prop);

        public void OnGUI(Rect rect, object target, IDecoratorAttribute attr, bool isArray = false) {
            var type = target.GetType();

            this.attribute = (BaseDecoratorAttribute) attr;
            
            if (type == typeof(SerializedProperty)) {
                this.property = (SerializedProperty) target;
                var  propTarget = CoreUtilities.GetTargetObjectWithProperty(this.property);
                bool boolValue;
                
                var field  = propTarget.TryGetField(this.attribute.Member);
                if (field != null) {
                    boolValue = (bool) field.GetValue(propTarget);
                    if (!boolValue)
                        return;
                }
                
                var prop = propTarget.TryGetProperty(this.attribute.Member);
                if (prop != null) {
                    boolValue = (bool) prop.GetValue(propTarget);
                    if (!boolValue)
                        return;
                }

                if (attr is RequiredAttribute) {
                    if (!this.IsVisible(this.property)) {
                        return;
                    }
                }
            }
            
            this.DrawDecorator(rect, target, isArray);
            
        }

        protected abstract float GetHeight(Rect rect);

        protected abstract void DrawDecorator(Rect rect, object target, bool isArray);
    }
}