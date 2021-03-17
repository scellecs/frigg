namespace Assets.Scripts.Tests {
    using System;
    using Attributes;
    using UnityEngine;

    public class EnumFlagsBehaviour : MonoBehaviour {
        
        [Flags]
        public enum TestFlags {
            None  = 0,
            One   = 1 << 0,
            Two   = 1 << 1,
            Three = 1 << 2,
            Four  = 1 << 3
        }

        [EnumFlags]
        public TestFlags flags;
        
        [EnumFlags("Enum flags with a custom name")]
        public TestFlags flagsTwo;
    }
}