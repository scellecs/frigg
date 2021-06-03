namespace Frigg.Editor.BuiltIn {
    using UnityEditor;
    using UnityEngine;

    public class FloatDrawer : BuiltInDrawer{
        public FloatDrawer(FriggProperty prop) : base(prop) {
        }

        public override void DrawLayout() {
            var value  = this.GetTargetValue<float>();
            var result = EditorGUILayout.FloatField(this.property.Label, value);
            this.UpdateAndCallNext(result);
        }

        public override void Draw(Rect rect) {
            var value  = this.GetTargetValue<float>();
            var result = EditorGUI.FloatField(rect, this.property.Label, value);
            rect.y += EditorGUIUtility.singleLineHeight;
            this.UpdateAndCallNext(result, rect);
        }

        public override float GetHeight() => EditorGUIUtility.singleLineHeight + GuiUtilities.SPACE;

        public override bool IsVisible => true;
    }
}