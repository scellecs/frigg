namespace Frigg {
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method,
        Inherited = false, AllowMultiple = true)]
    public class ValidatorAttribute : Attribute, IAttribute {
        public string FieldName { get; set; }

        public bool Condition { get; set; } = true;

        public object Value { get; set; }

        public ValidatorAttribute(string name) {
            this.FieldName = name;
        }
        
        public ValidatorAttribute(string name, object expected) {
            this.FieldName = name;
            this.Value     = expected;
        }
    }
}