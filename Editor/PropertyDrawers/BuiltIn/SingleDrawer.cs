namespace Frigg.Editor.BuiltIn {
    using System;
    using UnityEditor;
    using UnityEngine;

    public class SingleDrawer : BuiltInDrawer{
        public SingleDrawer(FriggProperty prop) : base(prop) {
        }

        public override void DrawLayout() {
            var value  = this.GetTargetValue<double>();
            var result = EditorGUILayout.DoubleField(this.property.Label, value);
            this.UpdateAndCallNext(result);
        }

        public override void Draw(Rect rect) {
            var value  = this.GetTargetValue<double>();
            var result = EditorGUI.DoubleField(rect, this.property.Label, value);
            rect.y += EditorGUIUtility.singleLineHeight;
            this.UpdateAndCallNext(result, rect);
        }

        public override float GetHeight() => EditorGUIUtility.singleLineHeight;

        public override bool IsVisible => true;
    }
}