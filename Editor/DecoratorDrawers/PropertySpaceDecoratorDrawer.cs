namespace Assets.Scripts.Editor.DecoratorDrawers {
    using Packages.Frigg.Attributes;
    using UnityEditor;
    using UnityEngine;

    public class PropertySpaceDecoratorDrawer : BaseDecoratorDrawer {
        protected override float GetHeight(Rect rect) {
            var attr = (PropertySpaceAttribute) this.attribute;
            
            return attr.SpaceBefore;
        }

        protected override void DrawDecorator(Rect rect, object target) {
            var attr = (PropertySpaceAttribute) this.attribute;
            
            EditorGUILayout.Space(this.GetHeight(rect));
        }
    }
}