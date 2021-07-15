namespace Packages.Frigg.Editor.Utils {
    using UnityEditor;

    internal class StoredBoolValue
    {
        private bool   _value;
        private string _name;

        public bool Value
        {
            get => _value;
            set
            {
                if (_value == value)
                {
                    return;
                }

                _value = value;
                EditorPrefs.SetBool(_name, value);
            }
        }

        public StoredBoolValue(string name, bool value)
        {
            _name  = name;
            _value = EditorPrefs.GetBool(name, value);
        }
    }
}