namespace Frigg.Editor {
    using UnityEngine;
    using UnityEditor;
    using Utils;

    public abstract class BaseDrawer : FriggPropertyDrawer {
        protected BaseDrawer(FriggProperty prop) : base(prop) {
            if (prop.TryGetFixedAttribute<HideLabelAttribute>() != null) {
                prop.Label = GUIContent.none;
            }
        }
    }
}