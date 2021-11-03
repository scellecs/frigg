namespace Frigg.Editor {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Groups;
    using Unity.Collections;
    using UnityEngine;
    using Utils;

    public abstract class FriggDrawer {
        public          FriggProperty property;

        public          Attribute linkedAttribute;
        public abstract void      DrawLayout();
        public abstract void      Draw(Rect rect);
        
        public abstract float GetHeight();

        
        //Check before draw an element
        public abstract bool      IsVisible { get;}

        public static IEnumerable<FriggDrawer> Resolve(FriggProperty property) {
            //Get attribute drawers
            //var attrs = new List<Attribute>();
            //attrs.AddRange(property.FixedAttributes);

            var result          = new List<FriggDrawer>();
            var hasCustomDrawer = false;
            
            //Add attribute custom decorators
            foreach (var attr in property.FixedAttributes) {
                if(attr.IsDefaultAttribute())
                    continue;
                
                if (attr is BaseDecoratorAttribute || attr is PropertyAttribute){
                    var decoratorDrawer = FriggDecoratorDrawer.Create(attr);
                    decoratorDrawer.property        = property;
                    decoratorDrawer.linkedAttribute = attr;
                    result.Add(decoratorDrawer);
                }

                if (attr is CustomAttribute || attr is BaseAttribute) {
                    var drawer = FriggPropertyDrawerUtils.GetCustomDrawer(property);
                    if (drawer == null) {
                        continue;
                    }

                    drawer.property        = property;
                    drawer.linkedAttribute = attr;
                    if (drawer.GetType() != typeof(ReadonlyPropertyDrawer) && !(drawer is BaseGroupDrawer)) {
                        hasCustomDrawer = true;
                    }
                    result.Add(drawer);
                }
            }
            
            if (hasCustomDrawer) {
                return result;
            }
            
            if (property.MetaInfo.isArray) {
                var drawer = new ReorderableListDrawer(property);
                result.Add(drawer);
                return result;
            }
            
            var builtInDrawer = FriggPropertyDrawerUtils.GetBuiltInDrawer(property);
            if (builtInDrawer != null) {
                builtInDrawer.property = property;
                result.Add(builtInDrawer);
                return result;
            }
            
            var customDrawer = FriggPropertyDrawerUtils.GetCustomDrawer(property);

            if (customDrawer == null) {
                return result;
            }

            customDrawer.property = property;
            result.Add(customDrawer);
            return result;
        }
    }
}