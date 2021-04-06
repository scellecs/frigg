namespace Assets.Scripts.Attributes.Meta {
    using System;
    using Frigg;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class PropertyTooltipAttribute : Attribute, IAttribute {
        public string Text { get; private set; }

        public PropertyTooltipAttribute(string text) {
            this.Text = text;
        }
    }
}