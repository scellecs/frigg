namespace Frigg {
    using System;

    public class BaseDecoratorAttribute : Attribute, IDecoratorAttribute {
        public const int   DEFAULT_HEIGHT = 38;
        public float Height { get; set; } = DEFAULT_HEIGHT;
        
        public string Text   { get; set; }
        public string Member { get; set; } = null;
        
        public bool HasCustomHeight => (int)this.Height != DEFAULT_HEIGHT;
    }
}