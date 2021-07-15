namespace Frigg.Editor {
    using System;
    using System.Collections;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using Utils;

    public class ReorderableListDrawer : FriggPropertyDrawer {
        private         ReorderableList list;

        public override float GetHeight() {
            //header
            var total = 0f;
            if (this.list.displayAdd || this.list.displayRemove) {
                //button
                if(this.property.IsExpanded)
                   total += EditorGUIUtility.singleLineHeight * 1.5f;
            }
            
            total += FriggProperty.GetPropertyHeight(this.property) + 4f * this.list.list.Count; // paddings
            
            if(this.property.IsExpanded)
                return total;
            
            return EditorGUIUtility.singleLineHeight;
        }

        public override bool  IsVisible => true;

        public ReorderableListDrawer(FriggProperty prop) : base(prop) {
            var elements = (IList) prop.PropertyValue.Value;
            if (this.list == null)
                this.list = new ReorderableList(elements, CoreUtilities.TryGetListElementType(elements.GetType()));
        }

        public override void Draw(Rect rect) {
            rect.y += GuiUtilities.SPACE;
            this.SetCallbacks(this.list, this.property, rect);
            rect.y += EditorGUIUtility.singleLineHeight;

            rect.width -= EditorGUI.indentLevel * 15;
            rect.x     += EditorGUI.indentLevel * 15;

            if(this.property.IsExpanded)
               this.list.DoList(rect);
        }
        
        public override void DrawLayout() {
            var elements = (IList) this.property.PropertyValue.Value;
            
            if(this.list == null)
               this.list = new ReorderableList(elements, CoreUtilities.TryGetListElementType(elements.GetType()));
            
            this.SetCallbacks(this.list, this.property);

            if(this.property.IsExpanded)
               this.list.DoLayoutList();
        }

        private void SetCallbacks(ReorderableList reorderableList, FriggProperty prop, Rect rect = default) {
            this.list = reorderableList;

            this.property.Label.text = $"{this.property.NiceName} - {this.property.MetaInfo.arraySize} elements.";
            this.property.IsExpanded = GuiUtilities.FoldoutToggle(this.property, rect);
            
            this.list.draggable      = this.list.displayAdd = this.list.displayRemove = true;
            this.list.headerHeight   = 1;
            
            var attr                         = prop.TryGetFixedAttribute<ListDrawerSettingsAttribute>();
            if (attr != null) {
                this.list.draggable     = attr.AllowDrag;
                this.list.displayAdd    = !attr.HideAddButton;
                this.list.displayRemove = !attr.HideRemoveButton;
            }

            this.list.drawElementCallback = (tempRect, index, active, focused) => {
                if (!prop.IsExpanded)
                    return;
                
                var pr = prop.GetArrayElementAtIndex(index);

                tempRect.y      += GuiUtilities.SPACE / 2f;
                tempRect.height =  EditorGUIUtility.singleLineHeight;
                tempRect.width  += EditorGUI.indentLevel * 15;
                tempRect.x      -= EditorGUI.indentLevel * 15;
                
                EditorGUI.BeginChangeCheck();
                pr.Draw(tempRect);
                if (EditorGUI.EndChangeCheck()) {
                    CoreUtilities.OnValueChanged(pr);
                }
            };

            var listType = CoreUtilities.TryGetListElementType(this.list.list.GetType());

            this.list.onAddCallback = delegate {
                var copy = this.list.list;

                this.list.list = Array.CreateInstance(listType, copy.Count + 1);
                for (var i = 0; i < copy.Count; i++) {
                    this.list.list[i] = copy[i];
                }

                if (!string.IsNullOrEmpty(prop.UnityPath)) {
                    var sp = prop.PropertyTree.SerializedObject.FindProperty(prop.UnityPath);
                    sp.arraySize++;
                }

                this.property.MetaInfo.arraySize++;
            };

            this.list.onRemoveCallback = delegate {
                var copy      = this.list.list;
                var newLength = copy.Count - 1;

                this.list.list = Array.CreateInstance(listType, newLength);
                for (var i = 0; i < this.list.index; i++)
                    this.list.list[i] = copy[i];

                for (var i = this.list.index; i < newLength; i++)
                    this.list.list[i] = copy[i + 1];
                
                if (!string.IsNullOrEmpty(prop.GetArrayElementAtIndex(this.list.index).UnityPath)) {
                    var sp = prop.PropertyTree.SerializedObject.FindProperty(prop.Name);
                    sp.DeleteArrayElementAtIndex(this.list.index);
                    sp.arraySize--;
                }
                
                this.property.MetaInfo.arraySize--;
            };

            this.list.elementHeightCallback = index => {
                if (!prop.IsExpanded)
                    return EditorGUIUtility.singleLineHeight;
                
                var element = this.property.GetArrayElementAtIndex(index);
                var height  = FriggProperty.GetPropertyHeight(element);
                return height + GuiUtilities.SPACE;
            };
        }
    }
}