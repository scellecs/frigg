using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour))]
public class ButtonDrawer : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
    }
}
