namespace Frigg
{
    using System;

    /// <summary>
    /// Force draw property without label.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, 
        AllowMultiple = true)]
    public class HideLabelAttribute : Attribute, IAttribute {
        
    }
}