namespace Frigg.Editor {
    using UnityEngine;

    public class VerticalGroupDrawer : BaseGroupDrawer {
        public VerticalGroupDrawer(FriggProperty prop) : base(prop) {
        }
        
        public override void DrawLayout() {
            throw new System.NotImplementedException();
        }

        public override void Draw(Rect rect) {
            throw new System.NotImplementedException();
        }

        public override float GetHeight() => throw new System.NotImplementedException();

        public override bool IsVisible => true;
    }
}