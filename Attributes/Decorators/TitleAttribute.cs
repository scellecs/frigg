﻿namespace Frigg {
    using System;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, 
        AllowMultiple = true)]
    public class TitleAttribute : BaseDecoratorAttribute {

        public TitleAlignment titleAlighment = TitleAlignment.Left;

        public bool drawLine = true;
        public bool bold     = true;

        public int lineHeight = 1;
        public int fontSize   = 12;

        public ColorUtils.FriggColor textColor = ColorUtils.FriggColor.Default;
        public ColorUtils.FriggColor lineColor = ColorUtils.FriggColor.Gray;

        public TitleAttribute(string title) {
            this.Text   = title;
#if UNITY_EDITOR
            this.Height = this.lineHeight + EditorGUIUtility.singleLineHeight;
#endif
        }
    }

    public enum TitleAlignment {
        Left     = 0,
        Right    = 1,
        Centered = 2
    }
}