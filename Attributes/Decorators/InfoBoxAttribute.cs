namespace Packages.Frigg.Attributes {
    using System;
    using UnityEditor;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class InfoBoxAttribute : BaseDecoratorAttribute {
        private const int DEFAULT_HEIGHT = 20;
        private const int DEFAULT_FONT = 14;
        
        private InfoMessageType infoMessageType = InfoMessageType.Info;

        private int height   = DEFAULT_HEIGHT;
        private int fontSize = DEFAULT_FONT;
        
        public InfoMessageType InfoMessageType {
            get => this.infoMessageType;
            set => this.infoMessageType = value;
        }

        public int Height {
            get => this.height;
            set => this.height = value;
        }

        public int FontSize {
            get => this.fontSize;
            set => this.fontSize = value;
        }

        public string Text { get; private set; }

        public InfoBoxAttribute(string text) {
            Text = text;
        }

        public bool HasCustomHeight => this.height != DEFAULT_HEIGHT;
    }

    public enum InfoMessageType {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }
}