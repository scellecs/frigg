namespace Frigg.Groups {
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
    public class BaseGroupAttribute : Attribute, IAttribute, IGroupAttribute {
        public string GroupName  { get; set; }
        public int    ElementWidth { get; set; }

        public BaseGroupAttribute() {
            
        }

        public BaseGroupAttribute(string name) => this.GroupName = name;

        public BaseGroupAttribute(string name, int width) {
            this.GroupName    = name;
            this.ElementWidth = width;
        }
    }
}