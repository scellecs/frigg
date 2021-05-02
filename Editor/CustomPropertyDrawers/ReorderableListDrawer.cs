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

        private readonly Dictionary<string, ReorderableList> reorderableLists = new Dictionary<string, ReorderableList>();
        //private readonly Dictionary<string, bool>            reorderableLayouts = new Dictionary<string, bool>();

        public static void ClearData() {
            instance = new ReorderableListDrawer();
        }

        private string GetPropertyKey(SerializedProperty property) => 
            property.serializedObject.targetObject.GetInstanceID() + "." + property.name;

        protected override object CreateAndDraw(Rect rect, MemberInfo memberInfo, object target, GUIContent label) {
            ReorderableList             list;
            object                      value = null;
            ListDrawerSettingsAttribute attr  = null;
            Type                        type  = null;

            //TODO:Refactor
            if (!instance.reorderableLists.ContainsKey(memberInfo.Name)) {

                switch (memberInfo.MemberType) {
                    case MemberTypes.Field: {
                        var fieldInfo = (FieldInfo) memberInfo;
                        type  = typeof(FieldInfo);
                        attr  = fieldInfo.GetCustomAttribute<ListDrawerSettingsAttribute>();
                        value = fieldInfo.GetValue(target) ?? Array.CreateInstance(CoreUtilities.TryGetListElementType(fieldInfo.FieldType), 0);
                        break;
                    }
                    case MemberTypes.Property: {
                        var propertyInfo = (PropertyInfo) memberInfo;
                        type  = typeof(PropertyInfo);
                        attr  = propertyInfo.GetCustomAttribute<ListDrawerSettingsAttribute>();
                        value = propertyInfo.GetValue(target) ?? Array.CreateInstance(CoreUtilities.TryGetListElementType(propertyInfo.PropertyType), 0);
                        break;
                    }
                }

                if (attr != null) {
                    var hideHeader = attr.HideHeader;
                    
                    list = new ReorderableList(value as IList, value.GetType(), attr.AllowDrag, !attr.HideHeader,
                        !attr.HideAddButton, !attr.HideRemoveButton);

                    SetCallbacks(rect, type, memberInfo, list, label, hideHeader);

                    if (rect == Rect.zero)
                        list.DoLayoutList();
                    else
                        list.DoList(rect);
                    
                    return list.list;
                }

                if (value == null) {
                    return null;
                }

                list = new ReorderableList(value as IList, value.GetType(), true, true, true, true);

                SetCallbacks(rect, type, memberInfo, list, label);

                instance.reorderableLists[memberInfo.Name] = list;
            }

            list = instance.reorderableLists[memberInfo.Name];
            if (rect == Rect.zero)
                list.DoLayoutList();
            else
                list.DoList(rect);
            
            return list.list;
        }

        private static void SetCallbacks(Rect rect, Type type, MemberInfo memberInfo, 
            ReorderableList reorderableList, GUIContent label, bool hideHeader = false) {
            
            if (!hideHeader) {
                reorderableList.drawHeaderCallback = tempRect => {
                    EditorGUI.LabelField(tempRect,
                        new GUIContent($"{ObjectNames.NicifyVariableName(memberInfo.Name)} - {reorderableList.count} elements"));
                };
            }

            reorderableList.drawElementCallback = (tempRect, index, active, focused) => {
                var element = reorderableList.list[index];
                
                //rect.y          += EditorGUIUtility.singleLineHeight;
                tempRect.y      += GuiUtilities.SPACE / 2f;
                tempRect.height =  EditorGUIUtility.singleLineHeight;
                tempRect.x      += 10.0f;
                tempRect.width  -= 10.0f;
                
                if (type == typeof(PropertyInfo)) {
                    var property = (PropertyInfo) memberInfo;
                    if (!property.CanWrite) {
                        using(new EditorGUI.DisabledScope(true)) {
                            reorderableList.draggable  = false;
                            reorderableList.displayAdd = reorderableList.displayRemove = false;
                            GuiUtilities.Field(property.PropertyType, element, tempRect, new GUIContent($"Element {index}"));
                        }
                        return;
                    }
                }

                Type declaredType       = null;
                switch (memberInfo.MemberType) {
                    case MemberTypes.Field:
                        declaredType = ((FieldInfo) memberInfo).FieldType.GetElementType();
                        break;
                    case MemberTypes.Property:
                        declaredType = ((PropertyInfo) memberInfo).PropertyType.GetElementType();
                        break;
                }

                if (CoreUtilities.IsPrimitiveUnityType(declaredType)) {
                    reorderableList.list[index] = GuiUtilities.Field(declaredType, element, tempRect,
                        label);
                }

                else {
                    reorderableList.list[index] =  GuiUtilities.MultiField(tempRect, memberInfo, element, label);
                }
            };
            
            var listType = CoreUtilities.TryGetListElementType(reorderableList.list.GetType());

            reorderableList.onAddCallback = delegate {
                var copy = reorderableList.list;
                
                reorderableList.list = Array.CreateInstance(listType, copy.Count + 1);
                for (var i = 0; i < copy.Count; i++) {
                    reorderableList.list[i] = copy[i];
                }
            };
            
            reorderableList.onRemoveCallback = delegate {
                var copy      = reorderableList.list;
                var newLength = copy.Count - 1;
                
                reorderableList.list = Array.CreateInstance(listType, newLength);
                for (var i = 0; i < reorderableList.index; i++) 
                    reorderableList.list[i] = copy[i];

                for (var i = reorderableList.index; i < newLength; i++) 
                    reorderableList.list[i] = copy[i + 1];
            };

            reorderableList.elementHeightCallback = index => {
                return (EditorGUIUtility.singleLineHeight + GuiUtilities.SPACE) * 12;
            };
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

                var endProperty = copy.GetEndProperty();
                
                copy.NextVisible(true);

                do {
                    //var content          = CoreUtilities.GetGUIContent(copy);
                    /*var decoratorsHeight = GuiUtilities.GetDecoratorsHeight(copy);
                    
                    if (decoratorsHeight > 0) {
                        //var amount = GuiUtilities.AmountOfDecorators(copy);
                        GuiUtilities.HandleDecorators(copy, tempRect, true);
                        tempRect.y += decoratorsHeight + GuiUtilities.SPACE; //* amount;
                    }*/

                    EditorGUI.PropertyField(new Rect(tempRect.x, tempRect.y, 
                        tempRect.width, EditorGUIUtility.singleLineHeight), copy, GUIContent.none, true);
                    
                    EditorGUI.indentLevel =  level;
                    tempRect.y            += EditorGUIUtility.singleLineHeight;
                } while (copy.NextVisible(true) && !SerializedProperty.EqualContents(copy, endProperty));
            };
            
            reorderableList.onAddCallback = delegate {
                property.arraySize++;
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
                var last    = element.GetEndProperty();

                float decoratorHeight = 0;
                var   height          = 0f;
                var   amount          = 0;

                //allocate total height.
                do { 
                    //decoratorHeight += GuiUtilities.GetDecoratorsHeight(element);
                    height          += EditorGUIUtility.singleLineHeight;
                    
                    //var currAmount = GuiUtilities.AmountOfDecorators(element);
                    /*if(currAmount <= 0)
                        continue;

                    amount          += currAmount;*/
                } while (element.NextVisible(true) && !SerializedProperty.EqualContents(element, last));

                height -= EditorGUIUtility.singleLineHeight;
                return height + decoratorHeight + GuiUtilities.SPACE * amount + 8.0f;
            };
        }
    }
}