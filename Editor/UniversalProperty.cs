namespace Frigg.Editor {
    using System.Collections.Generic;
    using UnityEngine;

    public class UniversalProperty {
        public GUIContent Label { get; set; }

        public UniversalProperty ParentProperty { get; set; }

        public PropertyTree PropertyTree { get; set; }

        private IEnumerable<UniversalProperty> ChildrenProperties { get; set; }
    }
}