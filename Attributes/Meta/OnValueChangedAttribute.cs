namespace Frigg {
    using System;

    public class OnValueChangedAttribute : Attribute, IAttribute {

        public string CallbackMethod { get; private set; }

        public OnValueChangedAttribute(string callback) {
            this.CallbackMethod = callback;
        }
    }
}