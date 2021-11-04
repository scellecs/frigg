namespace Frigg.Editor.BuiltIn {
    using System;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public class EnumDrawer : BuiltInDrawer {
        public EnumDrawer(FriggProperty prop) : base(prop) {
        }

        public override void DrawLayout() {
            this.DoEnumField();
        }

        public override void Draw(Rect rect) {
            this.DoEnumField(rect);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        private void DoEnumField(Rect rect = default) {
            EditorGUI.BeginChangeCheck();
            var target = (Enum) CoreUtilities.GetTargetValue
                (this.property.ParentProperty.GetValue(), this.property.MetaInfo.MemberInfo);
                
            if (target == null) {
                Debug.LogError("Invalid target.");
                return;
            }
            
            var enumValue = rect == default 
                ? EditorGUILayout.EnumPopup(this.property.Label, target)
                : EditorGUI.EnumPopup(rect, this.property.Label, target);

            if (rect != default) {
                rect.y += EditorGUIUtility.singleLineHeight;
            }
            this.UpdateAndCallNext(enumValue, rect);
        }

        public override float GetHeight() => EditorGUIUtility.singleLineHeight;

        public override bool IsVisible => true;
    }
}