namespace Frigg {
    using System;
    using UnityEngine.UI;
    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Struct | AttributeTargets.Property)]
    public class LabelText : Attribute, IAttribute{
        public string Text   { get; set; }
        public bool IsDynamic { get; set; }

        public LabelText(string text, bool isDynamic) {
            this.Text      = text;
            this.IsDynamic = isDynamic;
        }
    }
}