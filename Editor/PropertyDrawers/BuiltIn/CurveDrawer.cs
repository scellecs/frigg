namespace Frigg.Editor.BuiltIn {
    using UnityEditor;
    using UnityEngine;

    public class CurveDrawer : BuiltInDrawer {
        public CurveDrawer(FriggProperty prop) : base(prop) {
        }

        public override void DrawLayout() {
            var value  = this.GetTargetValue<AnimationCurve>();
            var result = EditorGUILayout.CurveField(this.property.Label, value);
            this.UpdateAndCallNext(result);
        }

        public override void Draw(Rect rect) {
            var value  = this.GetTargetValue<AnimationCurve>();
            var result = EditorGUI.CurveField(rect, this.property.Label, value);
            this.UpdateAndCallNext(result, rect);
        }

        public override float GetHeight() => EditorGUIUtility.singleLineHeight;

        public override bool IsVisible => true;
    }
}