namespace Frigg.Editor {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using BuiltIn;
    using Groups;
    using UnityEngine;
    using Utils;
    using Object = System.Object;

    public abstract class FriggPropertyDrawer : FriggDrawer {
        
        public FriggPropertyDrawer(FriggProperty prop) {
            this.property       = prop;
        }
    }

    public static class FriggPropertyDrawerUtils {
        public static FriggPropertyDrawer GetCustomDrawer(FriggProperty property) {
            var meta = property.MetaInfo.MemberInfo;

            if (meta.GetCustomAttribute<ReadonlyAttribute>() != null) {
                return new ReadonlyPropertyDrawer(property);
            }

            if (meta.GetCustomAttribute<HorizontalGroupAttribute>() != null) {
                return new HorizontalGroupDrawer(property);
            }
            
            if (meta.GetCustomAttribute<VerticalGroupAttribute>() != null) {
                return new VerticalGroupDrawer(property);
            }
            
            if (meta.GetCustomAttribute<InlinePropertyAttribute>() != null 
                || property.MetaInfo.isArray
                || property.MetaInfo.MemberType.IsDefined(typeof(InlinePropertyAttribute))
                || property.TryGetFixedAttribute<BaseGroupAttribute>() != null
                || property.HasGroupsInChildren()){
                return new InlinePropertyDrawer(property);
            }

            if (meta.GetCustomAttribute<DropdownAttribute>() != null) {
                return new DropdownDrawer(property);
            }
            
            if (meta.GetCustomAttribute<EnumFlagsAttribute>() != null) {
                return new EnumFlagsDrawer(property);
            }
            
            if (!CoreUtilities.IsBuiltIn(property.MetaInfo.MemberType) || meta.GetCustomAttribute<SerializableAttribute>() != null) {
                return new FoldoutPropertyDrawer(property);
            }

            return null;
        }

        public static BuiltInDrawer GetBuiltInDrawer(FriggProperty prop) {
            var valueType = prop.MetaInfo.MemberType;
            
            if (valueType == typeof(int)) {
                return new IntegerDrawer(prop);
            }

            if (valueType == typeof(string)) {
                return new StringDrawer(prop);
            }
            
            if (valueType == typeof(bool)) {
                return new BoolDrawer(prop);
            }
            
            if (valueType == typeof(Color)) {
                return new ColorDrawer(prop);
            }
            
            if (valueType == typeof(AnimationCurve)) {
                return new CurveDrawer(prop);
            }
            
            if (valueType == typeof(Gradient)) {
                return new GradientDrawer(prop);
            }
            
            if (valueType == typeof(long)) {
                return new LongDrawer(prop);
            }
            
            if (valueType == typeof(Rect)) {
                return new RectDrawer(prop);
            }
            
            if (valueType== typeof(Vector2)) {
                return new Vector2Drawer(prop);
            }
            
            if (valueType == typeof(Vector3)) {
                return new Vector3Drawer(prop);
            }
            
            if (valueType == typeof(Vector4)) {
                return new Vector4Drawer(prop);
            }

            if (valueType == typeof(float)) {
                return new FloatDrawer(prop);
            }

            if (valueType == typeof(double)) {
                return new SingleDrawer(prop);
            }

            if (typeof(Enum).IsAssignableFrom(valueType)) {
                return new EnumDrawer(prop);
            }

            if (typeof(UnityEngine.Object).IsAssignableFrom(valueType)) {
                return new ObjectDrawer(prop);
            }

            return null;
        }
    }
}