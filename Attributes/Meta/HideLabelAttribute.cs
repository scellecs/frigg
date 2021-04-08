namespace Frigg
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, 
        AllowMultiple = true)]
    public class HideLabelAttribute : Attribute, IAttribute {
        
    }
}