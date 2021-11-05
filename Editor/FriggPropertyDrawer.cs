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
        protected FriggPropertyDrawer(FriggProperty prop) => this.property = prop;
    }

    public static class FriggPropertyDrawerUtils {
        public static FriggPropertyDrawer GetCustomDrawer(FriggProperty property) {
            var meta = property.MetaInfo.MemberInfo;

            if (!CoreUtilities.IsWritable(property.MetaInfo.MemberInfo)) {
                return new ReadonlyPropertyDrawer(property);
            }

            if (meta.IsDefined(typeof(ButtonAttribute))) {
                return new ButtonDrawer(property);
            }
            
            if (meta.IsDefined(typeof(InlinePropertyAttribute)) 
                || property.MetaInfo.isArray
                || property.MetaInfo.MemberType.IsDefined(typeof(InlinePropertyAttribute))){
                return new InlinePropertyDrawer(property);
            }

            if (meta.IsDefined(typeof(DropdownAttribute))) {
                return new DropdownDrawer(property);
            }
            
            if (meta.IsDefined(typeof(EnumFlagsAttribute))) {
                return new EnumFlagsDrawer(property);
            }
            
            if (!CoreUtilities.IsBuiltIn(property.MetaInfo.MemberType) || meta.IsDefined(typeof(SerializableAttribute))) {
                return new FoldoutPropertyDrawer(property);
            }

            return null;
        }

        public static FriggDrawerWrapper? GetBuiltInDrawer(FriggProperty prop)
        {
            var valueType = prop.MetaInfo.MemberType;

            if (valueType == typeof(int)) {
                return new FriggDrawerWrapper { DrawerType = FriggDrawerType.Integer, Drawer = null };
            }

            if (valueType == typeof(string)) {
                return new FriggDrawerWrapper { DrawerType = prop.TryGetFixedAttribute<DisplayAsString>() != null ? FriggDrawerType.LabelString : FriggDrawerType.String, Drawer = null };
            }

            if (valueType == typeof(bool)) {
                return new FriggDrawerWrapper { DrawerType = FriggDrawerType.Bool, Drawer = null };
            }

            if (valueType == typeof(Color)) {
                return new FriggDrawerWrapper { DrawerType = FriggDrawerType.Color, Drawer = null };
            }

            if (valueType == typeof(AnimationCurve)) {
                return new FriggDrawerWrapper { DrawerType = FriggDrawerType.Curve, Drawer = null };
            }

            if (valueType == typeof(Gradient)) {
                return new FriggDrawerWrapper { DrawerType = FriggDrawerType.Gradient, Drawer = null };
            }

            if (valueType == typeof(long)) {
                return new FriggDrawerWrapper { DrawerType = FriggDrawerType.Long, Drawer = null };
            }

            if (valueType == typeof(Rect)) {
                return new FriggDrawerWrapper { DrawerType = FriggDrawerType.Rect, Drawer = null };
            }

            if (valueType == typeof(Vector2)) {
                return new FriggDrawerWrapper { DrawerType = FriggDrawerType.Vector2, Drawer = null };
            }

            if (valueType == typeof(Vector3)) {
                return new FriggDrawerWrapper { DrawerType = FriggDrawerType.Vector3, Drawer = null };
            }

            if (valueType == typeof(Vector4)) {
                return new FriggDrawerWrapper { DrawerType = FriggDrawerType.Vector4, Drawer = null };
            }

            if (valueType == typeof(float)) {
                return new FriggDrawerWrapper { DrawerType = FriggDrawerType.Float, Drawer = null };
            }

            if (valueType == typeof(double)) {
                return new FriggDrawerWrapper { DrawerType = FriggDrawerType.Single, Drawer = null };
            }

            if (typeof(Enum).IsAssignableFrom(valueType)) {
                return new FriggDrawerWrapper { DrawerType = FriggDrawerType.Enum, Drawer = null };
            }

            if (typeof(UnityEngine.Object).IsAssignableFrom(valueType)) {
                return new FriggDrawerWrapper { DrawerType = FriggDrawerType.Object, Drawer = null };
            }

            return null;
        }
    }
}