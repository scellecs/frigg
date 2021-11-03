namespace Packages.Frigg.Editor.Utils {
    using global::Frigg.Editor;
    using global::Frigg.Utils;
    using UnityEngine;

    public static class PropertyExtensions {
        public static object GetValue(this FriggProperty property) 
            => property.NativeValue.Get();

        public static void SetValue(this FriggProperty property, object value) {
            property.NativeValue.Set(value);
            
            while (!property.IsRootProperty) {
                var prevProperty = property;
                property = prevProperty.ParentProperty;

                CoreUtilities.SetTargetValue(property, prevProperty.MetaInfo, prevProperty.GetValue());
            }
        }
    }
}