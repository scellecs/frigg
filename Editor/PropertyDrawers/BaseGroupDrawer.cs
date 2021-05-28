namespace Frigg.Editor {
    using System.Collections.Generic;
    using Groups;
    using Packages.Frigg.Editor.Utils;
    using UnityEngine;

    public abstract class BaseGroupDrawer : FriggPropertyDrawer {
        private BaseGroupAttribute attribute;
        
        private Rect baseRect = Rect.zero;
        private Rect currentRect;

        public static Dictionary<GroupInfo, BaseGroupDrawer> activeGroups = new Dictionary<GroupInfo, BaseGroupDrawer>();
        
        public BaseGroupDrawer(FriggProperty prop) : base(prop) {
            this.attribute = (BaseGroupAttribute) this.linkedAttribute;
        }
    }
}