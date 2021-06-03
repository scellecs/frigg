namespace Frigg.Editor {
    using System.ComponentModel;
    using JetBrains.Annotations;
    using UnityEngine;

    public class PropertyValue : INotifyPropertyChanged {

        public  PropertyMeta MetaInfo { get; set; }
        private object       value;

        public object Value {
            get => this.value;
            set {
                this.value = value;
                this.OnPropertyChanged(nameof(this.Value));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public PropertyValue(object value) {
            this.value           =  value;
            /*this.PropertyChanged += ((sender, args) => {
                args.
            });*/
        }
    }
}