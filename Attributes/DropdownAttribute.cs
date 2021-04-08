namespace Frigg {
    using System;

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DropdownAttribute : BaseAttribute {
        public string Name { get; private set; }

        public DropdownAttribute(string name) {
            this.Name = name;
        }
    }
}