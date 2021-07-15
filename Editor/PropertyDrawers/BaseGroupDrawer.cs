namespace Frigg.Editor {
    using System.Collections.Generic;
    using Groups;
    using Packages.Frigg.Editor.Utils;
    using UnityEngine;

    public abstract class BaseGroupDrawer : FriggPropertyDrawer {
        private BaseGroupAttribute attribute;

        public BaseGroupDrawer(FriggProperty prop) : base(prop) {
            this.attribute = (BaseGroupAttribute) this.linkedAttribute;
        }
    }
}