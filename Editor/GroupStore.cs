namespace Frigg.Editor {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEngine;

    public class GroupsHandler {
        public static List<GroupInfo> activeGroups = new List<GroupInfo>();

        public static Rect GetLastRect(GroupInfo info) {
            var group = TryGetInfo(info);
            
            return group.currentRect;
        }

        public static void ResetRects() {
            foreach (var group in activeGroups) {
                group.currentRect = Rect.zero;
            }
        }

        public static void ResetGroups() {
            activeGroups = new List<GroupInfo>();
        }

        public static float GetGroupsHeight(FriggProperty prop) {
            var   groups = activeGroups.FindAll(x => x.Path == prop.Path);
            float height = 0;

            foreach (var group in groups) {
                height += group.GroupHeight;
            }

            return height;
        }

        public static void Init(GroupInfo group, Rect rect) {
            UpdateGroupInfo(group, rect);
        }

        public static void Clear() {
            activeGroups = new List<GroupInfo>();
        }

        public static void UpdateGroupInfo(GroupInfo info, Rect rect) {
            var data  = TryGetInfo(info);
            var index = activeGroups.IndexOf(data);
            rect.x    += GuiUtilities.SPACE * 2;
            activeGroups[index].currentRect =  rect;
        }

        public static void AddGroup(GroupInfo info) {
            if(!activeGroups.Contains(info))
               activeGroups.Add(info);
        }

        public static bool IsDefault(GroupInfo info) {
            var data = TryGetInfo(info);
            return data.currentRect == Rect.zero;
        }

        public static float GetAvailableWidth(GroupInfo info) {
            return Mathf.Abs(info.currentRect.width - EditorGUIUtility.currentViewWidth);
        }

        private static GroupInfo TryGetInfo(GroupInfo info) {
            var group = activeGroups.FirstOrDefault(x => x.Path == info.Path && x.Name == info.Name);
            if (group == null) {
                throw new Exception($"There's no any groups at path {info.Path}");
            }

            return group;
        }
    }
}