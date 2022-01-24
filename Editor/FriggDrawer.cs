namespace Frigg.Editor {
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Utils;
    using BuiltIn;

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

            SetDecorators(property, result);
            SetMainDrawer(property, result);
            return result;
        }

        private static void SetDecorators(FriggProperty property, List<FriggDrawerWrapper> list) {
            foreach (var attr in property.FixedAttributes) {
                if (!(attr is BaseDecoratorAttribute)) {
                    continue;
                }

                var drawer = FriggDecoratorDrawer.Create(attr);
                
                //todo: move into constructor
                drawer.linkedAttribute = attr;
                drawer.property        = property;
                list.Add(new FriggDrawerWrapper {
                    Drawer     = drawer,
                    DrawerType = FriggDrawerType.Custom
                });
            }

            if (!property.IsReadonly || list.Exists(x
                => x.Drawer.GetType() == typeof(ReadonlyDecoratorDrawer))) {
                return;
            }
            
            var readonlyDecoratorDrawer = new ReadonlyDecoratorDrawer {
                    property = property
            };

            //move into constructor
            list.Add(new FriggDrawerWrapper { 
                Drawer = readonlyDecoratorDrawer,
                DrawerType  = FriggDrawerType.Custom
            });
            
        }

        private static void SetMainDrawer(FriggProperty property, List<FriggDrawerWrapper> list) {
            var builtIn = FriggPropertyDrawerUtils.GetBuiltInDrawer(property);
            if (builtIn == null) {
                var custom = FriggPropertyDrawerUtils.GetCustomDrawer(property);
                list.Add(new FriggDrawerWrapper {
                    Drawer = custom,
                    DrawerType = FriggDrawerType.Custom
                });

                return;
            }
            
            list.Add(builtIn.Value);
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
                    DoubleDrawer.DrawLayout(property);
                    break;
                case FriggDrawerType.Enum:
                    EnumDrawer.DrawLayout(property);
                    break;
                case FriggDrawerType.Object:
                    ObjectDrawer.DrawLayout(property);
                    break;
                case FriggDrawerType.Custom:
                    //Debug.Log($"{this.Drawer.GetType()} -> {this.Drawer.property.Name}");
                    this.Drawer.DrawLayout();
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
                    DoubleDrawer.Draw(property, rect);
                    break;
                case FriggDrawerType.Enum:
                    EnumDrawer.Draw(property, rect);
                    break;
                case FriggDrawerType.Object:
                    ObjectDrawer.Draw(property, rect);
                    break;
                case FriggDrawerType.Custom:
                    this.Drawer.Draw(rect);
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
                    return DoubleDrawer.GetHeight();
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
