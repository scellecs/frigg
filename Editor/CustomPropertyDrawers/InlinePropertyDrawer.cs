namespace Frigg.Editor {
    using UnityEditor;
    using UnityEngine;

    public class InlinePropertyDrawer : CustomPropertyDrawer {
        public static readonly InlinePropertyDrawer instance = new InlinePropertyDrawer();

        public int labelWidth;
        
        protected override void CreateAndDraw(Rect rect, SerializedProperty property, GUIContent label) {
            var cachedWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = this.labelWidth;
            
            EditorGUILayout.BeginHorizontal();
            if(label != GUIContent.none)
               EditorGUILayout.LabelField(label);

            var copy       = property.Copy(); //we need to work with a property copy
            var enumerator = copy.GetEnumerator();
            
            while (enumerator.MoveNext()) {
                var prop = (SerializedProperty) enumerator.Current;
                EditorGUILayout.PropertyField(prop);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = cachedWidth;
        }
    }
}