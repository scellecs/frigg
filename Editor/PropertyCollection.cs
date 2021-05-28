namespace Frigg.Editor {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;
    using Utils;
    
    public class PropertyCollection : IEnumerable<FriggProperty> {
        private Dictionary<int, PropertyMeta>      metaByIndex    = new Dictionary<int, PropertyMeta>();
        private Dictionary<int, FriggProperty> propByIndex = new Dictionary<int, FriggProperty>();

        private FriggProperty property;

        //represents last updated index for this property
        private int index = -1;

        public PropertyCollection(FriggProperty prop) {
            if (prop == null)
                return;

            var meta  = prop.MetaInfo;

            if (!CoreUtilities.IsBuiltIn(meta.MemberType) || prop.IsRootProperty) {
                this.property = prop;

                var    membersArray = new List<PropertyValue>();
                object newTarget;

                if (prop.IsRootProperty) {
                    newTarget = prop.ParentValue;
                }
                
                else {
                    if (prop.ParentValue is IEnumerable) {
                        var list = (IList) prop.ParentValue;
                        newTarget = list[prop.MetaInfo.arrayIndex];
                    }
                    else {
                        newTarget = CoreUtilities.GetTargetObject(prop.ParentValue, meta.MemberInfo);
                    }
                }
                
                if(typeof(IEnumerable).IsAssignableFrom(meta.MemberType)) {
                    this.property.MetaInfo.isArray = true;
                    var list = (IList) CoreUtilities.GetTargetObject(prop.ParentValue, meta.MemberInfo);

                    this.property.MetaInfo.arraySize = list.Count;
                    for (var i = 0; i < list.Count; i++) {
                        this.propByIndex[i] = FriggProperty.DoProperty(prop.PropertyTree, prop, list, new PropertyMeta {
                            Name       = list[i].GetType().Name,
                            MemberType = list[i].GetType(),
                            MemberInfo = list[i].GetType(),
                            arrayIndex = i
                        });
                    }
                    
                    return;
                }

                if (newTarget == null) {
                    
                }
                this.SetMembers(this.property, newTarget, membersArray);
            }
        }

        public void SetMembers(FriggProperty prop, object target, List<PropertyValue> members) {
            target.TryGetMembers(members);

            if (members.Count == 0) {
                return;
            }

            var ordered = members.OrderBy(x => x.MetaInfo.Order);
            members = ordered.ToList();
            
            foreach (var member in members) {
                if (member.MetaInfo.MemberInfo.GetCustomAttribute<HideInInspector>() != null) {
                    continue;
                }
                
                this.index++;
                this.propByIndex[this.index] = FriggProperty.DoProperty(prop.PropertyTree, prop, target, new PropertyMeta {
                    Name       = member.MetaInfo.Name,
                    MemberType = member.MetaInfo.MemberType,
                    MemberInfo = member.MetaInfo.MemberInfo
                });
            }
        }

        public int AmountOfChildren => this.propByIndex.Count;

        public FriggProperty this[int indexer] => this.GetByIndex(indexer);

        public FriggProperty GetByIndex(int indexer) {
            if (this.propByIndex.TryGetValue(indexer, out var prop)) {
                //Debug.Log($"returned {indexer} - {prop.Path}");
                return prop;
            }
            
            //In case if property by index is not initialized
            var parentProp = this.property == this.property.PropertyTree.RootProperty ? null : this.property;

            var target = this.property.ParentValue;
            
            //Allows us to fill it in runtime
            if (this.property.MetaInfo.isArray) {
                var newTarget = CoreUtilities.GetTargetObject(target, this.property.MetaInfo.MemberInfo);
                var arrayElement = ((IList) newTarget)[indexer];
                prop = FriggProperty.DoProperty(this.property.PropertyTree, parentProp, target, new PropertyMeta {
                    Name       = arrayElement.GetType().Name,
                    MemberType = arrayElement.GetType(),
                    MemberInfo = arrayElement.GetType(),
                    arrayIndex = indexer
                }); 
            }

            else {
                prop = FriggProperty.DoProperty(this.property.PropertyTree, parentProp, target, new PropertyMeta {
                    Name       = this.property.MetaInfo.Name, 
                    MemberType = target.GetType()
                }); 
            }
            this.propByIndex[indexer] = prop;
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
    }
}