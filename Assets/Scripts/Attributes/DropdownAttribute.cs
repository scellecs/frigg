namespace Assets.Scripts.Attributes {
    using System;
    using UnityEngine;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DropdownAttribute : PropertyAttribute, IAttribute {

        public string Name { get; private set; }

        public DropdownAttribute(string name) {
            Name = name;
        }
    }
}