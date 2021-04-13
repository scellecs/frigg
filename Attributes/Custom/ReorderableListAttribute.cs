namespace Frigg {
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ReorderableListAttribute : CustomAttribute {
        public string Name { get; private set; }

        public ReorderableListAttribute(string name) {
            this.Name = name;
        }
        
        public ReorderableListAttribute() {
        }
    }
}