﻿namespace Frigg.Tests {
    using Frigg;
    using UnityEngine;

    public class TitleBehaviour : MonoBehaviour {

        [Title("Private fields")]
        [ShowInInspector]
        private int private1;
        
        [ShowInInspector]
        private int private2;
        
        [ShowInInspector]
        private int private3;
        
        [Title("Test without any arguments")]
        public int    test1;
        public string str1;
        public float  float1;
        
        [Title("Test without bold line", bold = false)]
        public int test2;
        public string str2;
        public float  float2;
        
        [Title("Test without horizontal line", drawLine = false)]
        public int test3;
        public string str3;
        public float  float3;
        
        [Title("Test with center alignment", titleAlighment = TitleAlignment.Centered)]
        public int test4;
        public string str4;
        public float  float4;
        
        [Title("Test with right-based alignment", titleAlighment = TitleAlignment.Right)]
        public int test5;
        public string str5;
        public float  float5;
        
        [Title("Test some custom colors and font size", fontSize = 16, lineColor = ColorUtils.FriggColor.Cyan, textColor = ColorUtils.FriggColor.Yellow)]
        public int test6;
        public string str6;
        public float  float6;

        [Title("Property title")]
        [ShowInInspector]
        public int prop { get; set; }

        [Title("Some buttons", fontSize = 16)]
        [Button("Test button")]
        public void ClickMe() {
        }
        
        [Button("Test button")]
        public void ClickMeTwo() {
        }
    }
}