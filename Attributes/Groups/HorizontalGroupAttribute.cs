namespace Frigg.Groups {
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
    public class HorizontalGroupAttribute : BaseGroupAttribute {
        public HorizontalGroupAttribute(string name) : base(name) {
            
        }
        
        public HorizontalGroupAttribute(string name, int width) : base(name, width) {
            
        }
    }
}