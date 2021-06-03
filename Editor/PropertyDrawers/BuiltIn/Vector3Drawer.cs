namespace Frigg.Editor.BuiltIn {
    using UnityEditor;
    using UnityEngine;

    public class Vector3Drawer : BuiltInDrawer {
        public Vector3Drawer(FriggProperty prop) : base(prop) {
        }

        public override void DrawLayout() {
            var value  = this.GetTargetValue<Vector3>();
            var result = EditorGUILayout.Vector3Field(this.property.Label, value);
            this.UpdateAndCallNext(result);
        }

        public override void Draw(Rect rect) {
            var value  = this.GetTargetValue<Vector3>();
            var result = EditorGUI.Vector3Field(rect, this.property.Label, value);
            rect.y += EditorGUIUtility.singleLineHeight;
            this.UpdateAndCallNext(result, rect);
        }

        public override float GetHeight() => EditorGUIUtility.singleLineHeight + GuiUtilities.SPACE;

        public override bool IsVisible => true;
        
    }
}