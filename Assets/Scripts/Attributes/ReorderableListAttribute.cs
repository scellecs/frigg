namespace Assets.Scripts.Attributes {
    public class ReorderableListAttribute : BaseAttribute {
        public string Name { get; private set; }

        public ReorderableListAttribute(string name) {
            Name = name;
        }

        public ReorderableListAttribute() {
        }
    }
}