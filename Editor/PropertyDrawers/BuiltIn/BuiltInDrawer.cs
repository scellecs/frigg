namespace Frigg.Editor.BuiltIn {
    using System;
    using System.Collections;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public abstract class BuiltInDrawer : BaseDrawer {
        protected BuiltInDrawer(FriggProperty prop) : base(prop) {
        }

        protected T GetTargetValue<T>() {
            object target;
            
            //Optimize: Assign target as a property value.
            target = this.property.ParentProperty.MetaInfo.isArray
                ? ((IList) this.property.ParentValue)[this.property.MetaInfo.arrayIndex] 
                : CoreUtilities.GetTargetObject(this.property.ParentValue, this.PropertyMeta.MemberInfo);
            
            return (T) target;
        }

        protected void UpdateAndCallNext(object value, Rect rect = default) {
            this.Update(value);
            this.CallNext(rect);
        }

        protected void CallNext(Rect rect = default) {
            this.property.CallNextDrawer(rect);
        }

        protected void Update(object value) {
            CoreUtilities.SetTargetValue(this.property, this.property.ParentValue,
                this.PropertyMeta.MemberInfo, value);
            
            this.property.PropertyTree.SerializedObject.UpdateIfRequiredOrScript();
            
            this.property.PropertyTree.SerializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}