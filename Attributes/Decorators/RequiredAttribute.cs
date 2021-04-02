namespace Packages.Frigg.Attributes {
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RequiredAttribute : BaseDecoratorAttribute {
        public RequiredAttribute() {
            
        }
        
        public RequiredAttribute(string text) {
            this.Text = text;
        }
    }
}