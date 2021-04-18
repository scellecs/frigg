namespace Frigg.Editor {
    using UnityEditor;
    using UnityEngine;

    public class PropertySpaceDecoratorDrawer : BaseDecoratorDrawer {
        protected override float GetHeight(Rect rect) {
            var attr = (PropertySpaceAttribute) this.attribute;
            
            return attr.SpaceBefore;
        }

        protected override void DrawDecorator(Rect rect, object target, bool isArray) {
            var attr = (PropertySpaceAttribute) this.attribute;
            
            EditorGUILayout.Space(this.GetHeight(rect));
        }
    }
}