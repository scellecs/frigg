namespace Frigg {
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method,
        Inherited = false, AllowMultiple = true)]
    public class ConditionAttribute : Attribute, IAttribute {
        public string FieldName { get; set; }

        public bool Condition { get; set; } = true;

        public object Value { get; set; }

        public ConditionAttribute(string name) {
            this.FieldName = name;
        }
        
        public ConditionAttribute(string name, object expected) {
            this.FieldName = name;
            this.Value     = expected;
        }
    }
}