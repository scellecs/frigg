namespace Frigg.Editor.BuiltIn {
    using UnityEditor;
    using UnityEngine;

    public class GradientDrawer : BuiltInDrawer {
        public GradientDrawer(FriggProperty prop) : base(prop) {
        }

        public override void DrawLayout() {
            var value  = this.GetTargetValue<Gradient>();
            var result = EditorGUILayout.GradientField(this.property.Label, value);
            this.UpdateAndCallNext(result);
        }

        public override void Draw(Rect rect) {
            var value  = this.GetTargetValue<Gradient>();
            var result = EditorGUI.GradientField(rect, this.property.Label, value);
            this.UpdateAndCallNext(result, rect);
        }

        public override float GetHeight() => 0;

        public override bool IsVisible => true;
    }
}