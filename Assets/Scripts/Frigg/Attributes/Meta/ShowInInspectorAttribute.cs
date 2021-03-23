namespace Assets.Scripts.Attributes.Meta {
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class ShowInInspectorAttribute : Attribute {
        
    }
}