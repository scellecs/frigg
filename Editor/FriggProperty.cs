namespace Frigg.Editor {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Groups;
    using Layouts;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEditor.Graphs;
    using UnityEngine;
    using Utils;

    public class FriggProperty {
        //All applied attributes on target property.
        private List<Attribute> fixedAttributes = new List<Attribute>();
        public  GUIContent      Label { get; set; }

        //All drawers that were detected on target property.
        public IEnumerator<FriggDrawer> Drawers { get; set; }

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
        public PropertyMeta MetaInfo { get; set; }

        /// <summary>
        /// Property's actual value.
        /// </summary>
        public PropertyValue Value { get; set; }

        /// <summary>
        /// Indicates whether property is root(e.g base property).
        /// </summary>
        public bool IsRootProperty { get; set; }
        
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
        public PropertyCollection     ChildrenProperties { get; private set; }
        
        /// <summary>
        /// Applied attributes on target property.
        /// </summary>
        public IEnumerable<Attribute> FixedAttributes    => this.fixedAttributes;

        public FriggProperty(PropertyValue value) => this.Value = value;

        /// <summary>
        /// Draw property.
        /// </summary>
        /// <param name="rect">Rect that represents draw info</param>
        public void Draw(Rect rect = default) {
            var current = this.Drawers.Current;
            if (current != null && current.IsVisible) {
                if (rect == default)
                    this.Drawers.Current?.DrawLayout();
                else {
                    this.Drawers.Current?.Draw(rect);
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
            if (this.Drawers.MoveNext()) {
                this.Draw(rect);
            }
            else {
                this.Drawers.Reset();
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
            var sProperty = this.PropertyTree.SerializedObject.FindProperty(fProperty.UnityPath);
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

            if (!prop.IsExpanded) {
                return EditorGUIUtility.singleLineHeight;
            }

            if (prop.ChildrenProperties.AmountOfChildren == 0) {
                do {
                    if (prop.Drawers.Current == null) {
                        continue;
                    }

                    if (prop.Drawers.Current.IsVisible) {
                        total += prop.Drawers.Current.GetHeight();
                    }
                } while (prop.Drawers.MoveNext());

                prop.Drawers.Reset();

                return total;
            }

            total += EditorGUIUtility.singleLineHeight;

            var properties = includeChildren ? prop.ChildrenProperties : prop.ChildrenProperties.RecurseChildren(true);
            foreach (var property in properties) {
                do {
                    if (property.Drawers.Current == null) {
                        continue;
                    }

                    if (property.Drawers.Current.IsVisible) {
                        total += property.Drawers.Current.GetHeight();
                    }
                } while (property.Drawers.MoveNext());

                property.Drawers.Reset();
            }

            return total;
        }

        /// <summary>
        /// 
        /// </summary>
        public void RefreshChildren() {
            Debug.Log(this.MetaInfo.arraySize);
            this.ChildrenProperties = new PropertyCollection(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newValue"></param>
        public void Update(object newValue) {
            //Secondly, we need to set a new Value.
            this.SetValue(newValue);
            this.ChildrenProperties.property = this;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T TryGetFixedAttribute<T>() where T : Attribute {
            var attr = this.fixedAttributes.FirstOrDefault(x => x is T);
            return (T) attr;
        }

        /// <summary>
        /// Creates a new Frigg property.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="parent"></param>
        /// <param name="metaInfo"></param>
        /// <param name="isRoot"></param>
        /// <returns></returns>
        internal static FriggProperty DoProperty(PropertyTree tree, FriggProperty parent, PropertyMeta metaInfo, 
            bool isRoot = false) {
            var property = new FriggProperty(new PropertyValue()) {
                PropertyTree = tree, 
                MetaInfo = metaInfo,
            };
            
            if (isRoot) {
                property.IsRootProperty = true;
            }
            
            else {
                //assign parent object as parent property for current property
                property.ParentProperty = parent;
                
                //Set value's parent object as parent reference value
                property.Value.Parent = parent.Value.ActualValue;
            }

            property.Name     = metaInfo.Name;
            property.NiceName = ObjectNames.NicifyVariableName(metaInfo.Name);
            property.Label    = new GUIContent(property.NiceName);
            
            //If this property can't have any children properties (is not struct or built-in unity type)
            property.fixedAttributes = !property.IsRootProperty
                ? property.MetaInfo.MemberInfo.GetCustomAttributes().ToList()
                : property.MetaInfo.MemberType.GetCustomAttributes().ToList();

            if (property.IsRootProperty) {
                property.Value.ActualValue = parent.Value.ActualValue;
            }

            else if(property.ParentProperty != null && property.ParentProperty.MetaInfo.isArray) {
                property.Value.ActualValue = ((IList) property.ParentProperty.Value.ActualValue)[property.MetaInfo.arrayIndex];
            }

            else {
                property.Value.ActualValue = CoreUtilities.GetTargetValue(property.Value.Parent, property.MetaInfo.MemberInfo);
            }

            property.Path = GetFriggPath(property);

            property.UnityPath = property.GetUnityPath();

            property.ChildrenProperties = new PropertyCollection(property);

            var drawers = FriggDrawer.Resolve(property).ToList();
            property.MetaInfo.drawersCount = drawers.Count;

            property.Drawers = drawers.GetEnumerator();
            property.HandleMetaAttributes();

            property.IsExpanded = EditorData.GetBoolValue(property.Path);

            var attr = property.TryGetFixedAttribute<BaseGroupAttribute>();

            if (attr == null) {
                return property;
            }
            
            property.IsLayoutMember = true;

            var layout = property.PropertyTree.Layouts.FirstOrDefault(x => x.layoutPath == property.ParentProperty.Path);
            
            if (layout != null) {
                layout.Add(property);
                return property;
            }

            if (attr is HorizontalGroupAttribute) {
                layout = new HorizontalLayout();
            }
            else {
                layout = new VerticalLayout();
            }

            layout.Add(property);

            property.PropertyTree.Layouts.Add(layout);

            return property;
        }

        private string GetUnityPath() {
            var prop = this.PropertyTree.SerializedObject.GetIterator();
            prop.Next(true);

            var path     = string.Empty;
            var initPath = this.Path;
            while (prop.Next(true)) {
                if (!prop.isArray || prop.name == "Array") {
                    continue;
                }

                var splitData = initPath.Split(new[] {prop.name}, StringSplitOptions.None);
                if (splitData.Length <= 1) {
                    continue;
                }

                if (splitData[1].Length <= 1) {
                    continue;
                }

                var cleanValue = splitData[1].Remove(0, 1);
                var p          = prop.FindPropertyRelative(cleanValue);

                if (p != null)
                    path = p.propertyPath;
            }

            if (string.IsNullOrEmpty(path)) {
                prop.Reset();
                while (prop.Next(true)) {
                    var p = prop.FindPropertyRelative(this.Name);
                    if (p != null) {
                        path = p.propertyPath;
                    }
                }
            }

            prop.Reset();
            return path;
        }

        private static string GetFriggPath(FriggProperty property) {
            var parentObjects = new List<string>();
            var prop          = property;
            while (prop != null) {
                var currPath = string.Empty;

                if (prop.IsRootProperty) {
                    currPath += prop.PropertyTree.SerializedObject.targetObject.name + ".";
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