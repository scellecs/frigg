namespace Frigg.Editor {
    using System;
    using System.ComponentModel;
    using JetBrains.Annotations;
    using UnityEngine;

    public sealed class GetterSetter<T> 
    {
        private Func<T>   getter;
        private Action<T> setter;
        public GetterSetter(Func<T> getter, Action<T> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }
        public T Value
        {
            get => getter();
            set {
                this.setter(value);
            }
        }
    }
    
    public class PropertyValue<T> {
        public  PropertyMeta MetaInfo { get; set; }

        private readonly GetterSetter<T> actual;

        public PropertyValue(object parent, object value, PropertyMeta metaInfo) {
            this.actual = new GetterSetter<T>(() => (T) value, x => value = x);

            this.MetaInfo = metaInfo;
        }

        public T Get()        => this.actual.Value;
        public void   Set(T value) => this.actual.Value = value;
    }
}