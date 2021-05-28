namespace Frigg.Editor {
    using UnityEditor;
    using UnityEngine;
    
    public class ReadonlyPropertyDrawer : BaseDrawer {
        public ReadonlyPropertyDrawer(FriggProperty prop) : base(prop) {
        }
        
        public override void DrawLayout() {
            using (new EditorGUI.DisabledScope(true)) {
                //foreach (var p in this.property.ChildrenProperties.RecurseChildren()) {
                    this.property.CallNextDrawer();
                //}
            }
        }

        public override void Draw(Rect rect) {
            using (new EditorGUI.DisabledScope(true)) {
                //foreach (var p in this.property.ChildrenProperties.RecurseChildren()) {
                    this.property.CallNextDrawer(rect);
                //}
            }
        }

        public override float GetHeight() => 0f;

        public override bool IsVisible => true;
    }
}