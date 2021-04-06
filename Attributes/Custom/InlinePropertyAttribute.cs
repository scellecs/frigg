using System;

namespace Assets.Scripts.Attributes.Custom {
    using Custom;
    using Frigg;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | 
                    AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
    public class InlinePropertyAttribute : CustomAttribute {

        public int LabelWitdh { get; set; }

        public InlinePropertyAttribute() {
            LabelWitdh = 10;
        }
    }
}