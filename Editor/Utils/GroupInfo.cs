namespace Packages.Frigg.Editor.Utils {
    using UnityEngine;

    public class GroupInfo {
        public string Name { get; set; }
        public string Path { get; set; }

        public float GroupHeight { get; set; }

        public Rect baseRect = Rect.zero;
        public Rect currentRect;

        public GroupInfo(string name, string path) {
            this.Name = name;
            this.Path = path;
        }

        public GroupInfo(string path) {
            this.Path = path;
        }
    }
}