namespace Frigg.Editor {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using Utils;
    using Object = UnityEngine.Object;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    public class FriggInspector : Editor {
        public List<List<Member>> allMembers = new List<List<Member>>();
        
        //Unity's Serialized properties (including fields)
        private List<SerializedProperty> serializedProperties = new List<SerializedProperty>();
        private List<Member> members              = new List<Member>();

        private bool anySerialized;
        private bool hasArrays;

        private ILookup<int, object> mixedData;
        private void OnEnable() {
            this.target.TryGetMembers(this.members);

            this.serializedProperties = this.GetSerializedProperties();
            
            this.anySerialized = this.serializedProperties.Count > 0;
            
            this.hasArrays = this.serializedProperties.Any(p => p.isArray);
            
            this.mixedData = SortAll(this.serializedProperties, this.members);

            ReorderableListDrawer.ClearData();
        }
        
        public override void OnInspectorGUI() {
            this.serializedObject.Update();

            //means there's no arrays and props with attributes at all.
            if (!this.anySerialized && !this.hasArrays) {
                this.DrawDefaultInspector();
            }

            if(this.mixedData.Count <= 0)
                return;

            var keys = this.mixedData.Select(x => x.Key);

            foreach (var key in keys) {
                var elements   = this.mixedData[key];
                //var enumerable = elements.ToList();

                /*if(!enumerable.Any())
                    continue;*/

                foreach (var element in elements) {
                    var type = element.GetType();

                    if (type.FullName == null) {
                        continue;
                    }

                    if (type == typeof(Member)) {
                        var member = (Member) element;

                        switch (member.memberInfo.MemberType) {
                            case MemberTypes.Field:
                                DrawNonSerializedField(member);
                                break;
                            case MemberTypes.Method:
                                this.DrawButton(member);
                                break;
                            case MemberTypes.Property:
                                this.DrawNativeProperty(member);
                                break;
                        }
                    }

                    if (type != typeof(SerializedProperty)) {
                        continue;
                    }

                    var prop = (SerializedProperty) element;

                    if (prop.name == "m_Script") {
                        var propType = CoreUtilities.GetTargetObjectWithProperty(prop).GetType();
                        if (propType.IsDefined(typeof(HideMonoScriptAttribute))) {
                            continue;
                        }
                    }

                    this.DrawSerializedProperty((SerializedProperty) element);
                }
            }
        }

        private List<SerializedProperty> GetSerializedProperties() {
            var list = new List<SerializedProperty>();
            
            var it = this.serializedObject.GetIterator();

            if (!it.NextVisible(true)) {
                return list;
            }

            do {
                var prop = this.serializedObject.FindProperty(it.name);
                list.Add(prop);
            }
            
            while (it.NextVisible(false));

            return list;
        }

        private void DrawSerializedProperty(SerializedProperty prop) {
            this.serializedObject.Update();
            
            GuiUtilities.HandleDecorators(prop);
            GuiUtilities.PropertyField(prop, true);
        }
        
        private void DrawButton(Member member) {
            var info = (MethodInfo) member.memberInfo;

            GuiUtilities.HandleDecorators(info);
            GuiUtilities.Button(this.serializedObject.targetObject, info);
        }

        private static void DrawNonSerializedField(Member member) {
            var info   = (FieldInfo) member.memberInfo;
            var obj = member.target;
            
            if (info.IsUnitySerialized()) {
                return;
            }
            
            var value = info.GetValue(obj);

            GuiUtilities.HandleDecorators(obj.GetType());
            GuiUtilities.HandleDecorators(info);
            var content  = CoreUtilities.GetGUIContent(info);
            var writable = CoreUtilities.IsWritable(info);

            if (typeof(IEnumerable).IsAssignableFrom(info.FieldType) && info.FieldType != typeof(string)) {
                var drawer = (ReorderableListDrawer) CustomAttributeExtensions.GetCustomDrawer(typeof(ReorderableListAttribute));
                drawer.OnGUI(obj, Rect.zero, info, content);
                return;
            }
            
            info.SetValue(obj, GuiUtilities.LayoutField(info.FieldType, value, content, writable));
        }
        
        private void DrawNativeProperty(Member member) {
            var info = (PropertyInfo) member.memberInfo;
            var obj  = member.target;
            
            GuiUtilities.HandleDecorators(obj.GetType());
            GuiUtilities.HandleDecorators(info);
            
            var content = CoreUtilities.GetGUIContent(info);
            
            if (typeof(IEnumerable).IsAssignableFrom(info.PropertyType) && info.PropertyType != typeof(string)) {
                var drawer = (ReorderableListDrawer) CustomAttributeExtensions.GetCustomDrawer(typeof(ReorderableListAttribute));
                drawer.OnGUI(obj, Rect.zero, info, content);
                return;
            }

            var writable = CoreUtilities.IsWritable(info);
            var value    = info.GetValue(obj);

            if (CoreUtilities.IsPrimitiveUnityType(info.PropertyType)) {
                if(writable)
                    info.SetValue(obj, GuiUtilities
                       .LayoutField(info.PropertyType, info.GetValue(obj), content));
                else { 
                    GuiUtilities.LayoutField(info.PropertyType, info.GetValue(obj), content, false);
                }
            }

            else {
                info.SetValue(obj, GuiUtilities
                    .MultiFieldLayout(info, info.GetValue(obj), content, writable));
            }

            var secondValue = info.GetValue(obj);

            if (value.Equals(secondValue) || Application.isPlaying) {
                return;
            }
                
            EditorUtility.SetDirty(this.target);
        }

        private static ILookup<int, object> SortAll(IEnumerable<SerializedProperty> serProps,
            IEnumerable<Member> members) {
            var pairs = new List<KeyValuePair<int, object>>();

            foreach (var prop in serProps) {
                var attr = CoreUtilities.TryGetAttribute<OrderAttribute>(prop);
                if (prop.name == "m_Script") {
                    pairs.Add(new KeyValuePair<int, object>(-1000, prop));
                    continue;
                }

                pairs.Add(attr != null
                    ? new KeyValuePair<int, object>(attr.Order, prop)
                    : new KeyValuePair<int, object>(0, prop));
            }

            foreach (var member in members) {
                var attr  = member.memberInfo.GetCustomAttribute<OrderAttribute>();
                pairs.Add(attr != null
                    ? new KeyValuePair<int, object>(attr.Order, member)
                    : new KeyValuePair<int, object>(0, member));
            }

            var kvPairs = pairs.OrderBy(s => s.Key);
            return kvPairs.ToLookup(pair =>
                pair.Key, pair => pair.Value);
        }
    }
} 