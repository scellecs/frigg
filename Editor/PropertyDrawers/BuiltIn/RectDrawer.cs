namespace Frigg.Editor.BuiltIn {
    using UnityEditor;
    using UnityEngine;

    public class RectDrawer : BuiltInDrawer {
        public RectDrawer(FriggProperty prop) : base(prop) {
        }

        public override void DrawLayout() {
            var value  = this.GetTargetValue<Rect>();
            var result = EditorGUILayout.RectField(this.property.Label, value);
            this.UpdateAndCallNext(result);
        }

        public override void Draw(Rect rect) {
            var value  = this.GetTargetValue<Rect>();
            var result = EditorGUI.RectField(rect, this.property.Label, value);
            this.UpdateAndCallNext(result, rect);
        }

        public override float GetHeight() => EditorGUIUtility.singleLineHeight + GuiUtilities.SPACE;

        public override bool IsVisible => true;
        
    }
}