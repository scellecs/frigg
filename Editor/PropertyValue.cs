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

        public GetterSetter<T> actual;
        public GetterSetter<T> parent;

        public PropertyValue(object parent, object value, PropertyMeta metaInfo) {
            this.parent = new GetterSetter<T>(() => (T) parent, x => parent = x);
            this.actual = new GetterSetter<T>(() => (T) value, x => value = x);

            this.MetaInfo = metaInfo;
        }
        
        /*private object       actual;
        private object       parent;

        public object ActualValue {
            get => this.actual;
            set => this.actual = value;
        }

        public object Parent {
            get => this.parent;
            set => this.parent = value;
        }*/

        /*public PropertyValue(object parent = null, object value = null) {
            this.parent = parent;
            this.actual = value;
        }*/
    }
}