namespace Frigg.Editor.BuiltIn {
    using System;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public class EnumDrawer : BuiltInDrawer {
        public EnumDrawer(FriggProperty prop) : base(prop) {
        }

        public override void DrawLayout() {
            this.DoEnumField();
            this.property.CallNextDrawer();
        }

        public override void Draw(Rect rect) {
            this.DoEnumField(rect);
            rect.y += EditorGUIUtility.singleLineHeight;
            this.property.CallNextDrawer(rect);
        }

        private void DoEnumField(Rect rect = default) {
            if (rect == Rect.zero) {
                rect = EditorGUILayout.GetControlRect(true);
            }
            
            //var target = (Enum) this.property.PropertyValue.Value;

            var target = (Enum) CoreUtilities.GetTargetValue(this.property.ParentValue, this.property.MetaInfo.MemberInfo);
                
            if (target == null) {
                Debug.LogError("Invalid target.");
                return;
            }
            
            var enumValue = EditorGUI.EnumPopup(rect, this.property.Label, target);
            this.property.PropertyTree.SerializedObject.ApplyModifiedProperties();
            
            this.property.Update(enumValue);
        }

        public override float GetHeight() => EditorGUIUtility.singleLineHeight + GuiUtilities.SPACE;

        public override bool IsVisible => true;
    }
}