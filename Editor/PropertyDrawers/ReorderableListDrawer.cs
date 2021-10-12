namespace Frigg.Editor {
    using System;
    using System.Collections;
    using System.Linq;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using Utils;

    public class ReorderableListDrawer : FriggPropertyDrawer {
        public const byte LIST_INTERFACE_WIDTH = 50;
        private ReorderableList list;

        public override float GetHeight() {
            //header
            var total = 0f;
            if (this.list.displayAdd || this.list.displayRemove) {
                //button
                if(this.property.IsExpanded)
                   total += EditorGUIUtility.singleLineHeight * 1.5f;
            }
            
            var layout = this.property.PropertyTree.Layouts.FirstOrDefault(x => x.layoutPath == this.property.Path);
            
            if (layout != null) {
                total += layout.TotalHeight;
                return total;
            }
            
            total += FriggProperty.GetPropertyHeight(this.property) + 4f * this.list.list.Count; // paddings
            
            if(this.property.IsExpanded)
                return total;
            
            return EditorGUIUtility.singleLineHeight;
        }

        public override bool  IsVisible => true;

        public ReorderableListDrawer(FriggProperty prop) : base(prop) {
            var elements = (IList) prop.GetValue();
            if (this.list == null)
                this.list = new ReorderableList(elements, CoreUtilities.TryGetListElementType(elements.GetType()));
            
            if(!string.IsNullOrEmpty(this.property.UnityPath)) 
                this.list.serializedProperty = 
                    this.property.PropertyTree.SerializedObject.FindProperty(this.property.UnityPath);
        }

        public override void Draw(Rect rect) {
            rect.y += GuiUtilities.SPACE;
            this.SetCallbacks(this.list, rect);
            rect.y += EditorGUIUtility.singleLineHeight;

            rect.width -= EditorGUI.indentLevel * 15;
            rect.x     += EditorGUI.indentLevel * 15;

            if(this.property.IsExpanded)
               this.list.DoList(rect);
        }
        
        public override void DrawLayout() {
            var elements = (IList) this.property.GetValue(); 
            
            if(this.list == null)
               this.list = new ReorderableList(elements, CoreUtilities.TryGetListElementType(elements.GetType()));
            
            this.SetCallbacks(this.list);

            if(this.property.IsExpanded)
               this.list.DoLayoutList();
        }

        private void SetCallbacks(ReorderableList reorderableList, Rect rect = default) {
            this.list = reorderableList;

            this.property.Label.text = $"{this.property.NiceName} - {this.property.MetaInfo.arraySize} elements.";
            this.property.IsExpanded = GuiUtilities.FoldoutToggle(this.property, rect);
            
            this.list.draggable      = this.list.displayAdd = this.list.displayRemove = true;
            this.list.headerHeight   = 1;
            
            var attr                         = this.property.TryGetFixedAttribute<ListDrawerSettingsAttribute>();
            if (attr != null) {
                this.list.draggable     = attr.AllowDrag;
                this.list.displayAdd    = !attr.HideAddButton;
                this.list.displayRemove = !attr.HideRemoveButton;
            }

            this.list.drawElementCallback = (tempRect, index, active, focused) => {
                if (!this.property.IsExpanded)
                    return;
                
                var pr = this.property.GetArrayElementAtIndex(index);

                tempRect.y      += GuiUtilities.SPACE / 2f;
                tempRect.height =  EditorGUIUtility.singleLineHeight;
                tempRect.width  += EditorGUI.indentLevel * 15;
                tempRect.x      -= EditorGUI.indentLevel * 15;
                
                pr.Draw(tempRect);
            };

            var listType = CoreUtilities.TryGetListElementType(this.list.list.GetType());

            this.list.onAddCallback = _ => {
                var copy = this.list.list;

                this.list.list = Array.CreateInstance(listType, copy.Count + 1);
                for (var i = 0; i < copy.Count; i++) {
                    this.list.list[i] = copy[i];
                }

                if (!string.IsNullOrEmpty(this.property.UnityPath)) {
                    var sp = this.property.PropertyTree.SerializedObject.FindProperty(this.property.UnityPath);
                    sp.arraySize++;
                }
                
                this.property.MetaInfo.arraySize++;
            };

            this.list.onRemoveCallback = _ => {
                var copy      = this.list.list;
                var element   = this.property.GetArrayElementAtIndex(this.list.index);
                var newLength = copy.Count - 1;

                this.list.list = Array.CreateInstance(listType, newLength);
                for (var i = 0; i < this.list.index; i++)
                    this.list.list[i] = copy[i];

                for (var i = this.list.index; i < newLength; i++)
                    this.list.list[i] = copy[i + 1];

                var childPath  = element.UnityPath;
                var propertyPath = this.property.UnityPath;

                if (string.IsNullOrEmpty(childPath)) {
                    return;
                }

                var parentProperty = this.property.PropertyTree.SerializedObject.FindProperty(propertyPath);

                this.property.MetaInfo.arraySize--;
                parentProperty.arraySize--;
            };

            this.list.elementHeightCallback = index => {
                var element = this.property.GetArrayElementAtIndex(index);
                var layout  = this.property.PropertyTree.Layouts.FirstOrDefault(x => x.layoutPath == element.Path);
            
                if (layout != null) {
                    return layout.TotalHeight;
                }
                
                if (!this.property.IsExpanded)
                    return EditorGUIUtility.singleLineHeight;
                
                var height  = FriggProperty.GetPropertyHeight(element);
                return height + GuiUtilities.SPACE;
            };
        }
    }
}