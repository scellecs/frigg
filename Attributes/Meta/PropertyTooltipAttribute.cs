namespace Frigg {
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, 
        AllowMultiple = true)]
    public class PropertyTooltipAttribute : Attribute, IAttribute {
        public string Text      { get; set; }
        public bool   IsDynamic { get; set; }

        public PropertyTooltipAttribute(string text, bool isDynamic = false) {
            this.Text      = text;
            this.IsDynamic = isDynamic;
        }
    }
}