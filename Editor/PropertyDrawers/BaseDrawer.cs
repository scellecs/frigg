namespace Frigg.Editor {
    using System;
    using UnityEngine;

    public abstract class BaseDrawer : FriggPropertyDrawer {
        private Type drawerType;
        protected BaseDrawer(FriggProperty prop) : base(prop) {
            if (prop.TryGetFixedAttribute<HideLabelAttribute>() != null) {
                prop.Label = GUIContent.none;
            }
        }

        protected void UpdateAndCallNext(object value, Rect rect = default) =>
            DrawerUtils.UpdateAndCallNext(this.drawerType, this.property, value, rect);

        protected void CallNext(Rect rect = default) => DrawerUtils.CallNext(this.property, rect);
    }
}