namespace Frigg.Editor {
    using System;
    using Packages.Frigg.Editor.Utils;
    using UnityEngine;
    using UnityEditor;
    using Utils;

    public abstract class BaseDrawer : FriggPropertyDrawer {
        private Type drawerType;
        protected BaseDrawer(FriggProperty prop) : base(prop) {
            if (prop.TryGetFixedAttribute<HideLabelAttribute>() != null) {
                prop.Label = GUIContent.none;
            }
        }

        protected T GetTargetValue<T>() {
            this.drawerType = typeof(T);
            EditorGUI.BeginChangeCheck();
            return (T) this.property.GetValue();
        }
        
        protected void UpdateAndCallNext(object value, Rect rect = default) {
            if (EditorGUI.EndChangeCheck()) {
                this.property.UpdateValue(value);
                CoreUtilities.OnValueChanged(this.property);
                
                if (!Application.isPlaying) {
                    if (this.property.NativeProperty != null) {
                        var prop             = this.property.NativeProperty;
                        var serializedObject = prop.serializedObject;
                        CoreUtilities.SetSerializedPropertyValue(prop, this.drawerType, value);
                        
                        Undo.RegisterCompleteObjectUndo(serializedObject.targetObject,
                            $"'update {prop.name} value to {value}'");
                        Undo.FlushUndoRecordObjects();
                        
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
            this.CallNext(rect);
        }
        
        protected void CallNext(Rect rect = default) {
            this.property.CallNextDrawer(rect);
        }
    }
}