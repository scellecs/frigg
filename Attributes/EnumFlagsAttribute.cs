namespace Frigg {
    using System;

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EnumFlagsAttribute : BaseAttribute{
        public string Name { get; private set; }

        public EnumFlagsAttribute(string name) {
            this.Name = name;
        }
        
        public EnumFlagsAttribute() {
        }
    }
}