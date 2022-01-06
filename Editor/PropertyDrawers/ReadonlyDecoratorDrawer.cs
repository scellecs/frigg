namespace Frigg.Editor {
    using UnityEditor;
    using UnityEngine;
    
    public class ReadonlyDecoratorDrawer : FriggDecoratorDrawer {
        public override void DrawLayout() {
            using (new EditorGUI.DisabledScope(true)) {
                this.property.CallNextDrawer();
            }
        }

        public override void Draw(Rect rect) {
            using (new EditorGUI.DisabledScope(true)) {
                this.property.CallNextDrawer(rect);
            }
        }

        public override float GetHeight() => 0f;

        public override bool IsVisible => true;
    }
}