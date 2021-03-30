namespace Assets.Scripts.Attributes.Meta {
    using System;
    using Frigg;

    public class OnValueChangedAttribute : Attribute, IAttribute {

        public string callbackMethod { get; private set; }

        public OnValueChangedAttribute(string callback) {
            this.callbackMethod = callback;
        }
    }
}