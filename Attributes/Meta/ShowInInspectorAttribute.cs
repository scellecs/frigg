namespace Frigg {
    using System;

    /// <summary>
    /// Allows to force show hidden property's value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class ShowInInspectorAttribute : Attribute {
        
    }
}