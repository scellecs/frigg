namespace Frigg.Utils {
    using System;
    using System.Collections.Generic;
    using Editor;
    using Frigg;

    public static class DecoratorDrawerUtils {
        /*private static readonly Dictionary<Type, BaseDecoratorDrawer> drawers;

        static DecoratorDrawerUtils() =>
            drawers = new Dictionary<Type, BaseDecoratorDrawer> {
                [typeof(TitleAttribute)] = new TitleDecoratorDrawer(),
                [typeof(InfoBoxAttribute)] = new InfoBoxDecoratorDrawer(),
                [typeof(RequiredAttribute)] = new RequiredDecoratorDrawer(),
                [typeof(PropertySpaceAttribute)] = new PropertySpaceDecoratorDrawer()
            };

        public static BaseDecoratorDrawer GetDecorator(Type decoratorType)
            => drawers.TryGetValue(decoratorType, out var drawer) ? drawer : null;*/
    }
}