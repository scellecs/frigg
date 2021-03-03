namespace Assets.Scripts.Attributes {
    using System;
    using UnityEngine;

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EnumFlagsAttribute : BaseAttribute{
        public string Name { get; private set; }

        public EnumFlagsAttribute(string name) {
            Name = name;
        }
        
        public EnumFlagsAttribute() {
        }
    }
}