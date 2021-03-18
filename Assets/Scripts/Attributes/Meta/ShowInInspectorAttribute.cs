namespace Assets.Scripts.Attributes.Custom {
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class ShowInInspectorAttribute : Attribute {
        
    }
}