namespace Frigg {
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class OrderAttribute : Attribute, IAttribute {
        public int Order { get; private set; }

        public OrderAttribute(int order = 0) {
            this.Order = order;
        }
    }
}