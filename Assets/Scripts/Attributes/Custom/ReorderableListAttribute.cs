namespace Assets.Scripts.Attributes.Custom {
    using System;

    [AttributeUsage(AttributeTargets.Field)]
    public class ReorderableListAttribute : CustomAttribute {
        public string Name { get; private set; }

        public ReorderableListAttribute(string name) {
            this.Name = name;
        }
        
        public ReorderableListAttribute() {
        }
    }
}