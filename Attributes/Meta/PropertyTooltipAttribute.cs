namespace Frigg {
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, 
        AllowMultiple = true)]
    public class PropertyTooltipAttribute : Attribute, IAttribute {
        public string Text { get; private set; }

        public PropertyTooltipAttribute(string text) {
            this.Text = text;
        }
    }
}