namespace Frigg.Editor.BuiltIn {
    using UnityEditor;
    using UnityEngine;

    public class ColorDrawer : BuiltInDrawer {
        public ColorDrawer(FriggProperty prop) : base(prop) {
        }

        public override void DrawLayout() {
            var value  = this.GetTargetValue<Color>();
            var result = EditorGUILayout.ColorField(this.property.Label, value);
            this.UpdateAndCallNext(result);
        }

        public override void Draw(Rect rect) {
            var value  = this.GetTargetValue<Color>();
            var result = EditorGUI.ColorField(rect, this.property.Label, value);
            this.UpdateAndCallNext(result, rect);
        }

        public override float GetHeight() => EditorGUIUtility.singleLineHeight;

        public override bool IsVisible => true;
    }
}