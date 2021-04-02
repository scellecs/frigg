namespace Packages.Frigg.Attributes {
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class PropertySpaceAttribute : BaseDecoratorAttribute {
        private int spaceBefore = 0;
        private int spaceAfter  = 0;

        public int SpaceBefore {
            get => this.spaceBefore;
            set => this.spaceBefore = value;
        }

        public int SpaceAfter {
            get => this.spaceAfter;
            set => this.spaceAfter = value;
        }
    }
}