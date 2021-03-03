namespace Assets.Scripts.Utils {
    using System.Collections;
    using System.Collections.Generic;

    public sealed class DropdownList<T> : IDropdownList{
        private readonly Dictionary<string, object> data = new Dictionary<string, object>();

        public void Add(string name, T element) {
            this.data.Add(name, element);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() 
            => this.data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    public interface IDropdownList : IEnumerable<KeyValuePair<string, object>>{
        
    }
}