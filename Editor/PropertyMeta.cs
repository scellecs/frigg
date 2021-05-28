namespace Frigg.Editor {
    using System;
    using System.Reflection;

    public class PropertyMeta {
        public Type   MemberType { get; set; }

        public MemberInfo MemberInfo { get; set; }

        public int    Order { get; set; }
        
        public string Name  { get; set; }
        
        public bool isArray;
        public int  arraySize  = 0;
        public int  arrayIndex = -1;
    }
}