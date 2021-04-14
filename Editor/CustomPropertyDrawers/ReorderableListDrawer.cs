namespace Frigg.Editor {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEditor;
    using UnityEditor.Graphs;
    using UnityEditorInternal;
    using UnityEngine;
    using Utils;

    public class ReorderableListDrawer : CustomPropertyDrawer {
        public static ReorderableListDrawer instance = new ReorderableListDrawer();

        private readonly Dictionary<string, ReorderableList> reorderableLists   = new Dictionary<string, ReorderableList>();
        private readonly Dictionary<string, bool>            reorderableLayouts = new Dictionary<string, bool>();

        public static void ClearData() {
            instance = new ReorderableListDrawer();
        }

        private string GetPropertyKey(SerializedProperty property) => 
            property.serializedObject.targetObject.GetInstanceID() + "." + property.name;

        protected override void CreateAndDraw(object target, Rect rect, MemberInfo memberInfo, GUIContent label) {
            ReorderableList             list;
            object                      value = null;
            ListDrawerSettingsAttribute attr  = null;
            Type                        type  = null;

            if (!instance.reorderableLists.ContainsKey(memberInfo.Name)) {

                if (memberInfo.GetType().IsSubclassOf(typeof(FieldInfo))) {
                    var fieldInfo = (FieldInfo) memberInfo;
                    type  = typeof(FieldInfo);
                    attr  = fieldInfo.GetCustomAttribute<ListDrawerSettingsAttribute>();
                    value = fieldInfo.GetValue(target);
                }

                if (memberInfo.GetType().IsSubclassOf(typeof(PropertyInfo))) {
                    var propertyInfo = (PropertyInfo) memberInfo;
                    type  = typeof(PropertyInfo);
                    attr  = propertyInfo.GetCustomAttribute<ListDrawerSettingsAttribute>();
                    value = propertyInfo.GetValue(target);
                }

                if (attr != null) {
                    var hideHeader = attr.HideHeader;
                    list = new ReorderableList((IList) value, value.GetType(), attr.AllowDrag, !attr.HideHeader,
                        !attr.HideAddButton, !attr.HideRemoveButton);
                    SetCallbacks(type, target, memberInfo, list, hideHeader);
                    list.DoLayoutList();
                    return;
                }

                list = new ReorderableList((IList) value, value?.GetType(), true, true, true, true);

                SetCallbacks(type, target, memberInfo, list);

                instance.reorderableLists[memberInfo.Name] = list;
            }

            list = instance.reorderableLists[memberInfo.Name];
            list.DoLayoutList();
        }
        
        protected override void CreateAndDraw(Rect rect, SerializedProperty property, GUIContent label) {
            var name = property.propertyPath.Split('.');
            var p    = property.serializedObject.FindProperty(name[0]);

            if(!p.isArray)
                return;
 
            ReorderableList reorderableList;

            if (!instance.reorderableLists.ContainsKey(p.name)) {
                var attr = CoreUtilities.TryGetAttribute<ListDrawerSettingsAttribute>(property);
                
                if (attr != null) {
                    reorderableList = new ReorderableList(p.serializedObject, p,
                        attr.AllowDrag, !attr.HideHeader, !attr.HideAddButton, !attr.HideRemoveButton);
                    SetCallbacks(property, reorderableList, attr.HideHeader);
                }
                else {
                    reorderableList = new ReorderableList(p.serializedObject, p,
                        true, true, true, true);
                    SetCallbacks(property, reorderableList);
                }

                instance.reorderableLists[p.name] = reorderableList;
            }
            
            //if is null
            reorderableList = instance.reorderableLists[p.name];
            reorderableList.DoLayoutList();
        }

        private static void SetCallbacks(Type type, object target, MemberInfo memberInfo, ReorderableList reorderableList, bool hideHeader = false) {
            if (!hideHeader) {
                reorderableList.drawHeaderCallback = tempRect => {
                    EditorGUI.LabelField(tempRect,
                        new GUIContent($"{ObjectNames.NicifyVariableName(memberInfo.Name)} - {reorderableList.count} elements"));
                };
            }
            
            reorderableList.drawElementCallback = (tempRect, index, active, focused) => {
                var element = reorderableList.list[index];
                tempRect.y     += 2.0f;
                tempRect.x     += 10.0f;
                tempRect.width -= 10.0f;

                if (type == typeof(PropertyInfo)) {
                    var property = (PropertyInfo) memberInfo;
                    if (!property.CanWrite) {
                        using(new EditorGUI.DisabledScope(true)) {
                            reorderableList.draggable  = false;
                            reorderableList.displayAdd = reorderableList.displayRemove = false;
                            GuiUtilities.Field(element, tempRect, new GUIContent($"Element {index}"));
                        }
                        return;
                    }
                }

                if (memberInfo.GetCustomAttribute<HideLabelAttribute>() != null) {
                    reorderableList.list[index] = GuiUtilities.Field(element, tempRect,
                        GUIContent.none);
                    return;
                }
                
                reorderableList.list[index] = GuiUtilities.Field(element, tempRect,
                    new GUIContent($"Element {index}"));
            };
            
            reorderableList.elementHeightCallback = index
                => EditorGUIUtility.singleLineHeight;
        }
        
        private static void SetCallbacks(SerializedProperty property, ReorderableList reorderableList, bool hideHeader = false) {

            if (!hideHeader) {
                reorderableList.drawHeaderCallback = tempRect => {
                    EditorGUI.LabelField(tempRect,
                        new GUIContent($"{reorderableList.serializedProperty.displayName} - {reorderableList.count} elements"));
                };
            }

            reorderableList.drawElementCallback = (tempRect, index, active, focused) => {
                var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index); //Element 0, Element 1, etc...
                tempRect.y     += 2.0f;
                tempRect.x     += 10.0f;
                tempRect.width -= 10.0f;

                var copy = element.Copy();

                var enumerator = element.GetEnumerator();
                enumerator.MoveNext();

                if (element.propertyType == SerializedPropertyType.Generic) {
                    
                    var type   = CoreUtilities.TryGetListElementType(CoreUtilities.GetPropertyType(element));
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    var count = fields.Length;
                    if (count == 1) {
                        //var newElement = (SerializedProperty) enumerator.Current;

                        EditorGUI.PropertyField(new Rect(tempRect.x, tempRect.y, tempRect.width,
                            EditorGUIUtility.singleLineHeight), element, CoreUtilities.GetGUIContent(element), true);

                        return;
                    }

                    if (!instance.reorderableLayouts.ContainsKey(copy.propertyPath))
                        instance.reorderableLayouts.Add(copy.propertyPath, false);

                    var status = instance.reorderableLayouts[copy.propertyPath];
                    instance.reorderableLayouts[copy.propertyPath] = EditorGUI.Foldout(new Rect(tempRect.x, tempRect.y, tempRect.width,
                        EditorGUIUtility.singleLineHeight), status, copy.displayName);

                    if (!instance.reorderableLayouts[copy.propertyPath]) {
                        return;
                    }

                    enumerator = copy.GetEnumerator();

                    var currentElement = 0;
                    while (enumerator.MoveNext()) {
                        var current = (SerializedProperty) enumerator.Current;

                        tempRect.x += currentElement * EditorGUIUtility.singleLineHeight;
                        tempRect.y += currentElement * EditorGUIUtility.singleLineHeight;

                        reorderableList.elementHeightCallback = _
                            => EditorGUIUtility.singleLineHeight + 5.0f;

                        var content = CoreUtilities.GetGUIContent(current);
                        EditorGUI.PropertyField(new Rect(tempRect.x, tempRect.y + EditorGUIUtility.singleLineHeight, tempRect.width,
                            EditorGUIUtility.singleLineHeight), current, content, true);

                        currentElement++;
                    }

                    return;
                }
                
                EditorGUI.PropertyField(new Rect(tempRect.x, tempRect.y, tempRect.width,
                        EditorGUIUtility.singleLineHeight), element, CoreUtilities.GetGUIContent(element), true);
            };
            
            reorderableList.onAddCallback = delegate {
                property.arraySize++;

                var type = CoreUtilities.TryGetListElementType(CoreUtilities.GetPropertyType(property));

                var element = property.GetArrayElementAtIndex(property.arraySize - 1);

                CoreUtilities.SetDefaultValue(element, type);
            };

            reorderableList.onRemoveCallback = delegate {
                var size = property.arraySize;

                for (var i = reorderableList.index; i < size - 1; i++) {
                    reorderableList.serializedProperty.MoveArrayElement(i + 1, i);
                }

                property.arraySize--;
            };

            reorderableList.elementHeightCallback = index
                => EditorGUIUtility.singleLineHeight + 5.0f;
        }
    }
}