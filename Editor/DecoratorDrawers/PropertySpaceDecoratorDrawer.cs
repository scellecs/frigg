namespace Frigg.Editor {
    using UnityEditor;
    using UnityEngine;

    public class PropertySpaceDecoratorDrawer : FriggDecoratorDrawer {
        public override void DrawLayout() {
            var attr = (PropertySpaceAttribute) this.linkedAttribute;
            EditorGUILayout.Space(attr.SpaceBefore);
            this.property.CallNextDrawer();
        }

        public override void Draw(Rect rect) {
            var attr = (PropertySpaceAttribute) this.linkedAttribute;
            rect.y += attr.SpaceBefore;
            this.property.CallNextDrawer(rect);
        }

        public override bool  IsVisible   => true;
        public override float GetHeight() => ((PropertySpaceAttribute) this.linkedAttribute).SpaceBefore;
    }
}