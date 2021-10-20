﻿namespace Frigg.Editor {
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
            if (rect == Rect.zero) {
                rect = EditorGUILayout.GetControlRect(true);
            }
            
            var attr   = (EnumFlagsAttribute) this.linkedAttribute;
            var target = (Enum) this.property.GetValue();

            if (target == null) {
                Debug.LogError("Invalid target.");
                return;
            }

            var enumValues = EditorGUI.EnumFlagsField(rect, this.property.Label, target);
            CoreUtilities.SetTargetValue(this.property, this.property.ReflectedValue.parent.Value,
                this.property.MetaInfo.MemberInfo, enumValues);
        }

        public override float GetHeight() => EditorGUIUtility.singleLineHeight;

        public override bool IsVisible => true;
    }
}