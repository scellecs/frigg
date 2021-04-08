namespace Frigg {
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ButtonAttribute : BaseAttribute {
        public string Text { get; private set; }

        public ButtonAttribute(string text = "Button") {
            this.Text = text;
        }
    }
}
