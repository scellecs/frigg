namespace Frigg.Editor.Layouts {
    using UnityEngine;

    public class VerticalLayout : Layout {
        public override void Add(FriggProperty property) {
            base.Add(property);
            
        }

        protected override void Recalculate() {
            throw new System.NotImplementedException();
        }
    }
}