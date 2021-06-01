namespace Frigg.Editor {
    using System;
    using Groups;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public class InlinePropertyDrawer : FriggPropertyDrawer {
        public InlinePropertyDrawer(FriggProperty prop) : base(prop) {
        }
        public override void DrawLayout() {
            this.property.IsExpanded = true;
            foreach (var p in this.property.ChildrenProperties.RecurseChildren()) {
                p.Draw();
            }
        }

        public override void Draw(Rect rect) {
            this.property.IsExpanded = true;
            foreach (var p in this.property.ChildrenProperties.RecurseChildren()) {
                var attr    = p.TryGetFixedAttribute<BaseGroupAttribute>();
                
                EditorGUI.BeginChangeCheck();
                if (attr != null) {
                    rect.width = attr.ElementWidth;
                    var group   = new GroupInfo(attr.GroupName, p.ParentProperty.Path);
                    if (GroupsHandler.IsDefault(group)) {
                        GroupsHandler.Init(group, rect);
                    }
                    
                    var newRect = GroupsHandler.GetLastRect(group);
                    rect = newRect;
                    p.Draw(rect);
                    GroupsHandler.UpdateGroupInfo(group, new Rect(rect.x + attr.ElementWidth, rect.y, GroupsHandler.GetAvailableWidth(group) - 105, rect.height));
                }

                else {
                    var h = FriggProperty.GetPropertyHeight(p);
                    p.Draw(rect);
                    rect.y     += h + GuiUtilities.SPACE / 2f;
                }
                
                if (EditorGUI.EndChangeCheck()) {
                    CoreUtilities.OnValueChanged(p);
                }
            }
            
            GroupsHandler.ResetGroups();
        }

        public override float GetHeight() => FriggProperty.GetPropertyHeight(this.property);

        public override bool IsVisible => true;
        
    }
}