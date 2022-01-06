namespace Frigg {
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class ReadonlyAttribute : BaseDecoratorAttribute {
        
    }
}