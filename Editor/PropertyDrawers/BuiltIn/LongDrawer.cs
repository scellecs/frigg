namespace Frigg.Editor.BuiltIn {
    using UnityEditor;
    using UnityEngine;

    public class LongDrawer : BuiltInDrawer {
        public LongDrawer(FriggProperty prop) : base(prop) {
            
        }

        public override void DrawLayout() {
            var value  = this.GetTargetValue<long>();
            var result = EditorGUILayout.LongField(this.property.Label, value);
            this.UpdateAndCallNext(result);
        }

        public override void Draw(Rect rect) {
            var value  = this.GetTargetValue<long>();
            var result = EditorGUI.LongField(rect, this.property.Label, value);
            rect.y += EditorGUIUtility.singleLineHeight;
            this.UpdateAndCallNext(result, rect);
        }

        public override float GetHeight() => EditorGUIUtility.singleLineHeight;

        public override bool IsVisible => true; 
    }
}