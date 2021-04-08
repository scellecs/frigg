namespace Packages.Frigg.Attributes.Validators {
    using System;

    public class HideIfAttribute : ValidatorAttribute {
        public HideIfAttribute(string name) : base(name) {
        }

        public HideIfAttribute(string name, object expected) : base(name, expected) {
        }
    }
}