namespace Assets.Scripts.Utils {
    using System;
    using System.Collections.Generic;
    using Attributes.Custom;
    using Editor.DecoratorDrawers;
    using Editor.PropertyDrawers;
    using Packages.Frigg.Attributes;
    using UnityEditor;
    using CustomPropertyDrawer = Editor.CustomPropertyDrawers.CustomPropertyDrawer;

    public static class DecoratorDrawerUtils {
        private static readonly Dictionary<Type, BaseDecoratorDrawer> drawers;

        static DecoratorDrawerUtils() =>
            drawers = new Dictionary<Type, BaseDecoratorDrawer> {
                [typeof(TitleAttribute)] = new TitleDecoratorDrawer(),
                [typeof(InfoBoxAttribute)] = new InfoBoxDecoratorDrawer(),
                [typeof(RequiredAttribute)] = new RequiredDecoratorDrawer(),
                [typeof(PropertySpaceAttribute)] = new PropertySpaceDecoratorDrawer()
            };

        public static BaseDecoratorDrawer GetDecorator(IDecoratorAttribute attribute)
            => drawers.TryGetValue(attribute.GetType(), out var drawer) ? drawer : null;
    }
}