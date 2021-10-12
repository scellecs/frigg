namespace Frigg {
    using System;

    /// <summary>
    /// Draw collection as Reorderable List,
    /// </summary>
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