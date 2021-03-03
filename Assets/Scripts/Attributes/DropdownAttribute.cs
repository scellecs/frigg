namespace Assets.Scripts.Attributes {
    using System;
    using UnityEngine;

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DropdownAttribute : BaseAttribute {
        public string Name { get; private set; }

        public DropdownAttribute(string name) {
            Name = name;
        }
    }
}