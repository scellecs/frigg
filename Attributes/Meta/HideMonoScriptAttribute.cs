namespace Frigg {
    using System;

    /// <summary>
    /// Hide MonoScript property on target object. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class HideMonoScriptAttribute : Attribute, IAttribute {
        
    }
}