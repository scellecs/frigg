namespace Assets.Scripts.Attributes {
    using System;

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ReadonlyAttribute : BaseAttribute {
        
    }
}