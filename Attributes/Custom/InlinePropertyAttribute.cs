namespace Frigg {
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | 
                    AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
    public class InlinePropertyAttribute : CustomAttribute {

        public int LabelWidth { get; set; }

        public InlinePropertyAttribute() {
            this.LabelWidth = 10;
        }
    }
}