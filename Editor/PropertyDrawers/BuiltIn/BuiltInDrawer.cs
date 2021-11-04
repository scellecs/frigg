namespace Frigg.Editor.BuiltIn {
    using System;
    using System.Collections;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public abstract class BuiltInDrawer : BaseDrawer {
        protected BuiltInDrawer(FriggProperty prop) : base(prop) {
        }
    }
}