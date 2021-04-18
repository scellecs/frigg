namespace Frigg {
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ListDrawerSettingsAttribute : Attribute {
        public bool AllowDrag   { get; set; }
        public bool HideAddButton    { get; set; }
        public bool HideRemoveButton { get; set; }
        public bool HideHeader       { get; set; }
    }
}