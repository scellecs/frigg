using System;
using Frigg;

namespace Assets.Scripts.Attributes.Meta
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class HideLabelAttribute : Attribute, IAttribute
    {
        
    }
}