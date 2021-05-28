namespace Frigg.Editor {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Groups;
    using UnityEditor;
    using UnityEditor.Graphs;
    using UnityEngine;
    using Utils;
    using EditorGUILayout = UnityEditor.Experimental.Networking.PlayerConnection.EditorGUILayout;

    public class FriggProperty {
        public  GUIContent               Label           { get; set; }
        
        public IEnumerator<FriggDrawer> QueueEnumerator { get; set; }

        public string Path { get; private set; }

        private int  drawersCount;
        
        public PropertyMeta MetaInfo { get; set; }
        
        public object ParentValue    { get; set; }
        public object ActualValue    { get; set; }
        
        public bool   IsRootProperty { get; set; }
        public bool   IsExpanded     { get; set; }

        public string Name           { get; set; }
        public string NiceName       { get; set; }

        public FriggProperty ParentProperty { get; set; }

        public PropertyTree PropertyTree { get; set; }

        //properties inside this friggProperty.
        public PropertyCollection ChildrenProperties { get; set; }

        private List<Attribute> fixedAttributes      = new List<Attribute>();

        public List<Attribute> FixedAttributes     => this.fixedAttributes;

        public void Draw(Rect rect = default) {
            var current = this.QueueEnumerator.Current;
            if (current != null && current.IsVisible) {
                if(rect == default)
                    this.QueueEnumerator.Current?.DrawLayout();
                else {
                    this.QueueEnumerator.Current?.Draw(rect);
                }
            }

            else {
                this.CallNextDrawer(rect);
            }
        }
        
        public void CallNextDrawer(Rect rect = default) {
            if (this.QueueEnumerator.MoveNext()) {
                this.Draw(rect);
            }
            else {
                this.QueueEnumerator.Reset();
            }
        }

        public FriggProperty GetArrayElementAtIndex(int index) {
            if (this.MetaInfo.isArray) {
                return this.ChildrenProperties.GetByIndex(index);
            }

            throw new Exception("Specified property is not an array!");
        }

        public static float GetPropertyHeight(FriggProperty property, bool includeChildren = true) => CalculateDrawersHeight(property, includeChildren);

        private static float CalculateDrawersHeight(FriggProperty prop, bool includeChildren) {
            var total      = 0f;

            if (!prop.IsExpanded) {
                return EditorGUIUtility.singleLineHeight;
            }

            if (prop.ChildrenProperties.AmountOfChildren == 0) {
                do {
                    if (prop.QueueEnumerator.Current == null) {
                        continue;
                    }

                    if (prop.QueueEnumerator.Current.IsVisible) {
                        total += prop.QueueEnumerator.Current.GetHeight();
                    }
                } while (prop.QueueEnumerator.MoveNext());

                prop.QueueEnumerator.Reset();
                
                return total;
            }

            total += EditorGUIUtility.singleLineHeight;

            var properties = includeChildren ? prop.ChildrenProperties : prop.ChildrenProperties.RecurseChildren(true);
            foreach (var property in properties) {
                do {
                    if (property.QueueEnumerator.Current == null) {
                        continue; 
                    }
                    
                    if (property.QueueEnumerator.Current.IsVisible) {
                        total += property.QueueEnumerator.Current.GetHeight();
                    }
                }
                while (property.QueueEnumerator.MoveNext());
                
                property.QueueEnumerator.Reset();
            }

            return total;
        }

        public void Update() {
            this.PropertyTree.SerializedObject.Update();
        }
        
        public T TryGetFixedAttribute<T>() where T : Attribute {
            var attr = this.fixedAttributes.FirstOrDefault(x => x.GetType().IsAssignableFrom(typeof(T)));
            return (T) attr;
        }
        
        internal static FriggProperty DoProperty(PropertyTree tree, FriggProperty parent, object parentTarget, PropertyMeta metaInfo) {
            var property = new FriggProperty {PropertyTree = tree};
            property.ParentValue = parentTarget;
            property.MetaInfo    = metaInfo;

            if (parent != null) {
                property.ParentProperty = parent;
            }
            else {
                property.IsRootProperty = true;
            }
            
            property.Name     = metaInfo.Name;
            property.NiceName = ObjectNames.NicifyVariableName(metaInfo.Name);
            property.Label    = new GUIContent(property.NiceName);

            var parentObjects = new List<string>();
            var prop          = property;
            while (prop != null) {
                var currPath = string.Empty;

                if (prop.IsRootProperty) {
                    currPath += prop.PropertyTree.SerializedObject.targetObject.name + ".";
                }
                
                var index    = prop.MetaInfo.arrayIndex;
                if (index == -1) {
                    currPath += prop.Name;
                }

                if (index != -1) {
                    currPath += $"[{index}]";
                }
                
                currPath += ".";

                parentObjects.Add(currPath);
                prop = prop.ParentProperty;
            }
            
            parentObjects.Reverse();

            foreach (var obj in parentObjects) {
                property.Path += obj;
            }

            property.Path = property.Path.Substring(0, property.Path.Length-1);

            //If this property can't have any children properties (is not struct or built-in unity type)
            property.fixedAttributes = !property.IsRootProperty 
                ? property.MetaInfo.MemberInfo.GetCustomAttributes().ToList() 
                : property.MetaInfo.MemberType.GetCustomAttributes().ToList();
            
            property.ChildrenProperties = new PropertyCollection(property);

            var drawers = FriggDrawer.Resolve(property).ToList();
            property.drawersCount = drawers.Count;

            property.QueueEnumerator = drawers.GetEnumerator();
            property.HandleMetaAttributes();

            return property;
        }   
    }
}