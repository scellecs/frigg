using UnityEngine;

namespace Assets.Scripts.Attributes {
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ButtonAttribute : PropertyAttribute, IAttribute 
    {
        public string Text { get; private set; }

        public ButtonAttribute(string text = "Button") {
            Text = text;
        }
    }
}
