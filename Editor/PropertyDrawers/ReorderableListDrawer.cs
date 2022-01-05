namespace Frigg.Editor {
    using System.Collections;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using Utils;

    public class ReorderableListDrawer : FriggPropertyDrawer {
        public const     byte            LIST_INTERFACE_WIDTH = 50;
        private const    byte            EMPTY_SIZE           = 22;
        private readonly ReorderableList list;

        private float total;
        private float elementsHeight;

        public override float GetHeight() {
            //header + space in the end
            this.total = EditorGUIUtility.singleLineHeight;

            if (!this.property.IsExpanded) 
                return this.total;
            
            //footer
            if (this.list.displayAdd || this.list.displayRemove) {
                this.total += this.list.footerHeight;
            }

            if (this.list.count == 0) {
                this.total += EMPTY_SIZE;
                return this.total;
            }

            this.elementsHeight = 0f;

            if (this.getHeightAction == null) {
                this.getHeightAction = friggProperty => {
                    if (this.property.PropertyTree.LayoutsByPath.TryGetValue(friggProperty.Path, out var val)) {
                        this.total += val.TotalHeight;
                        return;
                    }

                    var height = FriggProperty.GetPropertyHeight(friggProperty);
                    this.elementsHeight += height;
                };
            }

            this.property.ChildrenProperties.RecurseChildren(this.getHeightAction);

            this.total += this.elementsHeight;
            return this.total;
        }

        public override bool  IsVisible => true;

        public ReorderableListDrawer(FriggProperty prop) : base(prop) {
            var elements = (IList) prop.GetValue();
            this.list = new ReorderableList(elements, 
                CoreUtilities.TryGetListElementType(elements.GetType()));
        }

        public override void Draw(Rect rect) {
            this.SetCallbacks(this.list, rect);
            
            rect.y     += EditorGUIUtility.singleLineHeight;
            rect.width -= EditorGUI.indentLevel * 15;
            rect.x     += EditorGUI.indentLevel * 15;

            if(this.property.IsExpanded)
               this.list.DoList(rect);
        }
        
        public override void DrawLayout() {
            this.SetCallbacks(this.list);

            if (!this.property.IsExpanded) {
                return;
            }

            var controlRect = EditorGUILayout.GetControlRect(true, this.GetHeight() 
                - EditorGUIUtility.singleLineHeight + GuiUtilities.SPACE);
            
            controlRect.width -= EditorGUI.indentLevel * 15;
            controlRect.x     += EditorGUI.indentLevel * 15;
            this.list.DoList(controlRect);
        }

        private void SetCallbacks(ReorderableList reorderableList, Rect rect = default) {
            var elements = (IList) this.property.GetValue();

            reorderableList.list = elements;
            
            if (this.list.count < this.property.MetaInfo.arraySize) {
                for (var i = this.list.count; i <= this.property.MetaInfo.arraySize; i++) {
                    this.property.ChildrenProperties.RemoveProperty(i);
                    EditorUtility.SetDirty(this.property.PropertyTree
                        .SerializedObject.targetObject);
                }

                this.property.MetaInfo.arraySize = this.list.count;
            }

            //check for array size
            this.property.Label.text = $"{this.property.NiceName} - {this.list.count} elements.";
            
            var attr = this.property.TryGetFixedAttribute<ListDrawerSettingsAttribute>();

            reorderableList.draggable    = reorderableList.displayAdd = reorderableList.displayRemove = true;
            reorderableList.headerHeight = 1;
            
            if (attr != null) {
                reorderableList.draggable     = attr.AllowDrag;
                reorderableList.displayAdd    = !attr.HideAddButton;
                reorderableList.displayRemove = !attr.HideRemoveButton;
            }
            
            this.property.IsExpanded = GuiUtilities.FoldoutToggle(this.property, rect);
            var isReadOnly = this.property.IsReadonly;

            if (isReadOnly) {
                reorderableList.draggable = 
                    reorderableList.displayAdd =
                        reorderableList.displayRemove = false;
            }

            reorderableList.drawElementCallback = (tempRect, index, active, focused) => {
                if (!this.property.IsExpanded)
                    return;
                
                var pr = this.property.GetArrayElementAtIndex(index);

                tempRect.y      += GuiUtilities.SPACE / 2f;
                tempRect.height =  EditorGUIUtility.singleLineHeight;
                tempRect.width  += EditorGUI.indentLevel * 15;
                tempRect.x      -= EditorGUI.indentLevel * 15;
                
                pr.Draw(tempRect);
            };

            reorderableList.onAddCallback = _ => {
                this.property.AddArrayElement(this.property.MetaInfo.arraySize);
            };

            reorderableList.onRemoveCallback = l => {
                this.property.RemoveArrayElement(l.index);
            };

            reorderableList.elementHeightCallback = index => {
                if (index >= this.property.MetaInfo.arraySize)
                    return 0f;
                
                var element = this.property.GetArrayElementAtIndex(index);
                this.property.PropertyTree.LayoutsByPath.TryGetValue
                    (element.Path, out var layout);
                
                if (layout != null) {
                    return layout.TotalHeight;
                }
                
                if (!this.property.IsExpanded)
                    return EditorGUIUtility.singleLineHeight;
                
                var height = FriggProperty.GetPropertyHeight(element);
                return height;
            };
            
            this.property.CallNextDrawer(rect);
        }
    }
}