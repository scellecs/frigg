namespace Assets.Scripts.Attributes.CustomAttribute {
    using System;

    [AttributeUsage(AttributeTargets.Field)]
    public class ReorderableListAttribute : Attribute, IAttribute {
        public string Name { get; private set; }

        public ReorderableListAttribute(string name) {
            this.Name = name;
        }
        
        public ReorderableListAttribute() {
        }
    }
}