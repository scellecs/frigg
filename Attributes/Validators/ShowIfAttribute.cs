namespace Packages.Frigg.Attributes.Validators {
    using System;

    public class ShowIfAttribute : ValidatorAttribute {
        public ShowIfAttribute(string name) : base(name) {
        }

        public ShowIfAttribute(string name, object expected) : base(name, expected) {
        }
    }
}