namespace Packages.Frigg.Attributes {
    using System;

    public class BaseDecoratorAttribute : Attribute, IDecoratorAttribute {
        public string Text { get; set; }
    }
}