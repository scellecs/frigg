namespace Frigg.Editor {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;

    public abstract class PropertyTree {
        public abstract SerializedObject SerializedObject { get; }
        
        public abstract Type             TargetType       { get; }
        public abstract List<object>     MemberTargets    { get; }
        
        public static PropertyTree InitTree(object target) {
            if (target == null)
                throw new ArgumentNullException("target");

            return InitTree(new[] {target}, null);
        }
        
        public static PropertyTree InitTree(params object[] targets) {
            if (targets == null)
                throw new ArgumentNullException("targets");

            return InitTree((IList) targets);
        }
        
        public static PropertyTree InitTree(IList targets) {
            if (targets == null)
                throw new ArgumentNullException("targets");

            return InitTree(targets, null);
        }
        
        public static PropertyTree InitTree(SerializedObject target) {
            if (target == null)
                throw new ArgumentNullException("target");

            return InitTree(new object[] {target}, null);
        }

        private static PropertyTree InitTree(IList targets, SerializedObject serializedObject) {
            if (targets == null)
            {
                throw new ArgumentNullException("targets");
            }

            if (targets.Count == 0)
            {
                throw new ArgumentException("There must be at least one target.");
            }

            if (serializedObject != null)
            {
                bool valid = true;
                var targetObjects = serializedObject.targetObjects;

                if (targets.Count != targetObjects.Length)
                {
                    valid = false;
                }
                else
                {
                    for (int i = 0; i < targets.Count; i++)
                    {
                        if (!object.ReferenceEquals(targets[i], targetObjects[i]))
                        {
                            valid = false;
                            break;
                        }
                    }
                }

                if (!valid)
                {
                    throw new ArgumentException("Given target array must be identical in length and content to the target objects array in the given serializedObject.");
                }
            }

            Type targetType = null;

            for (int i = 0; i < targets.Count; i++)
            {
                Type otherType;
                object target = targets[i];

                if (ReferenceEquals(target, null))
                {
                    throw new ArgumentException("Target at index " + i + " was null.");
                }

                if (i == 0)
                {
                    targetType = target.GetType();
                }
                else if (targetType != (otherType = target.GetType()))
                {
                    if (targetType.IsAssignableFrom(otherType))
                    {
                        continue;
                    } 
                    
                    if (otherType.IsAssignableFrom(targetType))
                    {
                        targetType = otherType;
                        continue;
                    }

                    throw new ArgumentException("Expected targets of type " + targetType.Name + ", but got an incompatible target of type " + otherType.Name + " at index " + i + ".");
                }
            }

            var treeType = typeof(PropertyTree<>).MakeGenericType(targetType);
            Array targetArray;

            if (targets.GetType().IsArray && targets.GetType().GetElementType() == targetType)
            {
                targetArray = (Array)targets;
            }
            else
            {
                targetArray = Array.CreateInstance(targetType, targets.Count);
                targets.CopyTo(targetArray, 0);
            }

            if (serializedObject == null && targetType.IsAssignableFrom(typeof(UnityEngine.Object)))
            {
                UnityEngine.Object[] objs = new UnityEngine.Object[targets.Count];
                targets.CopyTo(objs, 0);

                serializedObject = new SerializedObject(objs);
            }

            return (PropertyTree)Activator.CreateInstance(treeType, targetArray, serializedObject);
        }

        public void Draw() {
            
        }
    }

    public class PropertyTree<T> : PropertyTree {
        private SerializedObject serializedObject;
        private Type             targetType;
        private List<object>     memberTargets;

        private static bool isValueType   = typeof(T).IsValueType;
        private static bool isUnityObject = typeof(UnityEngine.Object).IsAssignableFrom(typeof(T));

        public override SerializedObject SerializedObject => this.serializedObject;
        public override Type             TargetType       => this.targetType;
        public override List<object>     MemberTargets    => this.memberTargets;
        
        public 
    }
}