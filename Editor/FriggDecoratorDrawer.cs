namespace Frigg.Editor {
    using System;
    using UnityEngine;

    //Add a constructor and set it's property & attribute.
    public abstract class FriggDecoratorDrawer : FriggDrawer {
        public static FriggDecoratorDrawer Create(Attribute attr) {
            var type = attr.GetType();

            if (typeof(InfoBoxAttribute).IsAssignableFrom(type)) {
                return new InfoBoxDecoratorDrawer();
            }
            
            if (typeof(PropertySpaceAttribute).IsAssignableFrom(type)) {
                return new PropertySpaceDecoratorDrawer();
            }
            
            if (typeof(TitleAttribute).IsAssignableFrom(type)) {
                return new TitleDecoratorDrawer();
            }
            
            if (typeof(RequiredAttribute).IsAssignableFrom(type)) {
                return new RequiredDecoratorDrawer();
            }
            
            if (typeof(HeaderAttribute).IsAssignableFrom(type)) {
                return new UnityHeaderDecoratorDrawer();
            }
            
            if (typeof(SpaceAttribute).IsAssignableFrom(type)) {
                return new UnitySpaceDecoratorDrawer();
            }

            throw new Exception($"Decorator for attribute with type {type} cannot be found.");
        }
    }
}