namespace Frigg.Editor.BuiltIn {
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public class StringDrawer : BuiltInDrawer {
        private readonly bool asString;
        public StringDrawer(FriggProperty prop) : base(prop) {
            var attr = prop.TryGetFixedAttribute<DisplayAsString>();
            if (attr != null) {
                this.asString = attr.Value;
            }
        }
        
        public override void DrawLayout() {
            var value  = this.GetTargetValue<string>();
            if (!this.asString) {
                EditorGUILayout.LabelField(value);
                this.CallNext();
                return;

            }
            
            var result = EditorGUILayout.TextField(this.property.Label, value);
            this.UpdateAndCallNext(result);
        }

        public override void Draw(Rect rect) {
            var value = this.GetTargetValue<string>();
            if (!this.asString) {
                EditorGUI.LabelField(rect, value);
                this.CallNext(rect);
                return;

            }
            
            var result = EditorGUI.TextField(rect, this.property.Label, value);
            rect.y += EditorGUIUtility.singleLineHeight;
            this.UpdateAndCallNext(result, rect);
        }

        public override float GetHeight() => EditorGUIUtility.singleLineHeight;

        public override bool IsVisible => true;

    }
}