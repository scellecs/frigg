namespace Frigg {
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class InfoBoxAttribute : BaseDecoratorAttribute {
        private const int DEFAULT_FONT = 14;
        
        private InfoMessageType infoMessageType = InfoMessageType.Info;
        
        private int             fontSize        = DEFAULT_FONT;

        public InfoMessageType InfoMessageType {
            get => this.infoMessageType;
            set => this.infoMessageType = value;
        }

        public int FontSize {
            get => this.fontSize;
            set => this.fontSize = value;
        }

        public InfoBoxAttribute(string text) {
            this.Text = text;
        }
    }

    public enum InfoMessageType {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }
}