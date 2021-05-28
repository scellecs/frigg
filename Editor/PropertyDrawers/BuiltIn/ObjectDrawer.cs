namespace Frigg.Editor.BuiltIn {
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public class ObjectDrawer : BuiltInDrawer {
        public ObjectDrawer(FriggProperty prop) : base(prop) {
        }

        public override void DrawLayout() {
            var value  = this.GetTargetValue<Object>();
            var result = EditorGUILayout.ObjectField(this.property.Label, value, this.property.MetaInfo.MemberType, true);
            this.UpdateAndCallNext(result);
        }

        public override void Draw(Rect rect) {
            var value  = this.GetTargetValue<Object>();
            var result = EditorGUI.ObjectField(rect, this.property.Label, value, this.property.MetaInfo.MemberType, true);
            rect.y += EditorGUIUtility.singleLineHeight;
            this.UpdateAndCallNext(result, rect);
        }

        public override float GetHeight() => EditorGUIUtility.singleLineHeight;

        public override bool IsVisible => true;
    }
}