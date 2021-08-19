namespace Packages.Frigg.Editor.Utils {
    using global::Frigg.Editor;
    using global::Frigg.Utils;
    using UnityEngine;

    public static class PropertyExtensions {
        public static object GetValue(this FriggProperty property) 
            => CoreUtilities.GetTargetValue(property.ParentValue, property.MetaInfo.MemberInfo);

        public static void SetValue(this FriggProperty property, object value) {
            CoreUtilities.SetTargetValue(property, property.ParentValue, property.MetaInfo.MemberInfo, value);
        }
    }
}