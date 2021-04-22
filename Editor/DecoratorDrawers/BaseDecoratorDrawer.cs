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
                    var propertyType = CoreUtilities.GetPropertyType(this.property);

                    //If objectReferenceValue is null - we need to draw Required InfoBox, otherwise - return.
                    if (this.property.propertyType == SerializedPropertyType.ObjectReference) {
                        if (this.property.objectReferenceValue != null) {
                            return;
                        }
                    }
                    
                    //If value is not default - we don't need to draw Required InfoBox.
                    if (!CoreUtilities.HasDefaultValue(this.property, propertyType)) {
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