namespace Frigg.Groups {
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
    public class VerticalGroupAttribute : BaseGroupAttribute {
        public VerticalGroupAttribute(string name) : base(name) {
            
        }
        
        public VerticalGroupAttribute(string name, int width) : base(name, width) {
            
        }
    }
}