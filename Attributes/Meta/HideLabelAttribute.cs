namespace Frigg
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, 
        AllowMultiple = true)]
    public class HideLabelAttribute : Attribute, IAttribute {
        
    }
}