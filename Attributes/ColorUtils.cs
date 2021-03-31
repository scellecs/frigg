namespace Packages.Frigg {
    using System;
    using UnityEngine;

    public static class ColorUtils {
        public enum FriggColor {
            Default = 0,
            Gray    = 1,
            Black   = 2,
            White   = 3,
            Yellow  = 4,
            Red     = 5,
            Blue    = 6,
            Cyan    = 7,
            Green   = 8
        }

        public static Color ToColor(this FriggColor friggColor) {
            switch (friggColor) {
                case FriggColor.Default:
                    return new Color(0.7f, 0.7f, 0.7f);
                case FriggColor.Gray:
                    return  Color.gray;
                case FriggColor.Black:
                    return  Color.black;
                case FriggColor.White:
                    return  Color.white;
                case FriggColor.Yellow:
                    return  Color.yellow;
                case FriggColor.Red:
                    return  Color.red;
                case FriggColor.Blue:
                    return  Color.blue;
                case FriggColor.Cyan:
                    return  Color.cyan;
                case FriggColor.Green:
                    return  Color.green;
                default:
                    throw new ArgumentOutOfRangeException(nameof(friggColor), friggColor, null);
            }
        }
    }
}