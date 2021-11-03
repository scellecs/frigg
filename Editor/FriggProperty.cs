namespace Frigg.Editor {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Groups;
    using JetBrains.Annotations;
    using Layouts;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEditor.Graphs;
    using UnityEngine;
    using Utils;
    using Object = UnityEngine.Object;

    public class FriggProperty {
        public SerializedProperty NativeProperty { get; set; }
        
        public GUIContent         Label              { get; set; }

        public int ObjectInstanceID { get; set; } = -1;
        
        //Represents a queue of Frigg drawers.
        public IEnumerator<FriggDrawer> DrawersQueue { get; set; }

        //Represents all drawers of target property.
        public IEnumerable<FriggDrawer> Drawers { get; set; }

        /// <summary>
        /// Path, declared on target property by Frigg inspector.
        /// </summary>
        public string Path      { get; private set; }
        
        /// <summary>
        /// Path appropriate to Unity's Serialized Property. Empty if property is missing.
        /// </summary>
        public string UnityPath { get; private set; }

        /// <summary>
        /// Property's meta info.
        /// </summary>
        public PropertyMeta MetaInfo => this.NativeValue.MetaInfo;

        /// <summary>
        /// Property's actual value.
        /// </summary>
        public PropertyValue<object> NativeValue { get; private set; }

        /// <summary>
        /// Indicates whether property is root(e.g base property).
        /// </summary>
        public bool IsRootProperty { get; private set; }
        
        /// <summary>
        /// Indicates whether property is Expanded in Unity Editor.
        /// </summary>
        public bool IsExpanded     { get; set; }
        
        //Don't need to use this? Check for an attribute and then remove from list?
        public bool IsLayoutMember  { get; set; }

        /// <summary>
        /// Property's reflected name.
        /// </summary>
        public string Name     { get; set; }
        
        /// <summary>
        /// Property's displayable name.
        /// </summary>
        public string NiceName { get; set; }

        /// <summary>
        /// Parent property of target property.
        /// </summary>
        public FriggProperty ParentProperty { get; set; }
        /// <summary>
        /// Main property tree for target property.
        /// </summary>
        public PropertyTree  PropertyTree   { get; set; }

        /// <summary>
        /// Children properties of target property.
        /// </summary>
        public PropertyCollection ChildrenProperties { get; set; }

        /// <summary>
        /// Applied attributes on target property.
        /// </summary>
        /// 
        public IEnumerable<Attribute> FixedAttributes { get; private set; }
        //All applied attributes on target property.
        //private List<Attribute> fixedAttributes = new List<Attribute>();

        public FriggProperty(PropertyTree propertyTree, PropertyValue<object> value, bool isRoot = false) {
            this.NativeValue    = value;
            this.IsRootProperty = isRoot;
            this.PropertyTree   = propertyTree;
            this.Path           = propertyTree.TargetType.ToString();

            this.TrySetInstanceID(value.Get());
        }

        /// <summary>
        /// Creates a new Frigg property.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="metaInfo"></param>
        /// <returns></returns>
        internal static FriggProperty DoProperty(FriggProperty parent, [NotNull] PropertyMeta metaInfo) {
            var nativeValue = new PropertyValue<object>(null, metaInfo);
            
            if(parent != null && parent.MetaInfo.isArray) {
                nativeValue.Set(((IList) parent.NativeValue.Get())[metaInfo.arrayIndex]);
            }

            else {
                if (parent != null) {
                    nativeValue.Set(CoreUtilities.GetTargetValue
                        (parent.NativeValue.Get(), metaInfo.MemberInfo));
                }
            }

            var property = new FriggProperty(parent?.PropertyTree, nativeValue) {
                Name = metaInfo.Name, 
                NiceName = ObjectNames.NicifyVariableName(metaInfo.Name),
                ParentProperty = parent
            };

            property.ChildrenProperties = new PropertyCollection(property);
            
            property.Label          = new GUIContent(property.NiceName);
            
            property.FixedAttributes = !property.IsRootProperty
                ? property.MetaInfo.MemberInfo.GetCustomAttributes()
                : property.MetaInfo.MemberType.GetCustomAttributes();

            property.Path = GetFriggPath(property);

            property.TrySetInstanceID(property.GetValue());
            property.SetNativeProperty();
            
            property.ChildrenProperties = new PropertyCollection(property);

            var drawers = FriggDrawer.Resolve(property).ToList();
            property.MetaInfo.drawersCount = drawers.Count();

            property.Drawers      = drawers;
            property.DrawersQueue = drawers.GetEnumerator();
            
            property.HandleMetaAttributes();

            property.IsExpanded = EditorData.GetBoolValue(property.Path);

            var attr = property.TryGetFixedAttribute<BaseGroupAttribute>();

            if (attr == null) {
                return property;
            }
            
            property.IsLayoutMember = true;

            property.PropertyTree.LayoutsByPath.TryGetValue
                (property.ParentProperty.Path, out var layout);

            if (layout != null) {
                layout.Add(property);
                return property;
            }

            if (attr is HorizontalGroupAttribute) {
                layout = new HorizontalLayout {
                    IsListMember = property.ParentProperty.MetaInfo.arrayIndex != -1
                };
            }
            else {
                layout = new VerticalLayout();
            }

            layout.Add(property);

            property.PropertyTree.LayoutsByPath[layout.layoutPath] = layout;

            return property;
        }

        /// <summary>
        /// Draw property.
        /// </summary>
        /// <param name="rect">Rect that represents draw info</param>
        public void Draw(Rect rect = default) {
            var current = this.DrawersQueue.Current;
            if (current != null && current.IsVisible) {
                if (rect == default)
                    this.DrawersQueue.Current?.DrawLayout();
                else {
                    this.DrawersQueue.Current?.Draw(rect);
                }
            }

            else {
                this.CallNextDrawer(rect);
            }
        }

        /// <summary>
        /// Move to the next drawer in target property.
        /// </summary>
        /// <param name="rect"></param>
        public void CallNextDrawer(Rect rect = default) {
            if (this.DrawersQueue.MoveNext()) {
                this.Draw(rect);
            }
            else {
                this.DrawersQueue.Reset();
            }
        }

        /// <summary>
        /// Access property on required index.
        /// </summary>
        /// <param name="index">Required index.</param>
        /// <returns>Property on required index.</returns>
        /// <exception cref="Exception">Property is not an array.</exception>
        public FriggProperty GetArrayElementAtIndex(int index) {
            if (!this.MetaInfo.isArray) {
                throw new Exception("Specified property is not an array!");
            }
            
            var fProperty = this.ChildrenProperties.GetByIndex(index);
            return fProperty;

        }

        /// <summary>
        /// Calculates property's required height.
        /// </summary>
        /// <param name="property">Target property</param>
        /// <param name="includeChildren">Calculate children elements height?</param>
        /// <returns>Total property's height.</returns>
        public static float GetPropertyHeight(FriggProperty property, bool includeChildren = true) => CalculateDrawersHeight(property, includeChildren);

        private static float CalculateDrawersHeight(FriggProperty prop, bool includeChildren) {
            var total = 0f;

            if (prop.IsLayoutMember) {
                return EditorGUIUtility.singleLineHeight;
            }
            
            foreach (var drawer in prop.Drawers) {
                var height = drawer.GetHeight();
                if (height > 0) {
                    total += height + GuiUtilities.SPACE;   
                }
            }

            return total;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Refresh() {
            var parent = this.ParentProperty;
            if (parent != null && parent.MetaInfo.isArray) {
                var list = (IList) parent.NativeValue.Get();
                if (list.Count != parent.MetaInfo.arraySize)
                    return;
                    
                this.NativeValue.Set(list[this.MetaInfo.arrayIndex]);
            }

            else {
                if (parent != null) {
                    this.NativeValue.Set(CoreUtilities.GetTargetValue
                        (parent.NativeValue.Get(), this.MetaInfo.MemberInfo));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newValue"></param>
        public void UpdateValue(object newValue) {
            this.SetValue(newValue);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T TryGetFixedAttribute<T>() where T : Attribute {
            var attr = this.FixedAttributes.FirstOrDefault(x => x is T);
            return (T) attr;
        }

        private void TrySetInstanceID(object value) {
            if (!(value is Object obj)) {
                return;
            }
            
            this.ObjectInstanceID = obj.GetInstanceID();
        }
        
        private void SetNativeProperty() {
            var prop = this.ParentProperty?.NativeProperty?.FindPropertyRelative(this.Name);
            if (prop != null) {
                this.UnityPath      = prop.propertyPath;
                this.NativeProperty = prop;
                return;
            }

            prop = this.PropertyTree.SerializedObject.FindProperty(this.Name);
            if (prop != null) {
                this.UnityPath      = prop.propertyPath;
                this.NativeProperty = prop;
                return; 
            }

            //Initial property type for this tree, because our property wasn't created
            var root = this;
            while (!root.IsRootProperty) {
                root = root.ParentProperty;
            }
            
            var rootType = this.PropertyTree.TargetType;

            //Catch all scripts on scene with specified type (root).
            if (!typeof(Object).IsAssignableFrom(rootType)) {
                return;
            }
            
            //Find correct object using Unique Id comparison.
            var obj = Object.FindObjectsOfType(rootType)
                ?.FirstOrDefault(x => x.GetInstanceID() == root.ObjectInstanceID);

            if (obj == null) {
                Debug.Log(rootType);
                return;
            }

            var so       = new SerializedObject(obj);
            var iterator = so.GetIterator();

            while (iterator.Next(true)) {
                var relative = iterator.FindPropertyRelative(this.Name);
                if (relative == null) {
                    if (iterator.isArray && iterator.arraySize > 0
                                         && this.MetaInfo.arrayIndex != -1
                                         && this.ParentProperty?.Name == iterator.name) {
                        var arrayElement = iterator.GetArrayElementAtIndex
                            (this.MetaInfo.arrayIndex);
                        
                        this.UnityPath      = arrayElement.propertyPath;
                        this.NativeProperty = arrayElement;

                        return;
                    }
                    continue;
                }
                        
                this.UnityPath      = relative.propertyPath;
                this.NativeProperty = relative;
            }
        }

        private static string GetFriggPath(FriggProperty property) {
            var parentObjects = new List<string>();
            var prop          = property;
            while (prop != null) {
                var currPath = string.Empty;

                if (prop.IsRootProperty) {
                    currPath = prop.PropertyTree.SerializedObject.targetObject.name + ".";
                }
                
                var index = prop.MetaInfo.arrayIndex;
                if (index == -1) {
                    currPath += prop.Name;
                    currPath += ".";
                }

                if (index != -1) {
                    currPath += $"[{index}].";
                }

                if (prop.MetaInfo.isArray)
                    currPath += "Array.data";

                parentObjects.Add(currPath);
                prop = prop.ParentProperty;
            }

            parentObjects.Reverse();

            foreach (var obj in parentObjects) {
                property.Path += obj;
            }

            return property.Path.Substring(0, property.Path.Length - 1);
        }
    }
}