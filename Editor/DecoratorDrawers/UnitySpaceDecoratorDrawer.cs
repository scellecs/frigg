namespace Frigg.Editor {
    using UnityEditor;
    using UnityEngine;

    public class UnitySpaceDecoratorDrawer : FriggDecoratorDrawer {
        public override void DrawLayout() {
            var attr = (SpaceAttribute) this.linkedAttribute;
            EditorGUILayout.Space(attr.height);
            this.property.CallNextDrawer();
        }

        public override void Draw(Rect rect) {
            throw new System.NotImplementedException();
        }

        public override bool  IsVisible   => true;
        public override float GetHeight() =>  ((SpaceAttribute) this.linkedAttribute).height;
        
    }
}