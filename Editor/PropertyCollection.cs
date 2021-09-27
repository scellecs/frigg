﻿namespace Frigg.Editor {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Packages.Frigg.Editor.Utils;
    using UnityEngine;
    using Utils;
    
    public class PropertyCollection : IEnumerable<FriggProperty> {
        private Dictionary<int, PropertyMeta>      metaByIndex    = new Dictionary<int, PropertyMeta>();
        private Dictionary<int, FriggProperty> propByIndex = new Dictionary<int, FriggProperty>();

        public FriggProperty property;

        //represents last updated index for this property
        private int index = -1;

        //Represent children components of this property
        public PropertyCollection(FriggProperty prop) {
            if (prop == null)
                return;
            
            var meta  = prop.MetaInfo;

            if (CoreUtilities.IsBuiltIn(meta.MemberType) && !prop.IsRootProperty) {
                return;
            }

            this.property = prop;

            var    membersArray = new List<PropertyValue>();
            object target;

            if(typeof(IList).IsAssignableFrom(meta.MemberType)) {
                this.property.MetaInfo.isArray = true;
                var list = (IList) prop.GetValue();

                this.property.Value.ActualValue = list;

                this.property.MetaInfo.arraySize = list.Count;
                for (var i = 0; i < list.Count; i++) {
                    this.propByIndex[i] = FriggProperty.DoProperty(this.property.PropertyTree, this.property, new PropertyMeta {
                        Name       = list[i].GetType().Name,
                        MemberType = list[i].GetType(),
                        MemberInfo = list[i].GetType(),
                        arrayIndex = i,
                    });
                }
                    
                return;
            }
                
            if (prop.IsRootProperty) {
                target = prop.Value.ActualValue;
            }
                
            else {
                if (prop.Value.Parent is IList list) {
                    target = list[prop.MetaInfo.arrayIndex];
                }
                else {
                    target = prop.GetValue();
                }
            }

            if (target == null) {
                Debug.Log($"Target is null with {this.property.Name}");
                return;
            }
            
            target.TryGetMembers(membersArray);
            this.SetMembers(this.property, membersArray);
        }

        public void Update() {
            if (this.property.Value.ActualValue is IList list) {
                for (var i = 0; i < list.Count; i++) {
                    if (!this.propByIndex.ContainsKey(i)) {
                        this.propByIndex[i] = FriggProperty.DoProperty(this.property.PropertyTree, 
                            this.property, new PropertyMeta {
                                Name       = list[i].GetType().Name,
                                MemberType = list[i].GetType(),
                                MemberInfo = list[i].GetType(),
                                arrayIndex = i
                            });

                        this.property.MetaInfo.arraySize++;
                    }
                    
                    this.propByIndex[i].Value.ActualValue = list[i];
                    this.propByIndex[i].ChildrenProperties.Update();
                }
            }

            else {
                foreach (var child in this.RecurseChildren()) {
                    child.Value.ActualValue = this.property.GetValue();
                    child.Value.ActualValue = CoreUtilities.GetTargetValue(this.property.Value.ActualValue, child.MetaInfo.MemberInfo);
                }
            }
        }

        public int AmountOfChildren => this.propByIndex.Count;

        public FriggProperty this[int idx] => this.GetByIndex(idx);

        public FriggProperty GetByIndex(int idx) {
            if (this.propByIndex.TryGetValue(idx, out var prop)) {
                return prop;
            }
            
            //In case if property by index is not initialized
            var parentProp = this.property == this.property.PropertyTree.RootProperty ? null : this.property;

            var target = this.property.Value.Parent;
            
            //Allows us to fill it in runtime
            if (this.property.MetaInfo.isArray) {
                var list = (IList) this.property.GetValue();
                
                if (list.Count < this.property.MetaInfo.arraySize) {
                    var copy = list;
                    
                    list = Array.CreateInstance(CoreUtilities.TryGetListElementType(list.GetType()), copy.Count + 1);
                    for (var i = 0; i < copy.Count; i++) {
                        list[i] = copy[i];
                    }

                    this.property.Value.ActualValue = list;
                }
                
                var arrayElement = list[idx];
                
                if (arrayElement == null) {
                    arrayElement = Activator.CreateInstance(CoreUtilities.TryGetListElementType(list.GetType()));
                    list[idx] = (arrayElement);
                    
                    this.property.SetValue(list);
                }
                
                prop = FriggProperty.DoProperty(this.property.PropertyTree, this.property, new PropertyMeta {
                    Name       = arrayElement.GetType().Name,
                    MemberType = arrayElement.GetType(),
                    MemberInfo = arrayElement.GetType(),
                    arrayIndex = idx
                });
            }

            else {
                prop = FriggProperty.DoProperty(this.property.PropertyTree, parentProp, new PropertyMeta {
                    Name       = this.property.MetaInfo.Name, 
                    MemberType = target.GetType()
                }); 
            }

            this.propByIndex[idx] = prop;
            return prop;
        }

        public IEnumerator<FriggProperty> GetEnumerator() {
            for (var i = 0; i < this.AmountOfChildren; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            for (var i = 0; i < this.AmountOfChildren; i++)
            {
                yield return this[i];
            }
        }

        public IEnumerable<FriggProperty> RecurseChildren(bool includeArray = false) {
            var amountOfChildren = this.AmountOfChildren;
            
            for (var i = 0; i < amountOfChildren; i++) {
                var child = this[i];
                yield return child;

                if(includeArray)
                    foreach (var nextChild in child.ChildrenProperties.RecurseChildren(true)) {
                        yield return nextChild; 
                    }
            }
        }
        
        private void SetMembers(FriggProperty prop, List<PropertyValue> members) {
            var ordered = members.OrderBy(x => x.MetaInfo.Order);
            members = ordered.ToList();

            foreach (var member in members) {
                if (member.MetaInfo.MemberInfo.GetCustomAttribute<HideInInspector>() != null) {
                    continue;
                }
                
                this.index++;

                //Do own property for each MemberInfo inside a target object
                this.propByIndex[this.index] = FriggProperty.DoProperty(prop.PropertyTree, prop, new PropertyMeta {
                    Name       = member.MetaInfo.Name,
                    MemberType = member.MetaInfo.MemberType,
                    MemberInfo = member.MetaInfo.MemberInfo
                });
            }
        }
    }
}