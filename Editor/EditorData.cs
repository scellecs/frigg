namespace Frigg.Editor {
    using UnityEditor;

    public class EditorData {

        public static void Erase() {
            EditorPrefs.DeleteAll();
        }
        
        public static void SetBoolValue(string path, bool value) {
            EditorPrefs.SetBool(path, value);
        }

        public static bool GetBoolValue(string path) {
            return EditorPrefs.HasKey(path) && EditorPrefs.GetBool(path);
        }
    }
}