namespace Frigg.Groups {
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
    public class BaseGroupAttribute : Attribute, IAttribute {
        public string GroupName { get; set; }

        public BaseGroupAttribute() {
            
        }

        public BaseGroupAttribute(string name) {
            this.GroupName = name;
        }
    }
}