namespace Frigg.Editor.BuiltIn {
    using System.Numerics;
    using UnityEditor;
    using UnityEngine;
    using Vector4 = UnityEngine.Vector4;

    public class Vector4Drawer : BuiltInDrawer {
        public Vector4Drawer(FriggProperty prop) : base(prop) {
        }

        public override void DrawLayout() {
            var value  = this.GetTargetValue<Vector4>();
            var result = EditorGUILayout.Vector4Field(this.property.Label, value);
            this.UpdateAndCallNext(result);
        }

        public override void Draw(Rect rect) {
            var value  = this.GetTargetValue<Vector4>();
            var result = EditorGUI.Vector4Field(rect, this.property.Label, value);
            rect.y += EditorGUIUtility.singleLineHeight;
            this.UpdateAndCallNext(result, rect);
        }

        public override float GetHeight() => EditorGUIUtility.singleLineHeight + GuiUtilities.SPACE;

        public override bool IsVisible => true;
        
    }
}