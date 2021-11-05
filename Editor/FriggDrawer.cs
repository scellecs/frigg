namespace Frigg.Editor {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Groups;
    using Unity.Collections;
    using UnityEngine;
    using Utils;
    using Frigg.Editor.BuiltIn;

    public abstract class FriggDrawer {
        public          FriggProperty property;

        public          Attribute linkedAttribute;
        public abstract void      DrawLayout();
        public abstract void      Draw(Rect rect);
        
        public abstract float GetHeight();

        
        //Check before draw an element
        public abstract bool IsVisible { get;}

        public static List<FriggDrawerWrapper> Resolve(FriggProperty property) {
            var result          = new List<FriggDrawerWrapper>();
            var hasCustomDrawer = false;

            //Add attribute custom decorators
            foreach (var attr in property.FixedAttributes) {
                if(attr.IsDefaultAttribute())
                    continue;
                
                switch (attr) {
                    case BaseDecoratorAttribute _:
                    case PropertyAttribute _: {
                        var decoratorDrawer = FriggDecoratorDrawer.Create(attr);
                        decoratorDrawer.property        = property;
                        decoratorDrawer.linkedAttribute = attr;
                        result.Add(new FriggDrawerWrapper { DrawerType = FriggDrawerType.Custom, Drawer = decoratorDrawer });
                        break;
                    }
                    case CustomAttribute _:
                    case BaseAttribute _: {
                        var drawer = FriggPropertyDrawerUtils.GetCustomDrawer(property);
                        if (drawer == null) {
                            continue;
                        }

                        drawer.property        = property;
                        drawer.linkedAttribute = attr;
                        if (!(drawer is BaseGroupDrawer) &&  !(drawer is ReadonlyPropertyDrawer)) {
                            hasCustomDrawer = true;
                        }
                        result.Add(new FriggDrawerWrapper { DrawerType = FriggDrawerType.Custom, Drawer = drawer });
                        break;
                    }
                }
            }   
            
            if (hasCustomDrawer) {
                return result;
            }
            
            if (property.MetaInfo.isArray) {
                var drawer = new ReorderableListDrawer(property);
                result.Add(new FriggDrawerWrapper { DrawerType = FriggDrawerType.Custom, Drawer = drawer });
                return result;
            }
            
            var builtInDrawer = FriggPropertyDrawerUtils.GetBuiltInDrawer(property);
            if (builtInDrawer != null) {
                if (property.TryGetFixedAttribute<HideLabelAttribute>() != null) {
                    property.Label = GUIContent.none;
                }

                result.Add(builtInDrawer.Value);
                return result;
            }
            
            var customDrawer = FriggPropertyDrawerUtils.GetCustomDrawer(property);
            if (customDrawer == null) {
                return result;
            }

            customDrawer.property = property;
            result.Add(new FriggDrawerWrapper { DrawerType = FriggDrawerType.Custom, Drawer = customDrawer });
            return result;
        }
    }

    public struct FriggDrawerWrapper
    {
        public FriggDrawerType DrawerType;
        public FriggDrawer Drawer;

        public bool IsVisible { get => DrawerType != FriggDrawerType.Custom || Drawer.IsVisible; }

        public void DrawLayout(FriggProperty property) {
            switch (DrawerType) {
                case FriggDrawerType.Integer:
                    IntegerDrawer.DrawLayout(property);
                    break;
                case FriggDrawerType.String:
                    StringDrawer.DrawLayout(property);
                    break;
                case FriggDrawerType.LabelString:
                    LabelStringDrawer.DrawLayout(property);
                    break;
                case FriggDrawerType.Bool:
                    BoolDrawer.DrawLayout(property);
                    break;
                case FriggDrawerType.Color:
                    ColorDrawer.DrawLayout(property);
                    break;
                case FriggDrawerType.Curve:
                    CurveDrawer.DrawLayout(property);
                    break;
                case FriggDrawerType.Gradient:
                    GradientDrawer.DrawLayout(property);
                    break;
                case FriggDrawerType.Long:
                    LongDrawer.DrawLayout(property);
                    break;
                case FriggDrawerType.Rect:
                    RectDrawer.DrawLayout(property);
                    break;
                case FriggDrawerType.Vector2:
                    Vector2Drawer.DrawLayout(property);
                    break;
                case FriggDrawerType.Vector3:
                    Vector3Drawer.DrawLayout(property);
                    break;
                case FriggDrawerType.Vector4:
                    Vector4Drawer.DrawLayout(property);
                    break;
                case FriggDrawerType.Float:
                    FloatDrawer.DrawLayout(property);
                    break;
                case FriggDrawerType.Single:
                    SingleDrawer.DrawLayout(property);
                    break;
                case FriggDrawerType.Enum:
                    EnumDrawer.DrawLayout(property);
                    break;
                case FriggDrawerType.Object:
                    ObjectDrawer.DrawLayout(property);
                    break;
                case FriggDrawerType.Custom:
                    Drawer.DrawLayout();
                    break;
            }
        }

        public void Draw(FriggProperty property, Rect rect) {
            switch (DrawerType) {
                case FriggDrawerType.Integer:
                    IntegerDrawer.Draw(property, rect);
                    break;
                case FriggDrawerType.String:
                    StringDrawer.Draw(property, rect);
                    break;
                case FriggDrawerType.LabelString:
                    LabelStringDrawer.Draw(property, rect);
                    break;
                case FriggDrawerType.Bool:
                    BoolDrawer.Draw(property, rect);
                    break;
                case FriggDrawerType.Color:
                    ColorDrawer.Draw(property, rect);
                    break;
                case FriggDrawerType.Curve:
                    CurveDrawer.Draw(property, rect);
                    break;
                case FriggDrawerType.Gradient:
                    GradientDrawer.Draw(property, rect);
                    break;
                case FriggDrawerType.Long:
                    LongDrawer.Draw(property, rect);
                    break;
                case FriggDrawerType.Rect:
                    RectDrawer.Draw(property, rect);
                    break;
                case FriggDrawerType.Vector2:
                    Vector2Drawer.Draw(property, rect);
                    break;
                case FriggDrawerType.Vector3:
                    Vector3Drawer.Draw(property, rect);
                    break;
                case FriggDrawerType.Vector4:
                    Vector4Drawer.Draw(property, rect);
                    break;
                case FriggDrawerType.Float:
                    FloatDrawer.Draw(property, rect);
                    break;
                case FriggDrawerType.Single:
                    SingleDrawer.Draw(property, rect);
                    break;
                case FriggDrawerType.Enum:
                    EnumDrawer.Draw(property, rect);
                    break;
                case FriggDrawerType.Object:
                    ObjectDrawer.Draw(property, rect);
                    break;
                case FriggDrawerType.Custom:
                    Drawer.Draw(rect);
                    break;
            }
        }

        public float GetHeight(FriggProperty property) {
            switch (DrawerType) {
                case FriggDrawerType.Integer:
                    return IntegerDrawer.GetHeight();
                case FriggDrawerType.String:
                    return StringDrawer.GetHeight();
                case FriggDrawerType.LabelString:
                    return LabelStringDrawer.GetHeight();
                case FriggDrawerType.Bool:
                    return BoolDrawer.GetHeight();
                case FriggDrawerType.Color:
                    return ColorDrawer.GetHeight();
                case FriggDrawerType.Curve:
                    return CurveDrawer.GetHeight();
                case FriggDrawerType.Gradient:
                    return GradientDrawer.GetHeight();
                case FriggDrawerType.Long:
                    return LongDrawer.GetHeight();
                case FriggDrawerType.Rect:
                    return RectDrawer.GetHeight();
                case FriggDrawerType.Vector2:
                    return Vector2Drawer.GetHeight();
                case FriggDrawerType.Vector3:
                    return Vector3Drawer.GetHeight();
                case FriggDrawerType.Vector4:
                    return Vector4Drawer.GetHeight();
                case FriggDrawerType.Float:
                    return FloatDrawer.GetHeight();
                case FriggDrawerType.Single:
                    return SingleDrawer.GetHeight();
                case FriggDrawerType.Enum:
                    return EnumDrawer.GetHeight();
                case FriggDrawerType.Object:
                    return ObjectDrawer.GetHeight();
                case FriggDrawerType.Custom:
                    return Drawer.GetHeight();
            }

            return 0f;
        }
    }

    public enum FriggDrawerType
    {
        Custom,
        Integer,
        String,
        LabelString,
        Bool,
        Color,
        Curve,
        Gradient,
        Long,
        Rect,
        Vector2,
        Vector3,
        Vector4,
        Float,
        Single,
        Enum,
        Object
    }
}
