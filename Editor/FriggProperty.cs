namespace Frigg.Editor {
    using System.Collections.Generic;
    using UnityEngine;

    public class FriggProperty {
        public GUIContent Label { get; set; }

        public FriggProperty ParentProperty { get; set; }

        public PropertyTree PropertyTree { get; set; }

        private IEnumerable<FriggProperty> ChildrenProperties { get; set; }
    }
}