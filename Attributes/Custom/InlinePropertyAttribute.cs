namespace Frigg {
    using System;

    /// <summary>
    /// Force draw property in Inline Mode.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | 
                    AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
    public class InlinePropertyAttribute : CustomAttribute {

        public int LabelWidth { get; set; }

        public InlinePropertyAttribute() {
            this.LabelWidth = 10;
        }
    }
}