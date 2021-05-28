namespace Frigg {
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DisplayAsString : BaseAttribute {
        public bool Value { get; set; }

        public DisplayAsString(bool value) {
            this.Value = value;
        }
    }
}