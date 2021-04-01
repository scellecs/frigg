namespace Assets.Scripts.Editor.CustomPropertyDrawers {
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Graphs;
    using UnityEditorInternal;
    using UnityEngine;
    using Utils;

    public class ReorderableListDrawer : CustomPropertyDrawer {
        public static ReorderableListDrawer instance = new ReorderableListDrawer();

        private Dictionary<string, ReorderableList> reorderableLists = new Dictionary<string, ReorderableList>();

        public static void ClearData() {
            instance = new ReorderableListDrawer();
        }

        private string GetPropertyKey(SerializedProperty property) => 
            property.serializedObject.targetObject.GetInstanceID() + "." + property.name;

        protected override void CreateAndDraw(Rect rect, SerializedProperty property, GUIContent label) {
            var name = property.propertyPath.Split('.');
            var p    = property.serializedObject.FindProperty(name[0]);

            if(!p.isArray)
                return;
 
            ReorderableList reorderableList;
            
            if (!instance.reorderableLists.ContainsKey(p.name)) {
                reorderableList = new ReorderableList(p.serializedObject, p,
                    true, true, true, true);
                
                SetCallbacks(property, reorderableList);

                instance.reorderableLists[p.name] = reorderableList;
            }
            
            //if is null
            reorderableList = instance.reorderableLists[p.name];
            reorderableList.DoLayoutList();
        }

        private static void SetCallbacks(SerializedProperty property, ReorderableList reorderableList) {
            reorderableList.drawHeaderCallback = tempRect => { 
                EditorGUI.LabelField(tempRect, 
                new GUIContent($"{reorderableList.serializedProperty.displayName} - {reorderableList.count} elements")); 
            };

            reorderableList.drawElementCallback = (tempRect, index, active, focused) => {
                var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                tempRect.y     += 2.0f;
                tempRect.x     += 10.0f;
                tempRect.width -= 10.0f;
                
                EditorGUI.PropertyField(new Rect(tempRect.x, tempRect.y, tempRect.width, 
                    EditorGUIUtility.singleLineHeight), element, true);
            };

            reorderableList.onChangedCallback = list => {
                
            };

            reorderableList.onAddCallback = delegate {
                property.arraySize++;

                var type = CoreUtilities.TryGetListElementType(CoreUtilities.GetPropertyType(property));

                var element = property.GetArrayElementAtIndex(property.arraySize - 1);

                SetDefaultValue(element, type);
            };

            reorderableList.onRemoveCallback = delegate {
                var size = property.arraySize;

                for (var i = reorderableList.index; i < size - 1; i++) {
                    reorderableList.serializedProperty.MoveArrayElement(i + 1, i);
                }

                property.arraySize--;
            };

            reorderableList.elementHeightCallback = index
                => EditorGUI.GetPropertyHeight(reorderableList.serializedProperty.GetArrayElementAtIndex(index)) + 5.0f;
        }
        
        private static void SetDefaultValue(SerializedProperty property, Type type) {
            if (type == typeof(bool)) {
                property.boolValue = default;
            }
            if (type == typeof(int))
            {
                property.intValue = default;
            }
            if (type == typeof(long))
            {
                property.longValue = default;
            }
            if (type == typeof(float))
            {
                property.floatValue = default;
            }
            if (type == typeof(double))
            {
                property.doubleValue = default;
            }
            if (type == typeof(string))
            {
                property.stringValue = default;
            }
            if (type == typeof(Vector2))
            {
                property.vector2Value = default;
            }
            if (type == typeof(Vector3))
            {
                property.vector3Value = default;
            }
            if (type == typeof(Vector4))
            {
                property.vector4Value = default;
            }
            if (type == typeof(Color))
            {
                property.colorValue = default;
            }
            if (type == typeof(Bounds))
            {
                property.boundsValue = default;
            }
            if (type == typeof(Rect))
            {
                property.rectValue = default;
            }
        }
    }
}