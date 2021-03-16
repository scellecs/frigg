namespace Assets.Scripts.Attributes.Meta {
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class OrderAttribute : Attribute, IAttribute {
        public OrderAttribute(int order = 0) {
            
        }
    }
}