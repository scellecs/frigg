namespace Frigg.Editor {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public class PropertyCollection : IEnumerable<FriggProperty> {
        private Dictionary<int, PropertyMeta>  metaByIndex = new Dictionary<int, PropertyMeta>();
        private Dictionary<int, FriggProperty> propByIndex = new Dictionary<int, FriggProperty>();

        public FriggProperty property;

        //represents last updated index for this property
        private int index = -1;

        //Represent children components of this property
        public PropertyCollection(FriggProperty prop) {
            if (prop?.GetValue() == null)
                return;

            var meta = prop.MetaInfo;

            if (CoreUtilities.IsBuiltIn(meta.MemberType) && !prop.IsRootProperty) {
                return;
            }

            this.property = prop;
            object target;

            if (typeof(IList).IsAssignableFrom(meta.MemberType)) {
                this.property.MetaInfo.isArray = true;
                var list        = (IList) prop.GetValue();
                var elementType = CoreUtilities.TryGetListElementType(list.GetType());

                this.property.NativeValue.Set(list);
                this.property.MetaInfo.arraySize = list.Count;

                for (var i = 0; i < list.Count; i++) {
                    if (list[i] == null) {
                        this.propByIndex[i] = FriggProperty.DoProperty(this.property, new PropertyMeta {
                            Name       = elementType.Name,
                            MemberType = elementType,
                            MemberInfo = elementType,
                            arrayIndex = i,
                        });
                        continue;
                    }

                    this.propByIndex[i] = FriggProperty.DoProperty(this.property, new PropertyMeta {
                        Name       = list[i].GetType().Name,
                        MemberType = list[i].GetType(),
                        MemberInfo = list[i].GetType(),
                        arrayIndex = i,
                    });
                }

                return;
            }

            if (prop.IsRootProperty) {
                target = prop.GetValue();
            }

            else {
                if (prop.ParentProperty.GetValue() is IList list) {
                    target = list[prop.MetaInfo.arrayIndex];
                }
                else {
                    target = prop.GetValue();
                }
            }

            if (target == null) {
                return;
            }

            var membersArray = new List<PropertyValue<object>>();
            target.TryGetMembers(membersArray);
            this.SetMembers(this.property, membersArray);
        }

        public int AmountOfChildren => this.propByIndex.Count;

        public FriggProperty this[int idx] => this.GetByIndex(idx);

        public void AddElement(int idx) {
            var oldList     = (IList) this.property.GetValue();
            var elementType = CoreUtilities.TryGetListElementType(oldList.GetType());
            var newLength   = oldList.Count + 1;
            
            var newList     = (IList) Activator.CreateInstance(oldList.GetType());
            
            for (var i = 0; i < newLength - 1; i++) {
                newList.Insert(i, oldList[i]);
            }

            //fix for immutable types
            if (!elementType.IsValueType) {
                newList.Insert(newLength - 1, elementType == typeof(string)
                    ? string.Empty
                    : Activator.CreateInstance(elementType));
            }
            else {
                newList.Insert(newLength - 1, elementType.IsAbstract
                    ? null
                    : Activator.CreateInstance(elementType));
            }

            this.property.SetValue(newList);
            if (this.property.NativeProperty != null) {
                EditorUtility.SetDirty(this.property.PropertyTree
                    .SerializedObject.targetObject);
            }

            this.property.MetaInfo.arraySize++;
            
            this.propByIndex[idx] = FriggProperty.DoProperty(this.property, new PropertyMeta {
                arrayIndex = idx,
                Name       = elementType.Name,
                MemberInfo = elementType,
                MemberType = elementType
            });
        }

        public void RemoveElement(int idx) {
            var list      = (IList) this.property.GetValue();
            var newLength = list.Count - 1;

            var elementType = CoreUtilities.TryGetListElementType(list.GetType());
            
            var newArray = Array.CreateInstance(CoreUtilities.TryGetListElementType(list.GetType()),
                newLength);

            for (var i = 0; i <= idx; i++) {
                newArray.SetValue(list[i], i);
            }

            this.property.PropertyTree.LayoutsByPath.TryGetValue(this.propByIndex[idx].Path, out var value);
            value?.ResetLayout();

            for (var i = idx + 1; i < newLength; i++) {
                newArray.SetValue(list[i + 1], i);

                this.property.PropertyTree.LayoutsByPath.TryGetValue(this.propByIndex[i].Path, out value);
                value?.ResetLayout();

                var meta = CreateMeta(list, elementType, i);

                //GetValue is possible as null?
                this.propByIndex[i] = FriggProperty.DoProperty(this.property, meta);
            }

            this.property.PropertyTree.LayoutsByPath.TryGetValue(this.propByIndex[newLength].Path, out value);
            value?.ResetLayout();

            this.property.SetValue(newArray);

            Debug.Log($"Element removed at index {idx}");
            this.propByIndex.Remove(newLength);
            this.property.MetaInfo.arraySize--;
        }

        public FriggProperty GetByIndex(int idx) {
            if (this.propByIndex.TryGetValue(idx, out var prop)) {
                return prop;
            }

            //In case if property by index is not initialized
            var parentProp = this.property == this.property.PropertyTree.RootProperty
                ? null
                : this.property;

            //Allows us to fill it in runtime
            if (this.property.MetaInfo.isArray) {
                this.AddElement(idx);
                return this.propByIndex[idx];
            }

            var target = this.property.ParentProperty.GetValue();
            prop = FriggProperty.DoProperty(parentProp, new PropertyMeta {
                Name       = this.property.MetaInfo.Name,
                MemberType = target.GetType()
            });

            this.propByIndex[idx] = prop;
            return prop;
        }

        public IEnumerator<FriggProperty> GetEnumerator() {
            for (var i = 0; i < this.AmountOfChildren; i++) {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            for (var i = 0; i < this.AmountOfChildren; i++) {
                yield return this[i];
            }
        }

        public IEnumerable<FriggProperty> RecurseChildren(bool includeArray = false) {
            var amountOfChildren = this.AmountOfChildren;

            for (var i = 0; i < amountOfChildren; i++) {
                var child = this[i];
                yield return child;

                if (!includeArray)
                    continue;

                foreach (var nextChild in child.ChildrenProperties.RecurseChildren(true)) {
                    yield return nextChild;
                }
            }
        }

        private void SetMembers(FriggProperty prop, IEnumerable<PropertyValue<object>> members) {
            var ordered = members.OrderBy(x => x.MetaInfo.Order);

            foreach (var member in ordered) {
                if (member.MetaInfo.MemberInfo.IsDefined(typeof(HideInInspector))) {
                    continue;
                }

                this.index++;

                //Do own property for each MemberInfo inside a target object
                this.propByIndex[this.index] = FriggProperty.DoProperty(prop, new PropertyMeta {
                    Name       = member.MetaInfo.Name,
                    MemberType = member.MetaInfo.MemberType,
                    MemberInfo = member.MetaInfo.MemberInfo
                });
            }
        }

        private static PropertyMeta CreateMeta(IList list, Type elementType, int idx) {
            if (list[idx] == null) {
                return new PropertyMeta {
                    arrayIndex = idx,
                    MemberType = elementType,
                    MemberInfo = elementType,
                    Name       = elementType.Name
                };
            }

            //Get member and name
            return new PropertyMeta {
                arrayIndex = idx,
                MemberType = list[idx].GetType(),
                MemberInfo = list[idx].GetType(),
                Name       = list[idx].GetType().Name
            };
        }
    }
}