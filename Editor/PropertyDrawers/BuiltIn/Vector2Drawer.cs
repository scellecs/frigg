namespace Frigg.Editor.BuiltIn {
    using UnityEditor;
    using UnityEngine;

    public class Vector2Drawer : BuiltInDrawer {
        public Vector2Drawer(FriggProperty prop) : base(prop) {
        }

        public override void DrawLayout() {
            var value  = this.GetTargetValue<Vector2>();
            var result = EditorGUILayout.Vector2Field(this.property.Label, value);
            this.UpdateAndCallNext(result);
        }

        public override void Draw(Rect rect) {
            var value  = this.GetTargetValue<Vector2>();
            var result = EditorGUI.Vector2Field(rect, this.property.Label, value);
            rect.y += EditorGUIUtility.singleLineHeight;
            this.UpdateAndCallNext(result, rect);
        }

        public override float GetHeight() => EditorGUIUtility.singleLineHeight;

        public override bool IsVisible => true;
        
    }
}