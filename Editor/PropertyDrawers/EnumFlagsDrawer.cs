namespace Frigg.Editor {
    using System;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEngine;
    using Utils;
    
    public class EnumFlagsDrawer : BaseDrawer {
        public EnumFlagsDrawer(FriggProperty prop) : base(prop) {
        }
        public override void DrawLayout() {
            this.DoEnumFlags();
        }

        public override void Draw(Rect rect) {
            this.DoEnumFlags(rect);
            rect.y += EditorGUIUtility.singleLineHeight;
            this.property.CallNextDrawer(rect);
        }

        private void DoEnumFlags(Rect rect = default) {
            EditorGUI.BeginChangeCheck();
            
            var attr   = (EnumFlagsAttribute) this.linkedAttribute;
            var target = (Enum) this.property.GetValue();

            if (target == null) {
                Debug.LogError("Invalid target.");
                return;
            }

            var enumValues = rect == default 
                ? EditorGUILayout.EnumFlagsField(this.property.Label, target)
                : EditorGUI.EnumFlagsField(rect, this.property.Label, target);

            if (rect != default) {
                rect.y += EditorGUIUtility.singleLineHeight;
            }
            this.UpdateAndCallNext(enumValues, rect);
        }

        public override float GetHeight() => EditorGUIUtility.singleLineHeight;

        public override bool IsVisible => true;
    }
}