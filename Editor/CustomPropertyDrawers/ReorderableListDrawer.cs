namespace Frigg.Editor {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEditor;
    using UnityEditor.Graphs;
    using UnityEditorInternal;
    using UnityEngine;
    using System.Linq;
    using Utils;
    using Random = UnityEngine.Random;

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
                var level   = EditorGUI.indentLevel;
                
                var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index); //Element 0, Element 1, etc...
                tempRect.y     += 2.0f;
                tempRect.x     += 10.0f;
                tempRect.width -= 10.0f;
                
                var copy = element.Copy();

                //var target = CoreUtilities.GetTargetObjectOfProperty(copy) ?? CoreUtilities.GetTargetObjectWithProperty(copy);

                //var indentLevel = EditorGUI.indentLevel;
                //var num         = indentLevel - copy.depth;

                /*var count = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(x => x.IsUnitySerialized());*/
                
                //copy = element.Copy();

                //EditorGUI.indentLevel = copy.depth + num;
                tempRect.height       = EditorGUI.GetPropertyHeight(element);

                if (copy.propertyType != SerializedPropertyType.Generic) {
                    EditorGUI.PropertyField(new Rect(tempRect.x, tempRect.y, tempRect.width,
                        EditorGUIUtility.singleLineHeight), element, true);

                    return;
                }

                //var cachedPath = copy.propertyPath;
                
                /*if (count.Count() == 1) {
                    copy.NextVisible(true);
                    EditorGUI.PropertyField(new Rect(tempRect.x, tempRect.y, tempRect.width,
                        EditorGUIUtility.singleLineHeight), copy, CoreUtilities.GetGUIContent(copy), false);
                        
                    EditorGUI.indentLevel = level;
                    return;
                }*/
                
                var endProperty = copy.GetEndProperty();
                
                /*if (!instance.reorderableLayouts.ContainsKey(cachedPath)) {
                    instance.reorderableLayouts.Add(cachedPath, false);
                }

                var status = instance.reorderableLayouts[cachedPath];

                instance.reorderableLayouts[cachedPath] = EditorGUI.Foldout(new Rect(tempRect.x, tempRect.y, tempRect.width,
                    EditorGUIUtility.singleLineHeight), status, $"Element {index}");
                
                if(!instance.reorderableLayouts[cachedPath])
                    return;*/

                copy.NextVisible(true);

                do {
                    var content          = CoreUtilities.GetGUIContent(copy);
                    var decoratorsHeight = GuiUtilities.DecoratorsHeight(copy);
                    
                    if (decoratorsHeight > 0) {
                        var amount = GuiUtilities.AmountOfDecorators(copy);
                        GuiUtilities.HandleDecorators(copy, tempRect, true);
                        tempRect.y += decoratorsHeight + GuiUtilities.SPACE * amount;
                    }

                    EditorGUI.PropertyField(new Rect(tempRect.x, tempRect.y, 
                        tempRect.width, EditorGUIUtility.singleLineHeight), copy, content, true);
                    
                    EditorGUI.indentLevel =  level;
                    tempRect.y            += EditorGUIUtility.singleLineHeight;
                } while (copy.NextVisible(true) && !SerializedProperty.EqualContents(copy, endProperty));
            };
            
            reorderableList.onAddCallback = delegate {
                property.arraySize++;

                var type = CoreUtilities.TryGetListElementType(CoreUtilities.GetPropertyType(property));

                var element = property.GetArrayElementAtIndex(property.arraySize - 1);
                
                CoreUtilities.SetDefaultValue(property, element);
            };

            reorderableList.onRemoveCallback = delegate {
                var size = property.arraySize;

                for (var i = reorderableList.index; i < size - 1; i++) {
                    reorderableList.serializedProperty.MoveArrayElement(i + 1, i);
                }

                property.arraySize--;
            };

            reorderableList.elementHeightCallback = index
                => {
                var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                var path    = element.propertyPath;
                
                /*if (!instance.reorderableLayouts.ContainsKey(path)) {
                    return EditorGUIUtility.singleLineHeight + 5.0f;
                }

                if(!instance.reorderableLayouts[path])
                    return EditorGUIUtility.singleLineHeight + 5.0f;*/

                var copy = element.Copy();
                var last = copy.GetEndProperty();

                float decoratorHeight = 0;
                var   height          = 0f;
                var   amount          = 0;

                //allocate total height.
                do { 
                    decoratorHeight += GuiUtilities.DecoratorsHeight(copy);
                    height          += EditorGUIUtility.singleLineHeight;
                    
                    var currAmount = GuiUtilities.AmountOfDecorators(copy);
                    if(currAmount <= 0)
                        continue;

                    amount          += currAmount;
                } while (copy.NextVisible(true) && !SerializedProperty.EqualContents(copy, last));
                
                //Because it's last element.
                if(element.propertyType == SerializedPropertyType.Generic)
                   height -= EditorGUIUtility.singleLineHeight;
                
                return height + decoratorHeight + GuiUtilities.SPACE * amount + 8.0f;
            };
        }
    }
}